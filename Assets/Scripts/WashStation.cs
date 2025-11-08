using System.Collections;
using UnityEngine;

public class WashStation : WorkStation
{
    /// --- Methods ---

    /// <summary>
    /// Nettoie l'assiette passée en paramètre après un certain temps et change son état ainsi que son prefab, puis déverrouille la station.
    /// </summary>
    /// <param name="_plate"></param>
    public IEnumerator WashPlate(Plate _plate)
    {
        if (_plate == null)
            yield break;

        SetObject(_plate);
        ShowObjectOnStation(_plate);

        yield return new WaitForSeconds(3f);

        _plate.SetPrefab(_plate.GetCleanPlatePrefab());
        _plate.SetState(PlateState.Clean);
        Debug.Log("plate has been washed and is clean!");
        UnlockStation();
    }

}
