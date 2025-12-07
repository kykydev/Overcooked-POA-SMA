using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    /// --- Attributes ---
    [Header("IngredientPrefabs")]
    public GameObject m_steakPrefab;
    public GameObject m_tomatoPrefab;
    public GameObject m_saladPrefab;
    public GameObject m_breadPrefab;
    public GameObject m_onionPrefab;
    public GameObject m_chickenPrefab;
    public GameObject m_sausagePrefab;

    [Header("DishPrefabs")]
    public GameObject m_dishBurgerPrefab;
    public GameObject m_dishSaladPrefab;
    public GameObject m_dishSteakPrefab;
    public GameObject m_dishChickenSandwichPrefab;
    public GameObject m_dishHotdogPrefab;

    private List<Dish> m_allDishes;

    /// --- Methods ---

    /// <summary>
    /// Construit la liste de tous les plats disponibles dans le jeu.
    /// </summary>
    void Awake()
    {
        m_allDishes = new List<Dish>
        {
            new Dish("Burger", new List<Ingredient> {
                new Ingredient("Steak", true, false, m_steakPrefab),
                new Ingredient("Tomato", false, true, m_tomatoPrefab),
                new Ingredient("Salad", false, true, m_saladPrefab),
                new Ingredient("Bread", false, false, m_breadPrefab),
                new Ingredient("Onion", false, true, m_onionPrefab),
            },
            m_dishBurgerPrefab
            ),
             new Dish("Salad", new List<Ingredient> {
                 new Ingredient("Tomato", false, true, m_tomatoPrefab),
                 new Ingredient("Salad", false, true, m_saladPrefab),
                 new Ingredient("Onion", false, true, m_onionPrefab)

             },
             m_dishSaladPrefab
             ),
             new Dish("ChickenSandwich", new List<Ingredient> {
                 new Ingredient("Chicken", true, false, m_chickenPrefab),
                 new Ingredient("Onion", false, true, m_onionPrefab),
                 new Ingredient("Salad", false, true, m_saladPrefab),
                 new Ingredient("Bread", false, false, m_breadPrefab)
             },
             m_dishChickenSandwichPrefab
             ),
             new Dish("Hotdog", new List<Ingredient> {
                 new Ingredient("Sausage", true, false, m_sausagePrefab),
                 new Ingredient("Bread", false, false, m_breadPrefab)
             },
             m_dishHotdogPrefab
             )
             //new Dish("VeganBurger", new List<Ingredient> {
             //    new Ingredient("Tomato", false, true, m_tomatoPrefab),
             //    new Ingredient("Salad", false, true, m_saladPrefab),
             //    new Ingredient("Bread", false, false, m_breadPrefab),
             //    new Ingredient("Onion", false, true, m_onionPrefab)
             //},
             //m_dishBurgerPrefab
             //),
             //new Dish("Steak", new List<Ingredient> {
             //    new Ingredient("Steak", true, false, m_steakPrefab)
             //},
             //m_dishSteakPrefab
             //)
             //new Dish ("DoubleBurger", new List<Ingredient> {
             //    new Ingredient("Steak", true, false, m_steakPrefab),
             //    new Ingredient("Steak", true, false, m_steakPrefab),
             //    new Ingredient("Tomato", false, true, m_tomatoPrefab),
             //    new Ingredient("Salad", false, true, m_saladPrefab),
             //    new Ingredient("Bread", false, false, m_breadPrefab),
             //    new Ingredient("Onion", false, true, m_onionPrefab),
             //},
             //m_dishBurgerPrefab
             //)
        };
    }


    /// <summary>
    /// Retourne une liste de plats aléatoires.
    /// </summary>
    /// <param name="_count"></param>
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

