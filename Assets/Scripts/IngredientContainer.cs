using UnityEngine;

public class IngredientContainer : MonoBehaviour
{
    //[SerializeField] private Transform m_pickupPoint;
    [SerializeField] private Ingredient m_ingredient;

    // ---- Methods ----

    public Ingredient ProvideIngredient(){ /// Fournir l’ingrédient contenu dans le container
        return m_ingredient;
    }

    public void SetIngredient(Ingredient _ingredient){ /// Définir l’ingrédient contenu dans le container
        m_ingredient = _ingredient;
    }
}
