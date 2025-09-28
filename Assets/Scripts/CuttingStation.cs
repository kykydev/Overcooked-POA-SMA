using System.Collections;
using UnityEngine;

public class CuttingStation : WorkStation
{
    public IEnumerator CutIngredient(Ingredient _ingredient)
    {
        m_placedIngredients.Add(_ingredient);
        Debug.Log($"Cutting {_ingredient.GetName()}...");
        yield return new WaitForSeconds(3f);
        _ingredient.SetCuttingIngredientState(IngredientCuttingState.Cut);
        Debug.Log($"{_ingredient.GetName()} is cut!");
        m_placedIngredients.Remove(_ingredient);
    }
}
