using System.Collections;
using UnityEngine;

public class WashStation : WorkStation
{
    /// --- Methods ---
    public IEnumerator WashPlate(Plate _plate)
    {
        if (_plate == null)
            yield break;

        SetObject(_plate);
        ShowObjectOnStation(_plate);

        yield return new WaitForSeconds(3f);

        _plate.SetState(PlateState.Clean);
        Debug.Log("plate has been washed and is clean!");
    }
}
