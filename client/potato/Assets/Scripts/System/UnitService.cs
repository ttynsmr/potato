using Potato;
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

        MoveCommand moveCommand = new MoveCommand
        {
            StartTime = notification.Time,
            From = notification.From.ToVector3(),
            To = notification.To.ToVector3(),
            Speed = notification.Speed,
            Direction = 0,
        };
        unit.InputMove(moveCommand);
    }

    public void OnReceiveStop(Torikime.Unit.Stop.Notification notification)
    {
        var unitId = new UnitId(notification.UnitId);
        var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
        if (unit == null || unit is ControllablePlayerUnit)
        {
            Debug.LogWarning($"unit {notification.UnitId} not found");
            return;
        }

        StopCommand stopCommand = new StopCommand
        {
            StopTime = notification.StopTime,
        };
        unit.InputStop(stopCommand);
    }

    private void Update()
    {
        foreach (var unit in units)
        {
            unit.Update(Time.deltaTime);
        }
    }
}
