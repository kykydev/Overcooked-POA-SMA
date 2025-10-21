using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{

    /// --- Attributes ---
    public GameObject m_steakPrefab;
    public GameObject m_tomatoPrefab;
    public GameObject m_saladPrefab;
    public GameObject m_breadPrefab;
    public GameObject m_onionPrefab;
    
    public GameObject m_dishBurgerPrefab;
    public GameObject m_dishSaladPrefab;

    private List<Dish> m_allDishes;

    /// --- Methods ---
    void Awake()
    {
        m_allDishes = new List<Dish>
        {
            //new Dish("Burger", new List<Ingredient> {
            //    new Ingredient("Steak", true, true, m_steakPrefab),
            //    new Ingredient("Tomato", false, true, m_tomatoPrefab),
            //    new Ingredient("Salad", false, true, m_saladPrefab),
            //    new Ingredient("Bread", false, false, m_breadPrefab),
            //    new Ingredient("Onion", false, true, m_onionPrefab),
            //},
            //m_dishBurgerPrefab
            //),
             //new Dish("Salad", new List<Ingredient> {
             //    new Ingredient("Tomato", false, true, m_tomatoPrefab),
             //    new Ingredient("Salad", false, true, m_saladPrefab),
             //    new Ingredient("Onion", false, true, m_onionPrefab)

             //},
             //m_dishSaladPrefab
             //),
             //new Dish("VeganBurger", new List<Ingredient> {
             //    new Ingredient("Tomato", false, true, m_tomatoPrefab),
             //    new Ingredient("Salad", false, true, m_saladPrefab),
             //    new Ingredient("Bread", false, false, m_breadPrefab),
             //    new Ingredient("Onion", false, true, m_onionPrefab)
             //},
             //m_dishBurgerPrefab
             //),
             //new Dish("Steak", new List<Ingredient> {
             //    new Ingredient("Steak", true, true, m_steakPrefab)
             //},
             //m_dishBurgerPrefab
             //)
             new Dish ("DoubleBurger", new List<Ingredient> {
                 new Ingredient("Steak", true, true, m_steakPrefab),
                 new Ingredient("Steak", true, true, m_steakPrefab),
                 new Ingredient("Tomato", false, true, m_tomatoPrefab),
                 new Ingredient("Salad", false, true, m_saladPrefab),
                 new Ingredient("Bread", false, false, m_breadPrefab),
                 new Ingredient("Onion", false, true, m_onionPrefab),
             },
             m_dishBurgerPrefab
             )
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

