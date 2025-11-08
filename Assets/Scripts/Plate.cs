using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// --- Enumeration ---
public enum PlateState { Clean, Dirty }
public class Plate
{
    private GameObject m_prefab;
    private GameObject m_cleanPlatePrefab;
    private GameObject m_dirtyPlatePrefab;
    private PlateState m_state = PlateState.Clean;
    private object m_container;

    /// --- Attributs ---
    private List<Ingredient> m_placedIngredients = new List<Ingredient>();
    private Dish m_preparedDish;

    /// --- Constructor ---
    public Plate(GameObject _cleanPlatePrefab, GameObject _dirtyPlatePrefab)
    {
        m_prefab = _cleanPlatePrefab;
        m_cleanPlatePrefab = _cleanPlatePrefab;
        m_dirtyPlatePrefab = _dirtyPlatePrefab;
    }

    /// --- Getters ---
    public GameObject GetPrefab() => m_prefab;
    public GameObject GetCleanPlatePrefab() => m_cleanPlatePrefab;
    public GameObject GetDirtyPlatePrefab() => m_dirtyPlatePrefab;
    public PlateState GetState() => m_state;
    public object GetContainer() => m_container;
    public List<Ingredient> GetIngredients() => m_placedIngredients;
    public bool IsClean() => m_state == PlateState.Clean;

    /// --- Setters ---
    public void SetPrefab(GameObject prefab) => m_prefab = prefab;
    public void SetState(PlateState state) => m_state = state;
    public void SetContainer(object container) => m_container = container;

    /// --- Methods ---

    /// <summary>
    /// Ajoute un ingrédient sur l'assiette.
    /// </summary>
    /// <param name="ingredient"></param>
    public void AddIngredient(Ingredient ingredient)
    {
        m_placedIngredients.Add(ingredient);
    }


    /// <summary>
    /// Supprime tous les ingrédients de l'assiette.
    /// </summary>
    public void ClearIngredients()
    {
        m_placedIngredients.Clear();
    }


    /// <summary>
    /// Vérifie si l'assiette peut assembler le plat de la commande, renvoie true si c'est le cas.
    /// </summary>
    /// <param name="order"></param>
    public bool CanAssembleDish(Order order)
    {
        List<Ingredient> recipe = order.GetDish().GetRecipe();
        if (m_placedIngredients.Count != recipe.Count)
            return false;

        List<Ingredient> recipeCopy = new List<Ingredient>(recipe);
        foreach (Ingredient placed in m_placedIngredients)
        {
            int idx = recipeCopy.FindIndex(r => r.GetName() == placed.GetName());
            if (idx == -1) return false;
            recipeCopy.RemoveAt(idx);
        }
        return true;
    }


    /// <summary>
    /// Assemble le plat de la commande après un certain temps si c'est possible. Crée un nouvel objet Dish représentant le plat préparé.
    /// </summary>
    /// <param name="order"></param>
    public IEnumerator AssembleDish(Order order)
    {
        if (!CanAssembleDish(order))
            yield break;

        yield return new WaitForSeconds(5f);

        m_preparedDish = new Dish(order.GetDish().GetName(), order.GetDish().GetRecipe(), order.GetDish().GetPrefab());
    }


    /// <summary>
    /// Retourne le plat préparé et réinitialise l'assiette en supprimant les ingrédients.
    /// </summary>
    public Dish GetPreparedDish()
    {
        var tmp = m_preparedDish;
        m_preparedDish = null;
        ClearIngredients();
        return tmp;
    }

}
