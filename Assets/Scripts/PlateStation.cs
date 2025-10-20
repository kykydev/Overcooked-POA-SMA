using System.Collections.Generic;
using UnityEngine;

public class PlateStation : WorkStation
{

    /// --- Attributes ---
    private Queue<Plate> m_plates = new Queue<Plate>();

    /// --- Methods ---
    public Plate GetPlate()
    {
        if (m_plates.Count == 0)
            return null;
        return m_plates.Dequeue();
    }

    public void SetPlate(Plate _plate)
    {
        m_plates.Enqueue(_plate);
        _plate.SetContainer(this);
    }

    public void InitializePlateStack(int n, GameObject platePrefab)
    {
        for (int i = 0; i < n; i++)
        {
            Plate plate = new Plate(platePrefab);
            SetPlate(plate);
        }
    }
}
