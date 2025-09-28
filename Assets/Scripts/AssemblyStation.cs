using System.Collections.Generic;
using UnityEngine;

public class AssemblyStation : WorkStation
{
    /// --- Attributes ---

    /// --- Methods ---

    //Valider la commande en cours comment �a marche : la commande est valid�e si tous les ingr�dients plac�s correspondent � la recette
    public bool ValidateOrder(Order _order)
    {
        List<Ingredient> recipe = _order.GetDish().GetRecipe();

        if (m_placedIngredients.Count != recipe.Count)
            return false;

        //On cr�e une copie de la recette pour g�rer les doublons
        List<Ingredient> recipeCopy = new List<Ingredient>(recipe);

        foreach (Ingredient placed in m_placedIngredients)
        {
            //On retire le premier ingr�dient correspondant trouv� (par nom)
            int idx = recipeCopy.FindIndex(r => r.GetName() == placed.GetName());
            if (idx == -1)
                return false; // L'ingr�dient n'est pas dans la recette ou d�j� utilis�
            recipeCopy.RemoveAt(idx);
        }

        _order.SetStatus(OrderStatus.Completed);
        return true;
    }
}
