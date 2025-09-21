using System.Collections.Generic;
using UnityEngine;

public class Workstation : MonoBehaviour
{
    private List<Ingredient> m_placedIngredients = new List<Ingredient>();

    public void AddIngredient(Ingredient _ingredient){ /// Ajouter un ingr�dient � la station de travail
        m_placedIngredients.Add(_ingredient);
    }

    public bool ValidateOrder(Order _order){ /// Valider la commande en cours comment �a marche : la commande est valid�e si tous les ingr�dients plac�s correspondent � la recette
        List<Ingredient> recipe = _order.GetRecipe();

        if (m_placedIngredients.Count != recipe.Count)
            return false;

        foreach (Ingredient required in recipe)
        {
            bool match = m_placedIngredients.Exists(i => i.GetName() == required.GetName());
            if (!match) return false;
        }

        _order.SetStatus(OrderStatus.Completed);
        return true;
    }

    public void ClearStation(){ /// Vider la station de travail
        m_placedIngredients.Clear();
    }
}
