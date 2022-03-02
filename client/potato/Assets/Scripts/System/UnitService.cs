using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitService : MonoBehaviour
{
    public GameObject TestAvatar;

    private List<IUnit> units = new List<IUnit>();

    public void Register(IUnit unit)
    {
        unit.UnitService = this;
        units.Add(unit);
        unit.Start();
    }

    public void Unregister(IUnit unit)
    {
        units.Remove(unit);
    }

    public void UnregisterByUnitId(UnitId unitId)
    {
        int v = units.RemoveAll((u) => { return u.UnitId.Equals(unitId); });
    }

    public void OnReceiveMove(Torikime.Unit.Move.Notification notification)
    {
        var unitId = new UnitId(notification.UnitId);
        var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
        if (unit == null || unit is ControllablePlayerUnit)
        {
            Debug.LogWarning($"unit {notification.UnitId} not found");
            return;
        }

        unit.InputMove(notification);
    }

    private void Update()
    {
        foreach (var unit in units)
        {
            unit.Update(Time.deltaTime);
        }
    }
}
