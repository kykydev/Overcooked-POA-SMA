using System.Collections.Generic;
using UnityEngine;

public abstract class WorkStation : MonoBehaviour {

    /// --- Attributes ---
    private object m_object;
    public bool IsBusy { get; private set; }

    ///LockManagement
    public void LockStation() => IsBusy = true;
    public void UnlockStation() => IsBusy = false;
    public bool IsLocked() => IsBusy;

    /// --- Getters/Setters ---
    public object GetObject()
    {
        var tmp = m_object;
        m_object = null;
        ShowObjectOnStation(null);
        return tmp;
    }

    public void SetObject(object obj)
    {
        m_object = obj;
    }

    public object PeekObject()
    {
        return m_object;
    }

    /// --- Display Methods ---
    public void ShowObjectOnStation(object _obj)
    {
        Transform slotTransform = transform.Find("Slot");
        if (slotTransform == null)
            return;

        foreach (Transform child in slotTransform)
            GameObject.Destroy(child.gameObject);

        if (_obj is Ingredient ingredient && ingredient.GetPrefab() != null)
        {
            GameObject ingredientObj = GameObject.Instantiate(ingredient.GetPrefab(), slotTransform);
            ingredientObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        else if (_obj is Dish dish && dish.GetPrefab() != null)
        {
            GameObject dishObj = GameObject.Instantiate(dish.GetPrefab(), slotTransform);
            dishObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        else if(_obj is Plate plate && plate.GetPrefab() != null)
        {
            GameObject plateObj = GameObject.Instantiate(plate.GetPrefab(), slotTransform);
            plateObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
