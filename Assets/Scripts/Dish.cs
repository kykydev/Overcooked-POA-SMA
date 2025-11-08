using UnityEngine;
using System.Collections.Generic;

public class Dish
{
    /// --- Attributes ---
    public string m_name;
    public List<Ingredient> m_recipe;
    private GameObject m_prefab;
    private Order m_order;

    /// --- Constructor ---
    public Dish(string _name, List<Ingredient> _recipe, GameObject _prefab)
    {
        m_name = _name;
        m_recipe = _recipe;
        m_prefab = _prefab;
    }

    /// --- Getters ---
    public string GetName() => m_name;
    public List<Ingredient> GetRecipe() => m_recipe;
    public GameObject GetPrefab() => m_prefab;
    public Order GetOrder() => m_order;

    /// --- Setters ---
    public void SetName(string _name) => m_name = _name;
    public void SetRecipe(List<Ingredient> _recipe) => m_recipe = _recipe;
    public void SetOrder(Order _order) => m_order = _order;
}
