using System.Collections.Generic;
using UnityEngine;

public class IngredientStation : WorkStation
{
    private Queue<Ingredient> m_ingredients = new Queue<Ingredient>();

    public Ingredient GetIngredient()
    {
        if (m_ingredients.Count == 0)
            return null;
        return m_ingredients.Dequeue();
    }

    public void SetIngredient(Ingredient _ingredient)
    {
        m_ingredients.Enqueue(_ingredient);
        _ingredient.SetContainer(this);
    }
}
