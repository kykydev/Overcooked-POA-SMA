using System.Collections;
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
    private Ingredient m_agentMain;

    private AssemblyStation m_assemblyStation;
    private CookingStation m_cookingStation;
    private CuttingStation m_cuttingStation;

    [SerializeField] private NavMeshAgent m_navAgent;
    [SerializeField] private KitchenManager m_kitchenManager;
    
    /// ---- Getters ----
    public int GetAgentID() => m_agentID;
    public string GetAgentName() => m_agentName;
    public Ingredient GetAgentMain() => m_agentMain;

    /// ---- Setters ----
    public void SetAgentID(int _id) => m_agentID = _id;
    public void SetAgentName(string _name) => m_agentName = _name;
    public void SetAgentMain(Ingredient _main) => m_agentMain = _main;
    public void SetAssemblyStation(AssemblyStation _assemblyStation) => m_assemblyStation = _assemblyStation;
    public void SetCookingStation(CookingStation _cookingStation) => m_cookingStation = _cookingStation;
    public void SetCuttingStation(CuttingStation _cuttingStation) => m_cuttingStation = _cuttingStation;


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

                //Déposer la commande si tous les ingrédients sont sur la workstation d'assemblage
                if (m_assemblyStation.ValidateOrder(m_kitchenManager.GetCurrentOrder()))
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
            Debug.Log(m_agentID + " took order " + m_kitchenManager.GetCurrentOrder().GetOrderId());
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
        {
            m_agentMain = station.GetIngredient();
            ShowIngredientInHand();
        }

        if (container is CookingStation cooking)
        {
            m_agentMain = cooking.GetIngredient();
            ShowIngredientInHand();

        }

        Debug.Log(m_agentID + " picked up: " + m_agentMain.GetName());

        //Si l’ingrédient nécessite une cuisson, se déplacer vers la workstation de cuisson pour y déposer l’ingrédient
        if (m_agentMain.NeedsCooking())
        {
            MoveTo(m_cookingStation.transform.position);
            yield return new WaitUntil(() => Vector3.Distance(transform.position, m_cookingStation.transform.position) < 2f);

            m_cookingStation.ShowIngredientOnStation(m_agentMain); // <-- Ajoute cette ligne
            m_cookingStation.StartCoroutine(m_cookingStation.CookIngredient(m_agentMain));
            
            m_agentMain = null;
            ShowIngredientInHand();
            MoveTo(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f))); //Je bouge l'agent pour pas tout casser
            yield break;
        }

        //Si l’ingrédient nécessite une découpe, se déplacer vers la workstation de découpe pour y déposer l’ingrédient
        if (m_agentMain.NeedsCutting())
        {
            MoveTo(m_cuttingStation.transform.position);
            yield return new WaitUntil(() => Vector3.Distance(transform.position, m_cuttingStation.transform.position) < 2f);
            yield return StartCoroutine(m_cuttingStation.CutIngredient(m_agentMain));
            MoveTo(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f))); //Je bouge l'agent pour pas tout casser
        }


        //Se déplacer vers la workstation d’assemblage
        MoveTo(m_assemblyStation.transform.position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, m_assemblyStation.transform.position) < 2f);

        //Déposer l’ingrédient sur la workstation d’assemblage
        m_assemblyStation.AddIngredient(m_agentMain);
        Debug.Log(m_agentID + " placed: " + m_agentMain.GetName() + " on assemblyStation");
        MoveTo(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f))); //Je bouge l'agent pour pas tout casser


        m_agentMain = null;
        ShowIngredientInHand();
    }

    private IEnumerator DeliverOrderRoutine(Counter _counter)
    {
        Order currentOrder = m_kitchenManager.GetCurrentOrder();
        if (currentOrder == null)
            yield break;

        //Déplacer l’agent vers le comptoir
        MoveTo(_counter.transform.position);

        //Attendre que l’agent arrive à proximité
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _counter.transform.position) < 2f);

        //Livrer la commande
        _counter.ReceiveOrder(currentOrder, m_assemblyStation);
        Debug.Log(m_agentID + " delivered order: " + currentOrder.GetOrderId());

        //Nettoyer la commande dans le KitchenManager
        m_assemblyStation.ClearStation();
        m_kitchenManager.ClearCurrentOrder();
    }

// ...existing code...

    public void ShowIngredientInHand()
    {
        Transform handTransform = transform.Find("Hand");
        if (handTransform == null)
            return;

        // Détruit tout ce qu'il y a dans la main
        foreach (Transform child in handTransform)
            GameObject.Destroy(child.gameObject);

        // Si l'agent a un ingrédient, affiche-le
        if (m_agentMain != null && m_agentMain.GetPrefab() != null)
        {
            GameObject ingredientObj = GameObject.Instantiate(m_agentMain.GetPrefab(), handTransform);
            ingredientObj.transform.localPosition = Vector3.zero;
            ingredientObj.transform.localRotation = Quaternion.identity;
        }
    }

// ...appelle ShowIngredientInHand() juste après avoir récupéré un ingrédient dans FetchIngredientRoutine...
}
