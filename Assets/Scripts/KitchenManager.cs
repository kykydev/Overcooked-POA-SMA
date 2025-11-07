using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    /// --- Attributes ---
    [Header("Scene References")]
    [SerializeField] private List<Agent> m_agents;
    [SerializeField] private Counter m_counter;
    [SerializeField] private DishManager m_dishManager;

    [Header("Workstations")]
    [SerializeField] private IngredientStation m_tomatoContainer;
    [SerializeField] private IngredientStation m_saladContainer;
    [SerializeField] private IngredientStation m_breadContainer;
    [SerializeField] private IngredientStation m_steakContainer;
    [SerializeField] private IngredientStation m_onionContainer;

    [SerializeField] private List<CookingStation> m_cookingStations;
    [SerializeField] private List<CuttingStation> m_cuttingStations;
    [SerializeField] private List<TableStation> m_tableStations;
    [SerializeField] private List<PlateStation> m_plateStations;
    [SerializeField] private List<WashStation> m_washStations;

    [Header("Prefabs")]
    public GameObject m_cleanPlatePrefab;
    public GameObject m_dirtyPlatePrefab;
    public GameObject m_steakCuitPrefab;

    /// --- Current Order Management ---
    private List<Order> m_currentOrders = new List<Order>();

    /// --- Getters ---
    public List<Order> GetCurrentOrders() => m_currentOrders;

    /// <summary>
    /// Retourne le prochain ingrédient disponible dans la liste des commandes en cours. Le parcours se fait dans l'ordre des commandes.
    /// </summary>
    /// <returns></returns>
    public Ingredient GetNextAvailableIngredient()
    {
        foreach (var order in m_currentOrders)
        {
            if (order.GetIngredientQueue().Count == 0) continue;

            return order.GetIngredientQueue().Dequeue();
        }
        return null;
    }

    /// <summary>
    /// Ajoute une commande à la liste des commandes en cours et initialise les ingrédients nécessaires en les affectant aux stations appropriées. 
    /// Ensuite, remplit la Queue d'ingrédients, celle qui sera utilisée par les agents pour récupérer les ingrédients.
    /// </summary>
    /// <param name="_order"></param>    
    public void AddOrder(Order _order)
    
    {
        m_currentOrders.Add(_order);

        Queue<Ingredient> ingredientQueue = new Queue<Ingredient>();

        foreach (Ingredient proto in _order.GetDish().GetRecipe())
        {
            Ingredient ing = new Ingredient(proto.GetName(), proto.GetNeedsCooking(), proto.GetNeedsCutting(), proto.GetPrefab());
            ing.SetOrder(_order);

            switch (proto.GetName())
            {
                case "Tomato":
                    ing.SetContainer(m_tomatoContainer);
                    m_tomatoContainer.SetIngredient(ing);
                    break;
                case "Salad":
                    ing.SetContainer(m_saladContainer);
                    m_saladContainer.SetIngredient(ing);
                    break;
                case "Bread":
                    ing.SetContainer(m_breadContainer);
                    m_breadContainer.SetIngredient(ing);
                    break;
                case "Steak":
                    ing.SetContainer(m_steakContainer);
                    m_steakContainer.SetIngredient(ing);
                    break;
                case "Onion":
                    ing.SetContainer(m_onionContainer);
                    m_onionContainer.SetIngredient(ing);
                    break;
            }

            ingredientQueue.Enqueue(ing);
        }

        _order.GetIngredientQueue().Clear();
        foreach (var ing in ingredientQueue)
            _order.GetIngredientQueue().Enqueue(ing);
    }


    /// <summary>
    /// Initialise les commandes, la pile d'assiettes et assigne les stations aux agents.
    /// </summary>
    /// <returns></returns>
    void Start()
    {
        InitializeOrders();
        foreach (var plateStation in m_plateStations)
        {
            plateStation.InitializePlateStack(5, m_cleanPlatePrefab, m_dirtyPlatePrefab);
        }

        StartCoroutine(StartAgentsNextFrame());
    }

    void InitializeOrders()
    {
        int n = 10;
        List<Dish> randomDishes = m_dishManager.GetRandomDishes(n);

        int i = 0;
        foreach (Dish dish in randomDishes)
        {
            string orderId = dish.GetName() + "_" + i;
            i++;
            Order order = new Order(orderId, dish, 15);
            AddOrder(order);
        }

        Debug.Log("Initialized " + n + " orders.");
        foreach (Order order in m_currentOrders)
            Debug.Log("Order ID: " + order.GetOrderId());
    }

    IEnumerator StartAgentsNextFrame()
    {
        yield return new WaitUntil(() => m_plateStations.Count > 0 && m_plateStations[0].HasPlates());

        int j = 0;
        foreach (Agent agent in m_agents)
        {
            agent.SetAgentID(j);
            agent.SetKitchenManager(this);
            agent.SetCookingStation(m_cookingStations);
            agent.SetCuttingStation(m_cuttingStations);
            agent.SetTableStation(m_tableStations);
            agent.SetPlateStation(m_plateStations);
            agent.SetWashStation(m_washStations);
            agent.Work(m_counter);
            j++;
        }
    }


    /// <summary>
    /// Vérifie s'il y a des commandes en cours.
    /// </summary>
    /// <returns></returns>
    public bool HasCurrentOrders()
    {
        return m_currentOrders.Count > 0;
    }

    /// <summary>
    /// Retire une commande de la liste des commandes en cours.
    /// </summary>
    /// <param name="order"></param>
    public void RemoveOrder(Order _order)
    {
        if (m_currentOrders.Contains(_order))
            m_currentOrders.Remove(_order);
    }

}
