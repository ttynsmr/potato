using Potato;
using System;
using System.Collections;
using System.Collections.Generic;
using Potato.Battle.SkillCast;
using UnityEngine;

public class UnitService : MonoBehaviour
{
    public GameObject TestAvatar;

    private Potato.Network.NetworkService _networkService;
    private List<IUnit> units = new();

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

    public void OnReceiveSpawn(Potato.Unit.Spawn.Notification notification)
    {
        //Debug.Log($"spawn: area:{notification.AreaId} unit:{notification.UnitId}");
        var unit = new PlayerUnit(_networkService.Now, new UnitId(notification.UnitId), notification.Position.ToVector3(), notification.Direction, notification.Avatar);
        Register(unit);
    }

    public void OnReceiveDespawn(Potato.Unit.Despawn.Notification notification)
    {
        //Debug.Log($"despawn: area:{notification.AreaId} unit:{notification.UnitId}");
        UnregisterByUnitId(new UnitId(notification.UnitId));
    }

    public void OnReceiveMove(Potato.Unit.Move.Notification notification)
    {
        var unitId = new UnitId(notification.UnitId);
        var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
        if (unit == null || unit is ControllablePlayerUnit)
        {
            Debug.LogWarning($"area:{notification.AreaId} unit:{notification.UnitId} not found");
            return;
        }

        var moveCommand = new MoveCommand
        {
            StartTime = notification.Time,
            From = notification.From.ToVector3(),
            To = notification.To.ToVector3(),
            Speed = notification.Speed,
            Direction = notification.Direction,
        };
        unit.InputMove(moveCommand);
    }

    public void OnReceiveStop(Potato.Unit.Stop.Notification notification)
    {
        var unitId = new UnitId(notification.UnitId);
        var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
        if (unit == null)
        {
            Debug.LogWarning($"unit {notification.UnitId} not found");
            return;
        }

        var stopCommand = new StopCommand
        {
            StopTime = notification.StopTime,
            Direction = notification.Direction,
        };
        unit.InputStop(stopCommand);
    }

    public void OnReceiveKnockback(Potato.Unit.Knockback.Notification notification)
    {
        var unitId = new UnitId(notification.UnitId);
        var unit = units.Find((u) => { return u.UnitId.Equals(unitId); });
        if (unit == null)
        {
            Debug.LogWarning($"unit {notification.UnitId} not found");
            return;
        }

        var moveCommand = new KnockbackCommand
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

    public void OnReceiveCharacterStatus(Potato.Battle.SyncParameters.Notification notification)
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

    public ControllablePlayerUnit GetControllableUnit()
    {
        return units.Find((u) => { return u is ControllablePlayerUnit; }) as ControllablePlayerUnit;
    }
}
