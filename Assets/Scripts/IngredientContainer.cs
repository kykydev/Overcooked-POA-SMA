using UnityEngine;

public class IngredientStation : WorkStation
{
    /// --- Attributes ---
    private Ingredient m_ingredient;

    /// --- Getters ---
    public Ingredient GetIngredient() => m_ingredient;

    /// --- Setters ---
    public void SetIngredient(Ingredient _ingredient){
        m_ingredient = _ingredient;
        m_ingredient.SetContainer(this);
    }
}
