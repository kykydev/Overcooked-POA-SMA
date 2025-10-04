using System.Collections;
using UnityEngine;

public class CuttingStation : WorkStation
{

    /// --- Attributes ---
    [SerializeField] private Ingredient m_cuttedIngredient;
    public bool IsBusy { get; private set; }

    ///LockManagement
    public void LockStation() => IsBusy = true;
    public void UnlockStation() => IsBusy = false;

    /// --- Getters ---
    public Ingredient GetIngredient()
    {
        var tmp = m_cuttedIngredient;
        m_cuttedIngredient = null;
        ShowIngredientOnStation(null);
        return tmp;
    }

    /// --- Methods ---
    public IEnumerator CutIngredient(Ingredient _ingredient)
    {
        yield return new WaitForSeconds(3f);
        _ingredient.SetCuttingIngredientState(IngredientCuttingState.Cut);
        m_cuttedIngredient = _ingredient;
    }

    public void ShowIngredientOnStation(Ingredient _ingredient)
    {
        Transform slotTransform = transform.Find("CuttingIngredient");
        if (slotTransform == null)
            return;

        // Détruit tout ce qu'il y a sur la station
        foreach (Transform child in slotTransform)
            GameObject.Destroy(child.gameObject);

        // Si la station a un ingrédient, affiche-le
        if (_ingredient != null && _ingredient.GetPrefab() != null)
        {
            GameObject ingredientObj = GameObject.Instantiate(_ingredient.GetPrefab(), slotTransform);
            ingredientObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
