using System.Collections.Generic;
using UnityEngine;

public enum OrderStatus { Pending, Preparing, Completed, Delivered }

public class Order
{
    [SerializeField] private string m_orderId;
    [SerializeField] private List<Ingredient> m_recipe;
    [SerializeField] private int m_reward;

    private OrderStatus m_status = OrderStatus.Pending;

    // --- Getters ---
    public string GetOrderId() => m_orderId;
    public List<Ingredient> GetRecipe() => m_recipe;
    public int GetReward() => m_reward;
    public OrderStatus GetStatus() => m_status;

    // --- Setters ---
    public void SetOrderId(string _id) => m_orderId = _id;
    public void SetRecipe(List<Ingredient> _newRecipe) => m_recipe = _newRecipe;
    public void SetReward(int _newReward) => m_reward = _newReward;
    public void SetStatus(OrderStatus _newStatus) => m_status = _newStatus;
}
