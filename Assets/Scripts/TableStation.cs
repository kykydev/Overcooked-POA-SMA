using UnityEngine;

public class TableStation : WorkStation {

    /// --- Attributes ---
    public Plate CurrentPlate;

    public void AssignPlateToOrder(Plate _plate, Order _order)
    {
        CurrentPlate = _plate;
        _plate.SetContainer(this);
        _order.SetPlate(_plate);
        _order.SetTableStation(this);
        ShowObjectOnStation(CurrentPlate);
    }

    public Plate GetPlate() => CurrentPlate;
    public void RemovePlate()
    {
        CurrentPlate = null;
        ShowObjectOnStation(CurrentPlate);
    }

    public void UpdatePlateVisual()
    {
        if (CurrentPlate == null)
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
        foreach (Ingredient ing in CurrentPlate.GetIngredients())
        {
            if (ing == null || ing.GetPrefab() == null)
                continue;

            GameObject ingObj = GameObject.Instantiate(ing.GetPrefab(), plateTransform);
            ingObj.name = "IngredientVisual_" + ing.GetName();
            ingObj.transform.localPosition = new Vector3(0, height, 0);
            ingObj.transform.localRotation = Quaternion.identity;
            height += 0.20f;
        }
    }
    }
