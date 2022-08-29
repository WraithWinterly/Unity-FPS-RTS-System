/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSSelectedUnitsTable : MonoBehaviour
{
    public readonly Dictionary<int, GameObject> SelectedTable = new();

    public void AddToDict(GameObject gObj)
    {
        var id = gObj.GetInstanceID();
        if (!SelectedTable.ContainsKey(id))
        {
            SelectedTable.Add(id, gObj);
        }
    }

    public void RemoveFromDict(GameObject gObj)
    {
        var id = gObj.GetInstanceID();
        if (SelectedTable.ContainsKey(id))
        {
            SelectedTable.Remove(id);
        }
    }

    public void RemoveAllDict()
    {
        SelectedTable.Clear();
    }

    public void UpdateSelections()
    {
        // Remove selected script from game objects that aren't in the dictionary

        RTSSelectedComponent[] selected = FindObjectsOfType<RTSSelectedComponent>();
        foreach (RTSSelectedComponent component in selected)
        {
            if (!SelectedTable.ContainsKey(component.gameObject.GetInstanceID()))
            {
                DestroyImmediate(component);
            }

        }
        // Attach script to game objects in Dict
        foreach (int key in SelectedTable.Keys)
        {

            if (!SelectedTable[key].TryGetComponent(out RTSSelectedComponent test))
            {
                SelectedTable[key].AddComponent<RTSSelectedComponent>();
            }
        }

    }

    public bool HasUnit(GameObject gObject)
    {
        return SelectedTable.ContainsKey(gObject.GetInstanceID());
    }
}
