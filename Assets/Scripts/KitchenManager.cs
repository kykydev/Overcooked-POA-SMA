using System.Collections.Generic;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    /// --- Attributes ---
    [Header("Scene References")]
    [SerializeField] private List<Agent> m_agents;

    [SerializeField] private Counter m_counter;

    [SerializeField] private IngredientStation m_tomatoContainer;
    [SerializeField] private IngredientStation m_saladContainer;
    [SerializeField] private IngredientStation m_breadContainer;
    [SerializeField] private IngredientStation m_steakContainer;
    [SerializeField] private IngredientStation m_onionContainer;

    [SerializeField] private AssemblyStation m_assemblyStation;
    [SerializeField] private CookingStation m_cookingStation;
    [SerializeField] private CuttingStation m_cuttingStation;

    [SerializeField] private DishManager m_dishManager;

    private Order m_currentOrder;
    private bool m_isOrderBeingTaken = false;
    private bool m_isOrderBeingDelivered = false;

    private int m_money;

    private Queue<Ingredient> m_ingredientQueue;

    /// --- Order State Management ---
    public bool IsOrderBeingTaken() => m_isOrderBeingTaken;
    public void SetOrderBeingTaken(bool value) => m_isOrderBeingTaken = value;
    public bool IsOrderBeingDelivered() => m_isOrderBeingDelivered;
    public void SetOrderBeingDelivered(bool value) => m_isOrderBeingDelivered = value;

    /// --- Getters ---
    public Order GetCurrentOrder() => m_currentOrder;
    public Queue<Ingredient> GetIngredientQueue() => m_ingredientQueue;

    public Ingredient GetCurrentIngredient()
    {
        if (m_ingredientQueue == null || m_ingredientQueue.Count == 0)
            return null;

        return m_ingredientQueue.Dequeue();
    }

    /// --- Setters ---
    public void SetCurrentOrder(Order _order)
    {
        if (m_currentOrder == null)
        {
            m_currentOrder = _order;
            var ingredientQueue = new Queue<Ingredient>();
            foreach (Ingredient proto in _order.GetDish().GetRecipe())
            {
                // Clone l'ingrédient pour cette commande
                Ingredient ing = new Ingredient(proto.GetName(), proto.GetNeedsCooking(), proto.GetNeedsCutting(), proto.GetPrefab());

                if (proto.GetName() == "Tomato")
                {
                    ing.SetContainer(m_tomatoContainer);
                    m_tomatoContainer.SetIngredient(ing);
                }
                else if (proto.GetName() == "Salad")
                {
                    ing.SetContainer(m_saladContainer);
                    m_saladContainer.SetIngredient(ing);
                }
                else if (proto.GetName() == "Bread")
                {
                    ing.SetContainer(m_breadContainer);
                    m_breadContainer.SetIngredient(ing);
                }
                else if (proto.GetName() == "Steak")
                {
                    ing.SetContainer(m_steakContainer);
                    m_steakContainer.SetIngredient(ing);
                }
                else if (proto.GetName() == "Onion")
                {
                    ing.SetContainer(m_onionContainer);
                    m_onionContainer.SetIngredient(ing);
                }

                ingredientQueue.Enqueue(ing);
            }
            m_ingredientQueue = ingredientQueue;

        }
        else
        {
            Debug.LogWarning("Overwriting Current Order: " + m_currentOrder.GetOrderId() + " with " + _order.GetOrderId());
        }
    }

    /// --- Methods ---
    void Start()
    {

        //Récupérer 10 plats aléatoires
        List<Dish> randomDishes = m_dishManager.GetRandomDishes(10);

        //Créer les commandes
        int i = 0;
        foreach (Dish dish in randomDishes)
        {
            string orderId = dish.GetName() + "_" + i;
            i++;
            Order order = new Order(orderId, dish, 15);
            m_counter.AddOrder(order);
        }

        //Lancer les agents
        int j = 0;
        foreach (Agent agent in m_agents)
        {
            agent.SetAgentID(j);
            agent.SetAssemblyStation(m_assemblyStation);
            agent.SetCookingStation(m_cookingStation);
            agent.SetCuttingStation(m_cuttingStation);
            agent.Work(m_counter);
            j++;
        }
    }

    //Verifie si une commande est en cours
    public bool HasCurrentOrder()
    {
        return m_currentOrder != null && m_currentOrder.GetStatus() != OrderStatus.Delivered;
    }

    //Vide la commande en cours
    public void ClearCurrentOrder()
    {
        m_currentOrder = null;
        m_ingredientQueue = new Queue<Ingredient>();
    }


    //Ajoute de l'argent au total
    public void AddMoney(int amount)
    {
        m_money += amount;
        Debug.Log("Total Money: " + m_money);
    }

}
