using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    private List<Dish> m_allDishes;

    void Awake()
    {
        m_allDishes = new List<Dish>
        {
            new Dish("Burger", new List<Ingredient> {
                new Ingredient("Steak", true, true),
                new Ingredient("Tomato", false, true),
                new Ingredient("Salad", false, true),
                new Ingredient("Bread", false, false),
                new Ingredient("Onion", false, true)

            }),
            // new Dish("Salad", new List<Ingredient> {
            //     new Ingredient("Tomato", false, true),
            //     new Ingredient("Salad", false, true),
            //     new Ingredient("Onion", false, true)

            // }),
            // new Dish("VeganBurger", new List<Ingredient> {
            //     new Ingredient("Tomato", false, true),
            //     new Ingredient("Salad", false, true),
            //     new Ingredient("Bread", false, false),
            //     new Ingredient("Onion", false, true)
            // }),
            // new Dish("Steak", new List<Ingredient> {
            //     new Ingredient("Steak", true, true)
            // })
        };
    }

    public List<Dish> GetRandomDishes(int _count)
    {
        List<Dish> result = new List<Dish>();
        for (int i = 0; i < _count; i++)
        {
            int rand = Random.Range(0, m_allDishes.Count);
            result.Add(m_allDishes[rand]);
        }
        return result;
    }
}

