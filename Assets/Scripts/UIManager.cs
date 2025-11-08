using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

// Petite classe pour stocker la paire Nom/Sprite (plus propre que deux listes séparées)
[System.Serializable]
public class IngredientSprite
{
    public string ingredientName;
    public Sprite sprite;
}

public class UIManager : MonoBehaviour
{
    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI m_timerText;

    [Header("Money UI")]
    [SerializeField] private TextMeshProUGUI m_moneyText;

    [Header("References")]
    [SerializeField] private KitchenManager m_kitchenManager;

    [Header("Order UI")]
    [SerializeField] private GameObject m_orderTicketPrefab;
    [SerializeField] private GameObject m_ingredientIconPrefab;
    [SerializeField] private Transform m_orderListContainer;

    [Header("Sprites References")]
    [SerializeField] private List<IngredientSprite> m_ingredientSprites;
    [SerializeField] private List<IngredientSprite> m_dishSprites;

    private Dictionary<Order, GameObject> m_activeOrderUIs = new Dictionary<Order, GameObject>();

    /// --- Methods ---

    /// <summary>
    /// Met à jour le timer et l'argent total chaque frame.
    /// </summary>
    void Update()
    {
        if (m_kitchenManager != null && m_moneyText != null)
        {
            m_moneyText.text = m_kitchenManager.GetTotalMoney().ToString();
        }

        if (m_kitchenManager != null && m_timerText != null)
        {
            float timeInSeconds = m_kitchenManager.GetGameTimer();

            if (timeInSeconds < 0) timeInSeconds = 0;

            float minutes = Mathf.FloorToInt(timeInSeconds / 60);
            float seconds = Mathf.FloorToInt(timeInSeconds % 60); // Le reste de la division

            m_timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }


    /// <summary>
    /// Trouve le sprite d'un ingrédient ou plat par son nom.
    /// </summary>
    /// <param name="spriteList"></param> <param name="_name"></param>
    private Sprite GetSpriteByName(string _name, List<IngredientSprite> spriteList)
    {
        return spriteList.FirstOrDefault(item => item.ingredientName == _name)?.sprite;
    }


    /// <summary>
    /// Crée et affiche un ticket de commande sur l'UI avec des images.
    /// </summary>
    /// <param name="_order"></param>
    public void AddOrderToUI(Order _order)
    {
        if (m_activeOrderUIs.Count >= 10 || m_activeOrderUIs.ContainsKey(_order))
            return;

        GameObject newTicket = Instantiate(m_orderTicketPrefab, m_orderListContainer);

        // 1. Image du Plat
        Image dishImage = newTicket.transform.Find("DishContainer/DishImage").GetComponent<Image>();
        if (dishImage != null)
        {
            // Récupère le sprite du plat par son nom (ex: "Burger", "Salade Composée")
            Sprite dishSprite = GetSpriteByName(_order.GetDish().GetName(), m_dishSprites);

            if (_order.GetDish().GetName() == "Salad" || _order.GetDish().GetName() == "Steak")
            {
                RectTransform rt = dishImage.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(100, 100);
                Vector2 newPos = rt.anchoredPosition;
                newPos.y = 10f;
                rt.anchoredPosition = newPos;
            }

            if (dishSprite != null)
            {
                dishImage.sprite = dishSprite;
            }
            else
            {
                Debug.LogWarning($"Sprite not found for dish: {_order.GetDish().GetName()}");
            }
        }

        // 2. Liste d'Ingrédients (Icônes)
        Transform ingredientListContainer = newTicket.transform.Find("IngredientListContainer");
        if (ingredientListContainer != null)
        {
            foreach (Ingredient ingredient in _order.GetDish().GetRecipe())
            {
                GameObject newIngredientIcon = Instantiate(m_ingredientIconPrefab, ingredientListContainer);
                Image ingredientImage = newIngredientIcon.GetComponent<Image>();

                if (ingredientImage != null)
                {
                    // Récupère le sprite de l'ingrédient par son nom (ex: "Tomate", "Steak", "Salade")
                    Sprite ingredientSprite = GetSpriteByName(ingredient.GetName(), m_ingredientSprites);
                    if (ingredientSprite != null)
                    {
                        ingredientImage.sprite = ingredientSprite;
                    }
                    else
                    {
                        Debug.LogWarning($"Sprite not found for ingredient: {ingredient.GetName()}");
                    }
                }
            }
        }

        m_activeOrderUIs.Add(_order, newTicket);
    }


    /// <summary>
    /// Supprime le ticket de commande de l'UI.
    /// </summary>
    /// <param name="_order"></param>
    public void RemoveOrderFromUI(Order _order)
    {
        if (m_activeOrderUIs.TryGetValue(_order, out GameObject ticketToDestroy))
        {
            m_activeOrderUIs.Remove(_order);
            Destroy(ticketToDestroy);
        }
    }


    /// <summary>
    /// Force la reconstruction du layout des commandes à la fin de la frame, après l'initialisation.
    /// </summary>
    public void RefreshOrderLayoutAfterInitialization()
    {
        StartCoroutine(ForceRebuildAtEndOfFrame());
    }


    /// <summary>
    /// Coroutine pour forcer la reconstruction du layout à la fin de la frame.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ForceRebuildAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.MarkLayoutForRebuild(m_orderListContainer as RectTransform);
    }

}