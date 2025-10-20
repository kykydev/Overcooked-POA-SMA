using System.Collections;
using UnityEngine;

public class CuttingStation : WorkStation
{
    /// --- Methods ---
    public IEnumerator CutIngredient(Ingredient _ingredient)
    {
        if (_ingredient == null)
            yield break;

        SetObject(_ingredient);
        ShowObjectOnStation(_ingredient);

        yield return new WaitForSeconds(3f);

        _ingredient.SetCuttingIngredientState(IngredientCuttingState.Cut);
        Debug.Log($"{_ingredient.GetName()} has been cut and is ready!");
    }

    public bool HasCutIngredient()
    {
        object current = PeekObject();
        return current is Ingredient ingredient &&
               ingredient.GetCuttingState() == IngredientCuttingState.Cut;
    }
}
