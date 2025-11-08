using System.Collections.Generic;
using UnityEngine;

public class IngredientStation : WorkStation
{
    /// --- Attributes ---
    private Queue<Ingredient> m_ingredients = new Queue<Ingredient>();

    /// --- Methods ---

    /// <summary>
    /// Dépile l'ingrédient présent dans la station.
    /// </summary>
    public Ingredient GetIngredient()
    {
        if (m_ingredients.Count == 0)
            return null;
        return m_ingredients.Dequeue();
    }


    /// <summary>
    /// Ajoute un ingrédient à la station et lui assigne cette station comme contenaire.
    /// </summary>
    /// <param name="_ingredient"></param>
    public void SetIngredient(Ingredient _ingredient)
    {
        m_ingredients.Enqueue(_ingredient);
        _ingredient.SetContainer(this);
    }
}
