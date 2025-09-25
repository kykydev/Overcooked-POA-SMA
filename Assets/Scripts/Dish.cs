using UnityEngine;
using System.Collections.Generic;
using System;

public class Dish
{

    /// --- Attributes ---
    public string m_name;
    public List<Ingredient> m_recipe;

    /// --- Constructor ---
    public Dish(string _name, List<Ingredient> _recipe)
    {
        m_name = _name;
        m_recipe = _recipe;
    }

    /// --- Getters ---
    public string GetName() => m_name;
    public List<Ingredient> GetRecipe() => m_recipe;

    /// --- Setters ---
    public void SetName(string _name) => m_name = _name;
    public void SetRecipe(List<Ingredient> _recipe) => m_recipe = _recipe;
}
