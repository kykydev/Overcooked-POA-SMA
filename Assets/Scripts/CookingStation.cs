using System.Collections;
using UnityEngine;

public class CookingStation : WorkStation
{
    /// --- Attributes ---
    [SerializeField] private KitchenManager m_kitchenManager;

    /// --- Methods ---
    public IEnumerator CookIngredient(Ingredient _ingredient)
    {
        if (_ingredient == null)
            yield break;

        SetObject(_ingredient);
        ShowObjectOnStation(_ingredient);

        yield return new WaitForSeconds(5f);

        _ingredient.SetCookingIngredientState(IngredientCookingState.Cooked);
        Debug.Log($"{_ingredient.GetName()} is cooked and ready to get picked up!");

        _ingredient.SetContainer(this);
        m_kitchenManager.GetIngredientQueue().Enqueue(_ingredient);
    }

    public bool HasCookedIngredient()
    {
        object current = PeekObject();
        return current is Ingredient ingredient &&
               ingredient.GetCookingState() == IngredientCookingState.Cooked;
    }
}
