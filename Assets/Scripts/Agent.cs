using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using UnityEngineInternal;

public class Agent : MonoBehaviour
{
    /// --- Attributes ---
    private int m_agentID;
    private string m_agentName;
    private object m_agentMain;

    private List<CookingStation> m_cookingStation;
    private List<CuttingStation> m_cuttingStation;
    private List<TableStation> m_tableStation;
    private PlateStation m_plateStation;
    private WashStation m_washStation;

    [SerializeField] private NavMeshAgent m_navAgent;
    private KitchenManager m_kitchenManager;
    
    /// ---- Getters ----
    public int GetAgentID() => m_agentID;
    public string GetAgentName() => m_agentName;
    public object GetAgentMain() => m_agentMain;

    /// ---- Setters ----
    public void SetAgentID(int _id) => m_agentID = _id;
    public void SetAgentName(string _name) => m_agentName = _name;  
    public void SetAgentMain(object _main) => m_agentMain = _main;
    public void SetCookingStation(List<CookingStation> _cookingStation) => m_cookingStation = _cookingStation;
    public void SetCuttingStation(List<CuttingStation> _cuttingStation) => m_cuttingStation = _cuttingStation;
    public void SetTableStation(List<TableStation> _tableStation) => m_tableStation = _tableStation;
    public void SetPlateStation(PlateStation _plateStation) => m_plateStation = _plateStation;
    public void SetKitchenManager(KitchenManager _kitchenManager) => m_kitchenManager = _kitchenManager;
    public void SetWashStation(WashStation _washStation) => m_washStation = _washStation;


    /// ---- Methods ----

    // Déplacer l’agent vers une position
    public void MoveTo(Vector3 _direction)
    {
        m_navAgent.SetDestination(_direction);
    }




    // Lancer la routine de travail
    public void Work(Counter _counter)
    {
        StartCoroutine(WorkRoutine(_counter));
    }




    private IEnumerator WorkRoutine(Counter _counter)
    {
        while (true)
        {

            //Si aucune commande, en prendre une
            if (!m_kitchenManager.HasCurrentOrder() && !m_kitchenManager.IsOrderBeingTaken())
            {
                m_kitchenManager.SetOrderBeingTaken(true);
                yield return StartCoroutine(TakeOrderRoutine(_counter));
                yield return StartCoroutine(FetchAndAssignPlate());
                m_kitchenManager.SetOrderBeingTaken(false);
            }


            //Récupérer les ingrédients un par un
            if (m_kitchenManager.HasCurrentOrder())
            {
                Ingredient nextIngredient = m_kitchenManager.GetCurrentIngredient();
                while (nextIngredient != null)
                {
                    yield return StartCoroutine(FetchIngredientRoutine(nextIngredient));
                    nextIngredient = m_kitchenManager.GetCurrentIngredient();
                }

                // Si l'agent porte un plat, il livre la commande
                if (m_agentMain is Dish)
                {
                    if (!m_kitchenManager.IsOrderBeingDelivered())
                    {
                        m_kitchenManager.SetOrderBeingDelivered(true);

                        yield return StartCoroutine(DeliverOrderRoutine(_counter));

                        m_kitchenManager.SetOrderBeingTaken(true);
                        yield return StartCoroutine(TakeOrderRoutine(_counter));
                        yield return StartCoroutine(FetchAndAssignPlate());
                        m_kitchenManager.SetOrderBeingTaken(false);

                        m_kitchenManager.SetOrderBeingDelivered(false);
                    }
                }
            }

            //Attendre la prochaine boucle
            yield return null;
        }
    }




    // Prise de commande au comptoir
    private IEnumerator TakeOrderRoutine(Counter _counter)
    {

        //Se déplacer vers le comptoir
        MoveTo(_counter.transform.position);

        //Attendre que l’agent soit proche
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 1.5f);

        //Récupérer la commande
        m_kitchenManager.SetCurrentOrder(_counter.GiveOrder());
        if (m_kitchenManager.GetCurrentOrder() != null)
        {
            m_kitchenManager.GetCurrentOrder().SetStatus(OrderStatus.Preparing);
            Debug.Log(m_agentID + " took order: " + m_kitchenManager.GetCurrentOrder().GetOrderId());
        }
    }

    // Prise de commande au comptoir
    private IEnumerator FetchAndAssignPlate()
    {
        if (m_agentMain is Plate plate)
        {
            if (!plate.IsClean())
            {
                m_washStation.LockStation();

                //Se déplacer vers le bac à vaisselle
                MoveTo(m_washStation.transform.position);
                //Attendre que l’agent soit proche
                yield return new WaitUntil(() => Vector3.Distance(transform.position, m_washStation.transform.position) < 1.5f);
                m_agentMain = null;
                ShowObjectInHand();
                //Nettoyer l'assiette
                Debug.Log(m_agentID + " is washing the plate.");
                yield return StartCoroutine(m_washStation.WashPlate(plate));
                m_agentMain = m_washStation.GetObject();
                ShowObjectInHand();

                m_washStation.UnlockStation();
            }
        }

        else if (m_agentMain == null)
        {
            m_plateStation.LockStation();

            //Se déplacer vers la station d'assiette
            MoveTo(m_plateStation.transform.position);
            //Attendre que l’agent soit proche
            yield return new WaitUntil(() => Vector3.Distance(transform.position, m_plateStation.transform.position) < 1.5f);
            //Récupérer une assiette propre
            m_agentMain = m_plateStation.GetPlate();
            ShowObjectInHand();

            m_plateStation.UnlockStation();
        }

        if (m_agentMain is Plate newPlate)
        {
            Debug.Log(m_agentID + " picked up a clean plate.");
            //Va a la table la plus proche et assigne l'assiette a la commande
            var nearestTable = FindNearestAvailableStation(m_tableStation);
            if (nearestTable != null)
            {
                MoveTo(nearestTable.transform.position);
                yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestTable.transform.position) < 1.5f);
                nearestTable.AssignPlateToOrder(newPlate, m_kitchenManager.GetCurrentOrder());
                Debug.Log(m_agentID + " assigned plate to order" + m_kitchenManager.GetCurrentOrder().GetOrderId());
                m_agentMain = null;
                ShowObjectInHand();
            }
        }

    }




    // Récupération d’un ingrédient et dépose sur la workstation
    private IEnumerator FetchIngredientRoutine(Ingredient _ingredient)
    {
        if (_ingredient == null)
            yield break;

        WorkStation container = _ingredient.GetContainer();
        if (container == null)
            yield break;

        //Se déplacer vers le bac
        MoveTo(container.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, container.transform.position) < 1.5f);

        //Récupérer l’ingrédient
        if (container is IngredientStation station)
            m_agentMain = station.GetIngredient();
        else if (container is CookingStation cooking)
            m_agentMain = cooking.GetObject();

        ShowObjectInHand();


        if (m_agentMain is Ingredient ingredient)
        {
            Debug.Log(m_agentID + " picked up: " + ingredient.GetName());

            //Vérifier si l’ingrédient doit être cuit
            if (ingredient.NeedsCooking())
            {
                var nearestCooking = FindNearestAvailableStation(m_cookingStation);
                if (nearestCooking == null)
                    yield break;

                nearestCooking.LockStation();

                MoveTo(nearestCooking.transform.position);
                yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestCooking.transform.position) < 1.5f);

                m_agentMain = null;
                ShowObjectInHand();
                Debug.Log(m_agentID + " placing: " + ingredient.GetName() + " on cookingStation");
                nearestCooking.StartCoroutine(nearestCooking.CookIngredient(ingredient));

                MoveTo(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));
                yield break;
            }

            //Vérifier si l’ingrédient doit être coupé
            if (ingredient.NeedsCutting())
            {
                var nearestCutting = FindNearestAvailableStation(m_cuttingStation);
                if (nearestCutting == null)
                    yield break;

                nearestCutting.LockStation();

                MoveTo(nearestCutting.transform.position);
                yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestCutting.transform.position) < 1.5f);

                m_agentMain = null;
                ShowObjectInHand();
                Debug.Log(m_agentID + " placing: " + ingredient.GetName() + " on cuttingStation");
                Debug.Log(m_agentID + " cutting: " + ingredient.GetName());

                yield return StartCoroutine(nearestCutting.CutIngredient(ingredient));
                m_agentMain = nearestCutting.GetObject();
                ShowObjectInHand();

                nearestCutting.UnlockStation();

                MoveTo(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));
            }
        }
        else
        {
            Debug.Log(m_agentID + " picked up: " + m_agentMain?.GetType().Name);
        }


        //Se déplacer vers l'assiette assignée a la commande
        var tableStation = m_kitchenManager.GetCurrentOrder().GetTableStation();
        var plate = tableStation.GetPlate();
        MoveTo(tableStation.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, tableStation.transform.position) < 1.5f);

        //Déposer l’objet sur la workstation d’assemblage
        if (m_agentMain is Ingredient ingredientToPlace)
        {
            plate.AddIngredient(ingredientToPlace);
            m_agentMain = null;
            ShowObjectInHand();
            Debug.Log(m_agentID + " placed: " + ingredientToPlace.GetName() + " on plate");
            tableStation.UpdatePlateVisual();
        }

        if (plate.CanAssembleDish(m_kitchenManager.GetCurrentOrder()))
        {
            Debug.Log(m_agentID + " is assembling the dish...");
            yield return StartCoroutine(plate.AssembleDish(m_kitchenManager.GetCurrentOrder()));
            Dish plat = plate.GetPreparedDish();
            Debug.Log(m_agentID + " assembled the dish: " + (plat != null ? plat.GetName() : "Failed to assemble"));
            m_agentMain = plat;
            ShowObjectInHand();
            //Enlever l'assiette de la table
            tableStation.RemovePlate();

        }

        MoveTo(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));

    }

    private IEnumerator DeliverOrderRoutine(Counter _counter)
    {
        Order currentOrder = m_kitchenManager.GetCurrentOrder();
        if (currentOrder == null)
            yield break;

        if (m_agentMain is Dish dish)
        {
            //Déplacer l’agent vers le comptoir
            MoveTo(_counter.transform.position);

            //Attendre que l’agent arrive à proximité
            yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 1.5f);

            //Livrer la commande
            _counter.ReceiveOrder(currentOrder, dish);
            m_agentMain = null;
            ShowObjectInHand();
            Debug.Log(m_agentID + " delivered order: " + currentOrder.GetOrderId());

            // Créer une nouvelle assiette sale et la donner à l'agent
            Plate dirtyPlate = new Plate(m_kitchenManager.m_platePrefab);
            dirtyPlate.SetState(PlateState.Dirty);
            m_agentMain = dirtyPlate;
            ShowObjectInHand();
            Debug.Log(m_agentID + " picked up new dirty plate.");
        }

        //Nettoyer la commande dans le KitchenManager
        currentOrder.GetTableStation().RemovePlate();
        m_kitchenManager.ClearCurrentOrder();
    }






    private T FindNearestAvailableStation<T>(List<T> stations) where T : WorkStation
    {
        T nearest = null;
        float minDist = float.MaxValue;
        foreach (var station in stations)
        {
            if (station is CuttingStation cut && cut.IsBusy ) continue;
            if (station is CookingStation cook && (cook.HasCookedIngredient() || cook.IsBusy)) continue;

            float dist = Vector3.Distance(transform.position, station.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = station;
            }
        }
        return nearest;
    }




    public void ShowObjectInHand()
    {
        Transform handTransform = transform.Find("Hand");
        if (handTransform == null)
            return;

        foreach (Transform child in handTransform)
            GameObject.Destroy(child.gameObject);

        if (m_agentMain is Ingredient ingredient && ingredient.GetPrefab() != null)
        {
            GameObject ingredientObj = GameObject.Instantiate(ingredient.GetPrefab(), handTransform);
            ingredientObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        else if (m_agentMain is Dish dish && dish.GetPrefab() != null)
        {
            GameObject dishObj = GameObject.Instantiate(dish.GetPrefab(), handTransform);
            dishObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        else if (m_agentMain is Plate plate && plate.GetPrefab() != null)
        {
            GameObject plateObj = GameObject.Instantiate(plate.GetPrefab(), handTransform);
            plateObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }


}
