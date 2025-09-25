using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    /// --- Attributes ---
    private List<Dish> m_allDishes;

    [SerializeField] private IngredientManager m_ingredientManager;

    /// --- Methods ---
    void Awake()
    {
        m_allDishes = new List<Dish>
        {
            new Dish("Burger", new List<Ingredient> { m_ingredientManager.Tomato, m_ingredientManager.Salad, m_ingredientManager.Bread, m_ingredientManager.Steak }),
            new Dish("Salad", new List<Ingredient> { m_ingredientManager.Tomato, m_ingredientManager.Salad }),
            new Dish("VeganBurger", new List<Ingredient> { m_ingredientManager.Tomato, m_ingredientManager.Salad, m_ingredientManager.Bread })
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
