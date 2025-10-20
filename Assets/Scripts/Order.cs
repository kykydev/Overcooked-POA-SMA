using System.Collections.Generic;
using UnityEngine;

/// --- Enumeration ---
public enum OrderStatus { Pending, Preparing, Completed, Delivered }

public class Order
{
    /// --- Attributes ---
    private string m_orderId;
    private Dish m_dish;
    private int m_reward;
    private Plate m_plate = null;
    private TableStation m_tableStation = null;

    private OrderStatus m_status = OrderStatus.Pending;

    /// --- Constructor ---
    public Order(string _id, Dish _dish, int _reward)
    {
        m_orderId = _id;
        m_dish = _dish;
        m_reward = _reward;
    }

    /// --- Getters ---
    public string GetOrderId() => m_orderId;
    public Dish GetDish() => m_dish;
    public int GetReward() => m_reward;
    public OrderStatus GetStatus() => m_status;
    public Plate GetPlate() => m_plate;
    public TableStation GetTableStation() => m_tableStation;  

    /// --- Setters ---
    public void SetOrderId(string _id) => m_orderId = _id;
    public void SetDish(Dish _dish) => m_dish = _dish;
    public void SetReward(int _newReward) => m_reward = _newReward;
    public void SetStatus(OrderStatus _newStatus) => m_status = _newStatus;
    public void SetPlate(Plate _plate) => m_plate = _plate;
    public void SetTableStation(TableStation _tableStation) => m_tableStation = _tableStation;


}
