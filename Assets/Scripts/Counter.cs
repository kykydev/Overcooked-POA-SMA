using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    private Queue<Order> m_orders = new Queue<Order>();

    // ---- Methods ----

    public Order GiveOrder(){ /// Fournir la prochaine commande dans la file d’attente au client
        if (m_orders.Count == 0) return null;
        return m_orders.Dequeue();
    }

    public void ReceiveOrder(Order _order, Agent _agent, Workstation _workstation){ /// Recevoir une commande complétée
        if (_workstation.ValidateOrder(_order))
        {
            _order.SetStatus(OrderStatus.Delivered);
            _agent.AddMoney(_order.GetReward());
            _workstation.ClearStation();

            Debug.Log("Order delivered successfully!");
        }
        else
        {
            Debug.LogWarning("Order incorrect or incomplete!");
        }
    }

    public void AddOrder(Order _newOrder) => m_orders.Enqueue(_newOrder); /// Ajouter une nouvelle commande à la file d’attente
}
