using System.Collections.Generic;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Agent m_agent1;
    [SerializeField] private Agent m_agent2;
    [SerializeField] private Agent m_agent3;
    [SerializeField] private Counter m_counter;
    [SerializeField] private IngredientContainer m_tomatoContainer;
    [SerializeField] private IngredientContainer m_saladContainer;
    [SerializeField] private IngredientContainer m_breadContainer;
    [SerializeField] private IngredientContainer m_steakContainer;
    private Order m_currentOrder;

    private bool m_isOrderBeingTaken = false;
    public bool IsOrderBeingTaken() => m_isOrderBeingTaken;
    public void SetOrderBeingTaken(bool value) => m_isOrderBeingTaken = value;


    private bool m_isOrderBeingDelivered = false;
    public bool IsOrderBeingDelivered() => m_isOrderBeingDelivered;
    public void SetOrderBeingDelivered(bool value) => m_isOrderBeingDelivered = value;


    void Start()
    {

        for (int i = 0; i < 10; i++)
        {

            Ingredient tomato = new Ingredient("Tomato");
            Ingredient salad = new Ingredient("Salad");
            Ingredient bread = new Ingredient("Bread");
            Ingredient steak = new Ingredient("Steak");

            m_tomatoContainer.SetIngredient(tomato);
            m_saladContainer.SetIngredient(salad);
            m_breadContainer.SetIngredient(bread);
            m_steakContainer.SetIngredient(steak);

            tomato.SetContainer(m_tomatoContainer);
            salad.SetContainer(m_saladContainer);
            bread.SetContainer(m_breadContainer);
            steak.SetContainer(m_steakContainer);

            Order burger = new Order();
            burger.SetOrderId("Burger_" + i);
            burger.SetRecipe(new List<Ingredient> { tomato, salad, steak, bread});
            burger.SetReward(15);

            m_counter.AddOrder(burger);
        }

        m_agent1.Work(m_counter);
        m_agent2.Work(m_counter);
        m_agent3.Work(m_counter);
    }

    public void SetCurrentOrder(Order _order){ /// Set the current order being prepared
        m_currentOrder = _order; 
    }

    public Order GetCurrentOrder()
    {
        return m_currentOrder; // retourne la commande active ou null si aucune
    }


    public bool HasCurrentOrder(){ /// Check if there is a current order being prepared
        return m_currentOrder != null && m_currentOrder.GetStatus() != OrderStatus.Delivered;
    }

    public void ClearCurrentOrder(){ /// Clear the current order after delivery
        m_currentOrder = null;
    }

    public Ingredient GetCurrentIngredient()
    {
        if (m_currentOrder == null)
            return null;

        foreach (Ingredient ingredient in m_currentOrder.GetRecipe())
        {
            if (!ingredient.IsTaken())
            {
                ingredient.MarkAsTaken();  // marque comme pris
                return ingredient;
            }
        }
        return null; // tous les ingrédients sont pris
    }
}
