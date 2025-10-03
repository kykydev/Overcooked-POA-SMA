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
        ShowIngredientOnStation(null);
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

    public void ShowIngredientOnStation(Ingredient _ingredient)
    {
        Transform slotTransform = transform.Find("CookingIngredient");
        if (slotTransform == null)
            return;

        // Détruit tout ce qu'il y a sur la station
        foreach (Transform child in slotTransform)
            GameObject.Destroy(child.gameObject);

        // Si la station a un ingrédient, affiche-le
        if (_ingredient != null && _ingredient.GetPrefab() != null)
        {
            GameObject ingredientObj = GameObject.Instantiate(_ingredient.GetPrefab(), slotTransform);
            ingredientObj.transform.localPosition = Vector3.zero;
            ingredientObj.transform.localRotation = Quaternion.identity;
        }
    }
}
