using System.Collections;
using UnityEngine;

public class CookingStation : WorkStation
{

    /// --- Attributes ---
    [SerializeField] private KitchenManager m_kitchenManager;
    [SerializeField] private Ingredient m_cookedIngredient;

    /// --- Getters ---
    public Ingredient GetIngredient()
    {
        var tmp = m_cookedIngredient;
        m_cookedIngredient = null;
        return tmp;
    }

    /// --- Methods ---
    public IEnumerator CookIngredient(Ingredient _ingredient)
    {
        m_placedIngredients.Add(_ingredient);
        Debug.Log($"Cooking {_ingredient.GetName()}...");
        yield return new WaitForSeconds(5f);
        _ingredient.SetCookingIngredientState(IngredientCookingState.Cooked);
        Debug.Log($"{_ingredient.GetName()} is cooked!");

         m_cookedIngredient = _ingredient;
        _ingredient.SetContainer(this);
        m_placedIngredients.Remove(_ingredient);

        m_kitchenManager.GetIngredientQueue().Enqueue(_ingredient);
    }

}
