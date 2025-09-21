using System.Collections.Generic;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Agent m_agent1;
    [SerializeField] private Counter m_counter;
    [SerializeField] private IngredientContainer m_tomatoContainer;
    [SerializeField] private IngredientContainer m_saladContainer;

    void Start()
    {
        Ingredient tomato = new Ingredient("Tomato");
        Ingredient salad = new Ingredient("Salad");

        m_tomatoContainer.SetIngredient(tomato);
        m_saladContainer.SetIngredient(salad);

        tomato.SetContainer(m_tomatoContainer);
        salad.SetContainer(m_saladContainer);


        List<Order> orders = new List<Order>();

        for (int i = 0; i < 10; i++)
        {
            Order burger = new Order();
            burger.SetOrderId("Burger_" + i);
            burger.SetRecipe(new List<Ingredient> { tomato, salad });
            burger.SetReward(10);

            m_counter.AddOrder(burger);
        }

        m_agent1.Work(m_counter);
    }
}
