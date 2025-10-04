using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyStation : WorkStation
{
    /// --- Attributes ---
    [SerializeField] private Dish m_preparedDish;
    protected List<Ingredient> m_placedIngredients = new List<Ingredient>();

    /// --- Getters ---
    public Dish GetPreparedDish()
    {
        var tmp = m_preparedDish;
        m_preparedDish = null;
        return tmp;
    }

    /// --- Methods ---
    // Ajouter un ingrédient à la station de travail
    public void AddIngredient(Ingredient _ingredient)
    {
        m_placedIngredients.Add(_ingredient);
    }

    //Retirer un ingrédient de la station de travail
    public void RemoveIngredient(Ingredient _ingredient)
    {
        m_placedIngredients.Remove(_ingredient);
    }

    //Vider la station de travail
    public void ClearStation()
    {
        m_placedIngredients.Clear();
    }

    public bool CanAssembleDish(Order order)
    {
        List<Ingredient> recipe = order.GetDish().GetRecipe();

        if (m_placedIngredients.Count != recipe.Count)
            return false;

        List<Ingredient> recipeCopy = new List<Ingredient>(recipe);

        foreach (Ingredient placed in m_placedIngredients)
        {
            int idx = recipeCopy.FindIndex(r => r.GetName() == placed.GetName());
            if (idx == -1)
                return false;
            recipeCopy.RemoveAt(idx);
        }

        return true;
    }

    public IEnumerator AssembleDish(Order order)
    {
        if (!CanAssembleDish(order))
            yield break;

        yield return new WaitForSeconds(5f);
        Dish assembledDish = new Dish(order.GetDish().GetName(), order.GetDish().GetRecipe(), order.GetDish().GetPrefab());
        m_preparedDish = assembledDish;
        order.SetStatus(OrderStatus.Completed);
    }



}
