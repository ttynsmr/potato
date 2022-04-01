using Potato;
using System;
using System.Collections;
using System.Collections.Generic;
using Torikime.Battle.SkillCast;
using UnityEngine;

public class UnitService : MonoBehaviour
{
    public GameObject TestAvatar;

    private Potato.Network.NetworkService _networkService;
    private List<IUnit> units = new List<IUnit>();

    private void Start()
    {
        _networkService = FindObjectOfType<Potato.Network.NetworkService>();
    }

    public void Register(IUnit unit)
    {
        unit.UnitService = this;
        units.Add(unit);
        unit.Start();
    }

    public void Unregister(IUnit unit)
    {
        unit.OnDespawn();
        units.Remove(unit);
    }

    public void UnregisterByUnitId(UnitId unitId)
    {
        var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
        Unregister(unit);
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
            Direction = notification.Direction,
        };
        unit.InputMove(moveCommand);
    }

    public void OnReceiveStop(Torikime.Unit.Stop.Notification notification)
    {
        var unitId = new UnitId(notification.UnitId);
        var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
        if (unit == null)
        {
            Debug.LogWarning($"unit {notification.UnitId} not found");
            return;
        }

        StopCommand stopCommand = new StopCommand
        {
            StopTime = notification.StopTime,
            Direction = notification.Direction,
        };
        unit.InputStop(stopCommand);
    }

    public void OnReceiveKnockback(Torikime.Unit.Knockback.Notification notification)
    {
        var unitId = new UnitId(notification.UnitId);
        var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
        if (unit == null)
        {
            Debug.LogWarning($"unit {notification.UnitId} not found");
            return;
        }

        KnockbackCommand moveCommand = new KnockbackCommand
        {
            StartTime = notification.StartTime,
            EndTime = notification.EndTime,
            From = notification.From.ToVector3(),
            To = notification.To.ToVector3(),
            Speed = notification.Speed,
            Direction = notification.Direction,
        };
        unit.InputKnockback(moveCommand);
    }

    private void Update()
    {
        var now = _networkService.Now;
        foreach (var unit in units)
        {
            unit.Update(now);
        }
    }

    public void Reset()
    {
        foreach (var unit in units)
        {
            unit.Destroy();
        }
        units.Clear();
    }

    public void OnReceiveSkillCast(Notification notification)
    {
        foreach(var result in notification.Results)
        {
            var unitId = new UnitId(result.ReceiverUnitId);
            var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
            if (unit == null)
            {
                continue;
            }

            if (unit is ControllablePlayerUnit)
            {
                // me
            }
            else
            {
                // others
            }
        }
    }

    public void OnReceiveCharacterStatus(Torikime.Battle.SyncParameters.Notification notification)
    {
        var unitId = new UnitId(notification.UnitId);
        var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
        if (unit == null)
        {
            return;
        }
        if (unit is IHasStatus)
        {
            var iunit = unit as IHasStatus;
            iunit.CharacterStatus = notification.Parameters;
        }
    }
}
