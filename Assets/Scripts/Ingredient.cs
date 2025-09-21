using UnityEngine;

public class Ingredient
{
    [SerializeField] private string m_ingredientName;
    [SerializeField] private IngredientContainer m_container;

    public Ingredient(string _name){ /// Constructor
        m_ingredientName = _name;
    }

    // ---- Methods ----

    public string GetName() => m_ingredientName;
    public IngredientContainer GetContainer() => m_container;
    public void SetContainer(IngredientContainer _container){
        m_container = _container;
    }

}
