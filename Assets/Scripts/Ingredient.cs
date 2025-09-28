using UnityEngine;

/// --- Enumeration ---
public enum IngredientCookingState { Raw, Cooked }
public enum IngredientCuttingState { Whole, Cut }

public class Ingredient
{
    /// --- Attributes ---
    private string m_ingredientName;
    private IngredientCookingState m_cookingState = IngredientCookingState.Raw;
    private IngredientCuttingState m_cuttingState = IngredientCuttingState.Whole;
    private bool m_needsCooking = false;
    private bool m_needsCutting = false;
    private WorkStation m_container;

    /// --- Constructor ---
    public Ingredient(string _name, bool _needsCooking, bool _needsCutting)
    {
        m_ingredientName = _name;
        m_needsCooking = _needsCooking;
        m_needsCutting = _needsCutting;
    }

    /// --- Getters ---
    public WorkStation GetContainer() => m_container;
    public string GetName() => m_ingredientName;
    public IngredientCookingState GetState() => m_cookingState;
    public bool GetNeedsCooking() => m_needsCooking;
    public IngredientCuttingState GetCuttingState() => m_cuttingState;
    public bool GetNeedsCutting() => m_needsCutting;

    /// --- Setters ---
    public void SetContainer(WorkStation _container) => m_container = _container;
    public void SetIngredientName(string _name) => m_ingredientName = _name;
    public void SetCookingIngredientState(IngredientCookingState _state) => m_cookingState = _state;
    public void SetCuttingIngredientState(IngredientCuttingState _state) => m_cuttingState = _state;

    /// --- Methods ---
    public bool NeedsCooking() => m_needsCooking && m_cookingState == IngredientCookingState.Raw;
    public bool IsCooked() => m_cookingState == IngredientCookingState.Cooked;
    public bool NeedsCutting() => m_needsCutting && m_cuttingState == IngredientCuttingState.Whole;
    public bool IsCut() => m_cuttingState == IngredientCuttingState.Cut;
}
