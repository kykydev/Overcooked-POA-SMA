using System.Collections;
using UnityEngine;

public class CuttingStation : WorkStation
{
    /// --- Methods --- 

    /// <summary>
    /// Permet de couper l'ingrédient passé en paramètre après un certain temps et de changer son état.
    /// </summary>
    /// <param name="_ingredient"></param>
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
}
