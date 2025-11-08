using System.Collections.Generic;
using UnityEngine;

public class PlateStation : WorkStation
{
    /// --- Attributes ---
    private Queue<Plate> m_plates;

    /// --- Methods ---

    /// <summary>
    /// Récupère une assiette de la station.
    /// </summary>
    public Plate GetPlate()
    {
        if (m_plates.Count == 0)
            return null;

        Plate plate = m_plates.Dequeue();
        UpdatePlateStackVisual();
        return plate;
    }


    /// <summary>
    /// Ajoute une assiette à la station et lui assigne cette station comme contenaire.
    /// </summary>
    /// <param name="_plate"></param>
    public void SetPlate(Plate _plate)
    {
        m_plates.Enqueue(_plate);
        _plate.SetContainer(this);
        UpdatePlateStackVisual();
    }


    /// <summary>
    /// Retourne vrai si la station possède au moins une assiette.
    /// </summary>
    public bool HasPlates()
    {
        return m_plates.Count > 0;
    }


    /// <summary>
    /// Initialise la pile d'assiettes avec un nombre donné d'assiettes propres.
    /// </summary>
    /// <param name="_cleanPlatePrefab"></param> <param name="_dirtyPlatePrefab"></param> <param name="_n"></param>
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


    /// <summary>
    /// Met à jour le visuel de la pile d'assiettes.
    /// </summary>
    public void UpdatePlateStackVisual()
    {
        Transform stackRoot = transform.Find("Slot");
        if (stackRoot == null)
        {
            Debug.LogWarning("PlateStation missing 'PlateStack' transform.");
            return;
        }

        foreach (Transform child in stackRoot)
        {
            if (child.name.Contains("PlateVisual"))
                GameObject.Destroy(child.gameObject);
        }

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

            height += 0.08f;
            index++;
        }
    }

}
