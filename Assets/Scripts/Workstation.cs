using System.Collections.Generic;
using UnityEngine;

public abstract class WorkStation : MonoBehaviour
{
    /// --- Attributes ---
    protected List<Ingredient> m_placedIngredients = new List<Ingredient>();

    /// --- Methods ---
    // Ajouter un ingr�dient � la station de travail
    public void AddIngredient(Ingredient _ingredient)
    {
        m_placedIngredients.Add(_ingredient);
    }

    //Retirer un ingr�dient de la station de travail
    public void RemoveIngredient(Ingredient _ingredient)
    {
        m_placedIngredients.Remove(_ingredient);
    }

    //Vider la station de travail
    public void ClearStation()
    {
        m_placedIngredients.Clear();
    }
}
