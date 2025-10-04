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

    //Recevoir un plat d’un agent et le valider si il correspond a la commande en cours
    public void ReceiveOrder(Order _order, Dish _dish)
    {
        // Vérifie le nom et la composition du plat
        bool sameName = _order.GetDish().GetName() == _dish.GetName();

        List<Ingredient> expected = _order.GetDish().GetRecipe();
        List<Ingredient> delivered = _dish.GetRecipe();

        bool sameCount = expected.Count == delivered.Count;
        bool sameIngredients = sameCount;

        if (sameCount)
        {
            // Copie pour gérer les doublons
            List<Ingredient> expectedCopy = new List<Ingredient>(expected);
            foreach (Ingredient ing in delivered)
            {
                int idx = expectedCopy.FindIndex(e => e.GetName() == ing.GetName());
                if (idx == -1)
                {
                    sameIngredients = false;
                    break;
                }
                expectedCopy.RemoveAt(idx);
            }
        }
        else
        {
            sameIngredients = false;
        }

        if (sameName && sameIngredients)
        {
            _order.SetStatus(OrderStatus.Delivered);
        }
        else
        {
            Debug.LogWarning("Order incorrect or incomplete!");
        }
    }


}
