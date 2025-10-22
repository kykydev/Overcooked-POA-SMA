using System.Collections.Generic;
using UnityEngine;

public class PlateStation : WorkStation
{

    /// --- Attributes ---
    private Queue<Plate> m_plates;

    /// --- Methods ---
    public Plate GetPlate()
    {
        if (m_plates.Count == 0)
            return null;

        Plate plate = m_plates.Dequeue();
        UpdatePlateStackVisual();
        return plate;
    }


    public void SetPlate(Plate _plate)
    {
        m_plates.Enqueue(_plate);
        _plate.SetContainer(this);
        UpdatePlateStackVisual();
    }

    public bool HasPlates()
    {
        return m_plates.Count > 0;
    }

    public void InitializePlateStack(int _n, GameObject _cleanPlatePrefab, GameObject _dirtyPlatePrefab)
    {
        m_plates = new Queue<Plate>();
        for (int i = 0; i < _n; i++)
        {
            Plate plate = new Plate(_cleanPlatePrefab, _dirtyPlatePrefab);
            SetPlate(plate);
        }
        UpdatePlateStackVisual();
    }

    public void UpdatePlateStackVisual()
    {
        Transform stackRoot = transform.Find("Slot");
        if (stackRoot == null)
        {
            Debug.LogWarning("PlateStation missing 'PlateStack' transform.");
            return;
        }

        // Supprime les anciens visuels
        foreach (Transform child in stackRoot)
        {
            if (child.name.Contains("PlateVisual"))
                GameObject.Destroy(child.gameObject);
        }

        // Affiche les assiettes de la pile
        float height = 0f;
        int index = 0;

        foreach (Plate plate in m_plates)
        {
            if (plate == null || plate.GetPrefab() == null)
                continue;

            GameObject plateObj = GameObject.Instantiate(plate.GetPrefab(), stackRoot);
            plateObj.name = "PlateVisual_" + index;
            plateObj.transform.localPosition = new Vector3(0, height, 0);
            plateObj.transform.localRotation = Quaternion.identity;

            height += 0.08f; // léger décalage vertical pour empiler proprement
            index++;
        }
    }

}
