using System.Collections.Generic;
using UnityEngine;

public class AssemblyStation : WorkStation
{
    /// --- Attributes ---

    /// --- Methods ---

    //Valider la commande en cours comment ça marche : la commande est validée si tous les ingrédients placés correspondent à la recette
    public bool ValidateOrder(Order _order)
    {
        List<Ingredient> recipe = _order.GetDish().GetRecipe();

        if (m_placedIngredients.Count != recipe.Count)
            return false;

        //On crée une copie de la recette pour gérer les doublons
        List<Ingredient> recipeCopy = new List<Ingredient>(recipe);

        foreach (Ingredient placed in m_placedIngredients)
        {
            //On retire le premier ingrédient correspondant trouvé (par nom)
            int idx = recipeCopy.FindIndex(r => r.GetName() == placed.GetName());
            if (idx == -1)
                return false; // L'ingrédient n'est pas dans la recette ou déjà utilisé
            recipeCopy.RemoveAt(idx);
        }

        _order.SetStatus(OrderStatus.Completed);
        return true;
    }
}
