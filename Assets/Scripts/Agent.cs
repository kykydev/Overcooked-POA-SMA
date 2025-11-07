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
    private List<PlateStation> m_plateStation;
    private List<WashStation> m_washStation;

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
    public void SetKitchenManager(KitchenManager _kitchenManager) => m_kitchenManager = _kitchenManager;
    public void SetPlateStation(List<PlateStation> _plateStation) => m_plateStation = _plateStation;
    public void SetWashStation(List<WashStation> _washStation) => m_washStation = _washStation;

    /// ---- Methods ----

    /// <summary>
    /// Méthode pour déplacer l'agent vers une direction donnée.
    /// </summary>
    /// <param name="_direction"></param>  
    public void MoveTo(Vector3 _direction)
    {
        m_navAgent.SetDestination(_direction);
    }


    /// <summary>
    /// Lance la routine de travail de l'agent.
    /// </summary>
    /// <param name="_counter"></param>
    public void Work(Counter _counter)
    {
        StartCoroutine(WorkRoutine(_counter));
    }

    /// <summary>
    /// Routine de travail principale de l'agent.
    /// </summary>
    /// <param name="_counter"></param>
    /// <returns></returns>
    private IEnumerator WorkRoutine(Counter _counter)
    {
        while (true)
        {
            //Trouver le prochain ingrédient disponible
            Ingredient nextIngredient = m_kitchenManager.GetNextAvailableIngredient();

            //Identifier la commande associée
            Order currentOrder = nextIngredient.GetOrder();

            //Si la commande n’a pas encore d’assiette, aller en chercher une (une fois)
            if (currentOrder.GetPlate() == null && !currentOrder.IsPlateBeingAssigned())
            {
                currentOrder.SetPlateBeingAssigned(true);
                yield return StartCoroutine(FetchAndAssignPlate(currentOrder));
                currentOrder.SetPlateBeingAssigned(false);
            }

            //Maintenant que la commande a son assiette, l’agent peut bosser dessus
            yield return StartCoroutine(FetchIngredientRoutine(nextIngredient));

            //Vérifier si l’agent transporte maintenant un plat terminé
            if (m_agentMain is Dish dish)
            {
                Order dishOrder = dish.GetOrder();

                if (dishOrder != null && !dishOrder.IsBeingDelivered())
                {
                    dishOrder.SetBeingDelivered(true);
                    yield return StartCoroutine(DeliverOrderRoutine(_counter, dishOrder));
                    dishOrder.SetBeingDelivered(false);

                    // Une fois livré, retirer la commande
                    m_kitchenManager.RemoveOrder(dishOrder);
                }
            }

            //Attendre un peu avant la prochaine boucle
            yield return null;
        }
    }


    /// <summary>
    /// Méthode pour récupérer une assiette et l'assigner à une commande.
    /// </summary>
    /// <param name="_order"></param>
    /// <returns></returns>
    private IEnumerator FetchAndAssignPlate(Order _order)
    {
        // Si l'agent a une assiette sale dans les mains, il va la laver
        if (m_agentMain is Plate plate && !plate.IsClean())
        {
            // Attendre qu'une station de lavage soit libre
            WashStation nearestWash = null;
            yield return new WaitUntil(() => (nearestWash = FindNearestAvailableStation(m_washStation)) != null);

            nearestWash.LockStation();

            MoveTo(nearestWash.transform.position);
            yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestWash.transform.position) < 1.5f);

            m_agentMain = null;
            ShowObjectInHand();
            Debug.Log(m_agentID + " is washing the plate.");
            yield return StartCoroutine(nearestWash.WashPlate(plate));
            m_agentMain = nearestWash.GetObject(); // Récupère l'assiette propre
            ShowObjectInHand();
        }

        // Si l'agent n'a pas d'assiette, il va en chercher une propre
        else if (m_agentMain == null)
        {
            PlateStation nearestPlateStation = null;
            yield return new WaitUntil(() => (nearestPlateStation = FindNearestPlateStation(true)) != null);

            MoveTo(nearestPlateStation.transform.position);
            yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestPlateStation.transform.position) < 1.5f);

            // Attendre qu'une assiette propre soit disponible (la boucle est interne au WaitUntil)
            Plate cleanPlate = nearestPlateStation.GetPlate();

            m_agentMain = cleanPlate;
            ShowObjectInHand();
        }

        // L'agent assigne l'assiette propre à la commande et la dépose sur la table la plus proche
        if (m_agentMain is Plate newPlate)
        {
            Debug.Log(m_agentID + " picked up a clean plate.");
            var nearestTable = FindNearestAvailableStation(m_tableStation);
            if (nearestTable != null)
            {
                nearestTable.LockStation();

                MoveTo(nearestTable.transform.position);
                yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestTable.transform.position) < 1.5f);
                nearestTable.AssignPlateToOrder(newPlate, _order);
                Debug.Log(m_agentID + " assigned plate to order " + _order.GetOrderId());
                m_agentMain = null;
                ShowObjectInHand();
            }
        }
    }



    /// <summary>
    /// Méthode pour récupérer un ingrédient et le déposer sur l'assiette assignée à sa commande.
    /// </summary>
    /// <param name="_ingredient"></param>
    /// <returns></returns>
    private IEnumerator FetchIngredientRoutine(Ingredient _ingredient)
    {
        if (_ingredient == null)
            yield break;

        Order ingredientOrder = _ingredient.GetOrder();
        if (ingredientOrder == null)
        {
            Debug.LogWarning("Ingredient has no associated order: " + _ingredient.GetName());
            yield break;
        }

        WorkStation container = _ingredient.GetContainer();
        if (container == null)
            yield break;

        MoveTo(container.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, container.transform.position) < 1.5f);

        if (container is IngredientStation station)
            m_agentMain = station.GetIngredient();
        else if (container is CookingStation cooking)
            m_agentMain = cooking.GetObject();

        ShowObjectInHand();

        if (m_agentMain is Ingredient ingredient)
        {
            Debug.Log(m_agentID + " picked up: " + ingredient.GetName() + " appartient a la commande " + ingredient.GetOrder().GetOrderId());

            // Vérifier si l’ingrédient doit être cuit
            if (ingredient.NeedsCooking())
            {
                yield return new WaitUntil(() => FindNearestAvailableStation(m_cookingStation) != null);
                var nearestCooking = FindNearestAvailableStation(m_cookingStation);
                if (nearestCooking == null) yield break;

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

            // Vérifier si l’ingrédient doit être coupé
            if (ingredient.NeedsCutting())
            {
                yield return new WaitUntil(() => FindNearestAvailableStation(m_cuttingStation) != null);
                var nearestCutting = FindNearestAvailableStation(m_cuttingStation);
                if (nearestCutting == null) yield break;

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

        // Vérification et assignation de l’assiette à la commande
        var tableStation = ingredientOrder.GetTableStation();
        var plate = tableStation?.GetPlate();

        if (plate == null)
        {
            // Si pas d'assiette assignée, en chercher une
            yield return StartCoroutine(FetchAndAssignPlate(ingredientOrder));
            tableStation = ingredientOrder.GetTableStation();
            plate = tableStation?.GetPlate();
        }

        if (tableStation == null || plate == null)
        {
            Debug.LogWarning("No table or plate available for ingredient order: " + ingredientOrder.GetOrderId());
            yield break;
        }

        // Se déplacer vers la table
        MoveTo(tableStation.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, tableStation.transform.position) < 1.5f);

        // Déposer l’ingrédient sur l’assiette
        if (m_agentMain is Ingredient ingredientToPlace)
        {
            plate.AddIngredient(ingredientToPlace);
            m_agentMain = null;
            ShowObjectInHand();
            Debug.Log(m_agentID + " placed: " + ingredientToPlace.GetName() + " on plate");
            tableStation.UpdatePlateVisual();
        }

        // Assembler le plat si possible
        if (plate.CanAssembleDish(ingredientOrder))
        {
            Debug.Log(m_agentID + " is assembling the dish...");
            yield return StartCoroutine(plate.AssembleDish(ingredientOrder));
            Dish plat = plate.GetPreparedDish();
            plat.SetOrder(ingredientOrder);
            Debug.Log(m_agentID + " assembled the dish: " + (plat != null ? plat.GetName() : "Failed to assemble"));
            m_agentMain = plat;
            ShowObjectInHand();
            tableStation.RemovePlate();
        }
    }


    /// <summary>
    /// Méthode pour livrer un plat au comptoir et nettoyer la commande.
    /// </summary>
    /// <param name="_counter"></param>
    /// <param name="_order">La commande associée au plat à livrer.</param>
    /// <returns></returns>
    private IEnumerator DeliverOrderRoutine(Counter _counter, Order _order)
    {
        if (_order == null)
        {
            Debug.LogWarning("DeliverOrderRoutine called with null order.");
            yield break;
        }

        if (!(m_agentMain is Dish dish))
        {
            Debug.LogWarning("Agent is not carrying a Dish to deliver.");
            yield break;
        }

        // Déplacer l’agent vers le comptoir
        MoveTo(_counter.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 1.5f);

        // Livrer la commande
        _counter.ReceiveOrder(_order, dish);
        m_agentMain = null;
        ShowObjectInHand();
        Debug.Log(m_agentID + " delivered order: " + _order.GetOrderId());

        // Créer une nouvelle assiette sale et la donner à l'agent
        Plate dirtyPlate = new Plate(m_kitchenManager.m_cleanPlatePrefab, m_kitchenManager.m_dirtyPlatePrefab);
        dirtyPlate.SetPrefab(m_kitchenManager.m_dirtyPlatePrefab);
        dirtyPlate.SetState(PlateState.Dirty);
        m_agentMain = dirtyPlate;
        ShowObjectInHand();
        Debug.Log(m_agentID + " picked up new dirty plate.");

        // Enlever l'assiette de la table
        var tableStation = _order.GetTableStation();
        tableStation?.RemovePlate();
        tableStation?.UnlockStation();

        // 1. Trouver une station de lavage
        WashStation nearestWash = null;
        yield return new WaitUntil(() => (nearestWash = FindNearestAvailableStation(m_washStation)) != null);

        nearestWash.LockStation();

        MoveTo(nearestWash.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestWash.transform.position) < 1.5f);
        m_agentMain = null;
        ShowObjectInHand();
        Debug.Log(m_agentID + " is washing the plate.");
        yield return StartCoroutine(nearestWash.WashPlate(dirtyPlate));
        m_agentMain = nearestWash.GetObject(); // Récupère l'assiette propre
        ShowObjectInHand();

        nearestWash.UnlockStation(); // WashPlate le fait déjà, mais par sécurité

        // 2. Trouver une station d'assiettes (n'importe laquelle) pour la déposer
        PlateStation nearestPlateStation = FindNearestPlateStation(false); // false = pas besoin qu'elle ait des assiettes

        MoveTo(nearestPlateStation.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, nearestPlateStation.transform.position) < 1.5f);
        nearestPlateStation.SetPlate(m_agentMain as Plate);
        m_agentMain = null;
        ShowObjectInHand();

        // Retirer la commande du KitchenManager
        m_kitchenManager.RemoveOrder(_order);
    }


    /// <summary>
    /// Méthode pour trouver la station de travail disponible la plus proche.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stations"></param>
    /// <returns></returns>
    private T FindNearestAvailableStation<T>(List<T> stations) where T : WorkStation
    {
        T nearest = null;
        float minDist = float.MaxValue;
        foreach (var station in stations)
        {
            if (station is CuttingStation cut && cut.IsBusy ) continue;
            if (station is CookingStation cook && (cook.HasCookedIngredient() || cook.IsBusy)) continue;
            if (station is TableStation table && table.IsBusy) continue;
            if (station is WashStation wash && wash.IsBusy) continue;

            float dist = Vector3.Distance(transform.position, station.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = station;
            }
        }
        return nearest;
    }


    /// <summary>
    /// Trouve la PlateStation la plus proche.
    /// Si mustHavePlates est true, ignore les stations vides.
    /// </summary>
    private PlateStation FindNearestPlateStation(bool mustHavePlates = false)
    {
        PlateStation nearest = null;
        float minDist = float.MaxValue;
        foreach (var station in m_plateStation)
        {
            // Si on cherche une assiette et que la station est vide, on l'ignore
            if (mustHavePlates && !station.HasPlates())
                continue;

            float dist = Vector3.Distance(transform.position, station.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = station;
            }
        }
        return nearest;
    }



    /// <summary>
    /// Affiche l'objet que l'agent tient dans sa main.
    /// </summary>
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
