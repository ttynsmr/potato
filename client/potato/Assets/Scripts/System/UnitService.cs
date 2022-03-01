using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitService : MonoBehaviour
{
    List<IUnit> units = new List<IUnit>();

    public void Register(IUnit unit)
    {
        units.Add(unit);
    }

    public void Unregister(IUnit unit)
    {
        units.Remove(unit);
    }

    public void UnregisterByUnitId(UnitId unitId)
    {
        int v = units.RemoveAll((u) => { return u.UnitId.Equals(unitId); });
    }

    private void Start()
    {
        foreach(var unit in units)
        {
            unit.Start();
        }
    }

    private void Update()
    {
        foreach (var unit in units)
        {
            unit.Update();
        }
    }
}
