using System.Collections.Generic;
using UnityEngine;

public class Order
{
    /// --- Attributes ---
    private string m_orderId;
    private Dish m_dish;
    private int m_reward;
    private Plate m_plate = null;
    private TableStation m_tableStation = null;

    private Queue<Ingredient> m_ingredientQueue = new Queue<Ingredient>();

    /// --- Constructor ---
    public Order(string _id, Dish _dish, int _reward)
    {
        m_orderId = _id;
        m_dish = _dish;
        m_reward = _reward;
    }

    /// --- Plate & Order Assignment State ---
    private bool m_isPlateBeingAssigned = false;
    public bool IsPlateBeingAssigned() => m_isPlateBeingAssigned;
    public void SetPlateBeingAssigned(bool value) => m_isPlateBeingAssigned = value;

    private bool m_isBeingDelivered = false;
    public bool IsBeingDelivered() => m_isBeingDelivered;
    public void SetBeingDelivered(bool value) => m_isBeingDelivered = value;

    /// --- Getters ---
    public string GetOrderId() => m_orderId;
    public Dish GetDish() => m_dish;
    public int GetReward() => m_reward;
    public Plate GetPlate() => m_plate;
    public TableStation GetTableStation() => m_tableStation;
    public Queue<Ingredient> GetIngredientQueue() => m_ingredientQueue;

    /// --- Setters ---
    public void SetOrderId(string _id) => m_orderId = _id;
    public void SetDish(Dish _dish) => m_dish = _dish;
    public void SetReward(int _newReward) => m_reward = _newReward;
    public void SetPlate(Plate _plate) => m_plate = _plate;
    public void SetTableStation(TableStation _tableStation) => m_tableStation = _tableStation;

}
