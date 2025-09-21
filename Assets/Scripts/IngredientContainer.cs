using UnityEngine;

public class IngredientContainer : MonoBehaviour
{
    //[SerializeField] private Transform m_pickupPoint;
    [SerializeField] private Ingredient m_ingredient;

    // ---- Methods ----

    public Ingredient ProvideIngredient(){ /// Fournir l�ingr�dient contenu dans le container
        return m_ingredient;
    }

    public void SetIngredient(Ingredient _ingredient){ /// D�finir l�ingr�dient contenu dans le container
        m_ingredient = _ingredient;
    }
}
