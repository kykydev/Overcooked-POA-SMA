using UnityEngine;

public class Ingredient
{
    /// --- Attributes ---
    private string m_ingredientName;
    private IngredientContainer m_container;

    /// --- Constructor ---
    public Ingredient(string _name)
    {
        m_ingredientName = _name;
    }

    /// --- Getters ---
    public IngredientContainer GetContainer() => m_container;
    public string GetName() => m_ingredientName;

    /// --- Setters ---
    public void SetContainer(IngredientContainer _container) => m_container = _container;
    public void SetIngredientName(string _name) => m_ingredientName = _name;
}
