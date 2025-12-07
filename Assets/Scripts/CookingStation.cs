using System.Collections;
using UnityEngine;

public class CookingStation : WorkStation
{
    /// --- Attributes ---
    [SerializeField] private KitchenManager m_kitchenManager;

    /// --- Methods ---

    /// <summary>
    /// Cuit l'ingrédient passé en paramètre après un certain temps, change le prefab et le rajoute à la queue d'ingrédients de la commande.
    /// </summary>
    /// <param name="_ingredient"></param>  
    public IEnumerator CookIngredient(Ingredient _ingredient)
    {
        if (_ingredient == null)
            yield break;

        SetObject(_ingredient);
        ShowObjectOnStation(_ingredient);

        yield return new WaitForSeconds(5f);

        _ingredient.SetCookingIngredientState(IngredientCookingState.Cooked);
        if(_ingredient.GetName() == "Steak")
            _ingredient.SetPrefab(m_kitchenManager.m_steakCuitPrefab);
        else if (_ingredient.GetName() == "Chicken")
            _ingredient.SetPrefab(m_kitchenManager.m_chickenCuitPrefab);
        else if (_ingredient.GetName() == "Sausage")
            _ingredient.SetPrefab(m_kitchenManager.m_sausageCuitPrefab);
        ShowObjectOnStation(_ingredient);
        Debug.Log($"{_ingredient.GetName()} is cooked and ready to get picked up!");

        _ingredient.SetContainer(this);
       _ingredient.GetOrder().GetIngredientQueue().Enqueue(_ingredient);

        UnlockStation();
    }


    /// <summary>
    /// Vérifie si l'ingrédient actuellement sur la station est cuit.
    /// </summary>
    /// <returns></returns>
    public bool HasCookedIngredient()
    {
        object current = PeekObject();
        return current is Ingredient ingredient &&
               ingredient.GetCookingState() == IngredientCookingState.Cooked;
    }

}
