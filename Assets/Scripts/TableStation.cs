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
    }
