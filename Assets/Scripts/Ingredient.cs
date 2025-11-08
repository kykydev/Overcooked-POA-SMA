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
    private GameObject m_prefab;
    private Order m_order;

    /// --- Constructor ---
    public Ingredient(string _name, bool _needsCooking, bool _needsCutting, GameObject _prefab)
    {
        m_ingredientName = _name;
        m_needsCooking = _needsCooking;
        m_needsCutting = _needsCutting;
        m_prefab = _prefab;
    }

    /// --- Getters ---
    public WorkStation GetContainer() => m_container;
    public string GetName() => m_ingredientName;
    public IngredientCookingState GetCookingState() => m_cookingState;
    public bool GetNeedsCooking() => m_needsCooking;
    public IngredientCuttingState GetCuttingState() => m_cuttingState;
    public bool GetNeedsCutting() => m_needsCutting;
    public GameObject GetPrefab() => m_prefab;
    public Order GetOrder() => m_order;

    /// --- Setters ---
    public void SetContainer(WorkStation _container) => m_container = _container;
    public void SetIngredientName(string _name) => m_ingredientName = _name;
    public void SetCookingIngredientState(IngredientCookingState _state) => m_cookingState = _state;
    public void SetCuttingIngredientState(IngredientCuttingState _state) => m_cuttingState = _state;
    public void SetOrder(Order _order) => m_order = _order;
    public void SetPrefab(GameObject _prefab) => m_prefab = _prefab;

    /// --- Methods ---
    public bool NeedsCooking() => m_needsCooking && m_cookingState == IngredientCookingState.Raw;
    public bool IsCooked() => m_cookingState == IngredientCookingState.Cooked;
    public bool NeedsCutting() => m_needsCutting && m_cuttingState == IngredientCuttingState.Whole;
    public bool IsCut() => m_cuttingState == IngredientCuttingState.Cut;
}
