using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    /// --- Attributes ---
    private Queue<Order> m_orders = new Queue<Order>();

    /// --- Methods ---
    //Retourne la prochaine commande dans la file d’attente
    public Order GiveOrder()
    {
        return m_orders.Dequeue();
    }

    //Ajouter une nouvelle commande à la file d’attente
    public void AddOrder(Order _newOrder)
    {
        m_orders.Enqueue(_newOrder);
    }

    //Recevoir une commande d’un agent et la valider via une workstation d'assemblage
    public void ReceiveOrder(Order _order, AssemblyStation _assemblyStation)
    {
        if (_assemblyStation.ValidateOrder(_order))
        {
            _order.SetStatus(OrderStatus.Delivered);
            _assemblyStation.ClearStation();
        }
        else
        {
            Debug.LogWarning("Order incorrect or incomplete!");
        }
    }
}
