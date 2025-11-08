using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    /// --- Attributes ---
    private Queue<Order> m_orders = new Queue<Order>();
    private KitchenManager m_kitchenManager;

    /// --- Setters ---
    public void SetKitchenManager(KitchenManager _kitchenManager) => m_kitchenManager = _kitchenManager;

    /// --- Methods ---

    /// <summary>
    /// Permet de recevoir une commande et de vérifier si le plat livré correspond à la commande. Puis, ajoute l'argent à la caisse si la commande est correcte.
    /// </summary>
    /// <param name="_dish"></param> <param name="_order"></param>
    public void ReceiveOrder(Order _order, Dish _dish)
    {
        bool sameName = _order.GetDish().GetName() == _dish.GetName();

        List<Ingredient> expected = _order.GetDish().GetRecipe();
        List<Ingredient> delivered = _dish.GetRecipe();

        bool sameCount = expected.Count == delivered.Count;
        bool sameIngredients = sameCount;

        if (sameCount)
        {
            m_kitchenManager.AddMoney(_order.GetReward());
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
    }

}
