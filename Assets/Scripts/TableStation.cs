using UnityEngine;

public class TableStation : WorkStation {

    /// --- Attributes ---
    public Plate m_currentPlate;

    /// --- Methods ---

    /// <summary>
    /// Assigne une assiette à une commande et lui assigne cette station comme contenaire.
    /// </summary>
    /// <param name="_order"></param> <param name="_plate"></param>
    public void AssignPlateToOrder(Plate _plate, Order _order)
    {
        m_currentPlate = _plate;
        _plate.SetContainer(this);
        _order.SetPlate(_plate);
        _order.SetTableStation(this);
        ShowObjectOnStation(m_currentPlate);
    }


    /// <summary>
    /// Récupère l'assiette présente sur la table.
    /// </summary>
    /// <returns></returns>
    public Plate GetPlate() => m_currentPlate;
    public void RemovePlate()
    {
        m_currentPlate = null;
        ShowObjectOnStation(m_currentPlate);
    }


    /// <summary>
    /// Met à jour la visualisation des ingrédients sur l'assiette.
    /// </summary>
    public void UpdatePlateVisual()
    {
        if (m_currentPlate == null)
            return;

        Transform slot = transform.Find("Slot");
        if (slot == null)
        {
            Debug.LogWarning("TableStation missing 'Slot' transform.");
            return;
        }

        if (slot.childCount == 0)
            return;

        Transform plateTransform = slot.GetChild(0); // L’assiette
        if (plateTransform == null)
            return;

        foreach (Transform child in plateTransform)
        {
            if (child.name.Contains("IngredientVisual"))
                GameObject.Destroy(child.gameObject);
        }

        float height = 0.05f;
        foreach (Ingredient ing in m_currentPlate.GetIngredients())
        {
            if (ing == null || ing.GetPrefab() == null)
                continue;

            GameObject ingObj = GameObject.Instantiate(ing.GetPrefab(), plateTransform);
            ingObj.name = "IngredientVisual_" + ing.GetName();
            ingObj.transform.localPosition = new Vector3(0, height, 0);
            ingObj.transform.localRotation = Quaternion.identity;
            if(ing.GetName().Contains("Onion"))
                ingObj.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            else
                ingObj.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
            height += 0.30f;
        }
    }

}
