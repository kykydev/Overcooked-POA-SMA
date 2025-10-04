using System.Collections;
using System.Collections.Generic;
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

    private AssemblyStation m_assemblyStation;
    private List<CookingStation> m_cookingStation;
    private List<CuttingStation> m_cuttingStation;

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
    public void SetAssemblyStation(AssemblyStation _assemblyStation) => m_assemblyStation = _assemblyStation;
    public void SetCookingStation(List<CookingStation> _cookingStation) => m_cookingStation = _cookingStation;
    public void SetCuttingStation(List<CuttingStation> _cuttingStation) => m_cuttingStation = _cuttingStation;
    public void SetKitchenManager(KitchenManager _kitchenManager) => m_kitchenManager = _kitchenManager;


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
        m_navAgent.SetDestination(_counter.transform.position);

        //Attendre que l’agent soit proche
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 2f);

        //Récupérer la commande
        m_kitchenManager.SetCurrentOrder(_counter.GiveOrder());
        if (m_kitchenManager.GetCurrentOrder() != null)
        {
            m_kitchenManager.GetCurrentOrder().SetStatus(OrderStatus.Preparing);
            Debug.Log(m_agentID + " took order: " + m_kitchenManager.GetCurrentOrder().GetOrderId());
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
        yield return new WaitUntil(() => Vector3.Distance(transform.position, container.transform.position) < 2f);

        //Récupérer l’ingrédient
        if (container is IngredientStation station)
            m_agentMain = station.GetIngredient();
        else if (container is CookingStation cooking)
            m_agentMain = cooking.GetIngredient();

        ShowIngredientInHand();


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
                yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestCooking.transform.position) < 2f);

                nearestCooking.ShowIngredientOnStation(ingredient);
                nearestCooking.StartCoroutine(nearestCooking.CookIngredient(ingredient));

                Debug.Log(m_agentID + " placing: " + ingredient.GetName() + " on cookingStation");
                m_agentMain = null;
                ShowIngredientInHand();

                nearestCooking.UnlockStation();

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
                yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestCutting.transform.position) < 2f);

                nearestCutting.ShowIngredientOnStation(ingredient);
                m_agentMain = null;
                ShowIngredientInHand();
                Debug.Log(m_agentID + " placing: " + ingredient.GetName() + " on cuttingStation");
                Debug.Log(m_agentID + " cutting: " + ingredient.GetName());

                yield return StartCoroutine(nearestCutting.CutIngredient(ingredient));
                Debug.Log(m_agentID + " has cut: " + ingredient.GetName());
                m_agentMain = nearestCutting.GetIngredient();
                ShowIngredientInHand();

                nearestCutting.UnlockStation();

                MoveTo(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));
            }
        }
        else
        {
            Debug.Log(m_agentID + " picked up: " + m_agentMain?.GetType().Name);
        }


        //Se déplacer vers la workstation d’assemblage
        MoveTo(m_assemblyStation.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, m_assemblyStation.transform.position) < 2f);

        //Déposer l’objet sur la workstation d’assemblage
        if (m_agentMain is Ingredient ingredientToPlace)
        {
            m_assemblyStation.AddIngredient(ingredientToPlace);
            m_agentMain = null;
            ShowIngredientInHand();
            Debug.Log(m_agentID + " placed: " + ingredientToPlace.GetName() + " on assemblyStation");
        }

        if (m_assemblyStation.CanAssembleDish(m_kitchenManager.GetCurrentOrder()))
        {
            Debug.Log(m_agentID + " is assembling the dish...");
            yield return StartCoroutine(m_assemblyStation.AssembleDish(m_kitchenManager.GetCurrentOrder()));
            Dish plat = m_assemblyStation.GetPreparedDish();
            Debug.Log(m_agentID + " assembled the dish: " + (plat != null ? plat.GetName() : "Failed to assemble"));
            m_agentMain = plat;
            ShowIngredientInHand();
        }

        MoveTo(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));

    }




    private IEnumerator DeliverOrderRoutine(Counter _counter)
    {
        Order currentOrder = m_kitchenManager.GetCurrentOrder();
        if (currentOrder == null)
            yield break;

        if(m_agentMain is Dish dish)
        {
            //Déplacer l’agent vers le comptoir
            MoveTo(_counter.transform.position);

            //Attendre que l’agent arrive à proximité
            yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 2f);

            //Livrer la commande
            _counter.ReceiveOrder(currentOrder, dish);
            m_agentMain = null;
            ShowIngredientInHand();
            Debug.Log(m_agentID + " delivered order: " + currentOrder.GetOrderId());
        }

        //Nettoyer la commande dans le KitchenManager
        m_assemblyStation.ClearStation();
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




    public void ShowIngredientInHand()
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
    }


}
