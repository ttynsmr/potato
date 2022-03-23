using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitMove
{
    [Test]
    public void PlayerUnitGeneration()
    {
        PlayerUnit unit1 = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
        Assert.IsNotNull(unit1);
        Assert.AreEqual(new UnitId(0), unit1.UnitId);
        Assert.AreEqual(Vector3.zero, unit1.Position);
        Assert.AreEqual(Potato.UnitDirection.Down, unit1.Direction);
        Assert.AreEqual(Vector3.zero, unit1.Position);
        unit1.Update(1000);
        Assert.AreEqual(Vector3.zero, unit1.Position);

        PlayerUnit unit2 = new PlayerUnit(0, new UnitId(7), Vector3.one, Potato.UnitDirection.Left, null);
        Assert.IsNotNull(unit2);
        Assert.AreEqual(new UnitId(7), unit2.UnitId);
        Assert.AreEqual(Vector3.one, unit2.Position);
        Assert.AreEqual(Potato.UnitDirection.Left, unit2.Direction);
    }

    [Test]
    public void PlayerUnitMoveAndStop()
    {
        PlayerUnit unit1 = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
        Assert.AreEqual(Vector3.zero, unit1.Position);
        unit1.Update(1000);
        Assert.AreEqual(Vector3.zero, unit1.Position);
        unit1.InputMove(new MoveCommand
        {
            From = Vector3.zero,
            To = new Vector3(10, 0, 0),
            StartTime = 2000,
            Speed = 1 / 1000.0f,
        });
        unit1.InputStop(new StopCommand
        {
            StopTime = 3000,
        });
        unit1.Update(1500);
        Assert.AreEqual(Vector3.zero, unit1.Position);
        unit1.Update(2000);
        Assert.AreEqual(Vector3.zero, unit1.Position);
        unit1.Update(2500);
        Assert.AreEqual(new Vector3(0.5f, 0, 0), unit1.Position);
        unit1.Update(3000);
        Assert.AreEqual(new Vector3(1, 0, 0), unit1.Position);
        unit1.Update(4000);
        Assert.AreEqual(new Vector3(1, 0, 0), unit1.Position);

        unit1.Update(5500);
        Assert.AreEqual(new Vector3(1, 0, 0), unit1.Position);
        unit1.InputMove(new MoveCommand
        {
            From = Vector3.zero,
            To = new Vector3(10, 0, 0),
            StartTime = 5000,
            Speed = 1 / 1000.0f,
        });
        unit1.InputStop(new StopCommand
        {
            StopTime = 7000,
        });
        unit1.Update(6000);
        Assert.AreEqual(new Vector3(1, 0, 0), unit1.Position);
        unit1.Update(7000);
        Assert.AreEqual(new Vector3(2, 0, 0), unit1.Position);
        unit1.Update(8000);
        Assert.AreEqual(new Vector3(2, 0, 0), unit1.Position);
    }

    [Test]
    public void MoveCommand()
    {
        var move = new MoveCommand
        {
            From = new Vector3(0, 0, 0),
            To = new Vector3(2, 0, 0),
            StartTime = 0,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.AreEqual(CommandType.Move, move.CommandType);
        Assert.AreEqual(new Vector3(0, 0, 0), move.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(1000));
        Assert.AreEqual(new Vector3(2, 0, 0), move.CalcCurrentPosition(2000));

        move.To = new Vector3(1, 1, 1).normalized * 2;
        Assert.AreEqual(new Vector3(0, 0, 0), move.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 1, 1).normalized * 1, move.CalcCurrentPosition(1000));
        Assert.AreEqual(new Vector3(1, 1, 1).normalized * 2, move.CalcCurrentPosition(2000));
        Assert.AreEqual(new Vector3(1, 1, 1).normalized * 2, move.CalcCurrentPosition(3000));
    }

    [Test]
    public void MoveCommandOffsetPosition()
    {
        var move = new MoveCommand
        {
            From = new Vector3(1, 0, 0),
            To = new Vector3(3, 0, 0),
            StartTime = 0,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.AreEqual(CommandType.Move, move.CommandType);
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(2, 0, 0), move.CalcCurrentPosition(1000));
        Assert.AreEqual(new Vector3(3, 0, 0), move.CalcCurrentPosition(2000));
        Assert.AreEqual(new Vector3(3, 0, 0), move.CalcCurrentPosition(3000));
    }

    [Test]
    public void MoveCommandNotZeroTime()
    {
        var move = new MoveCommand
        {
            From = new Vector3(1, 0, 0),
            To = new Vector3(3, 0, 0),
            StartTime = 100,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.AreEqual(CommandType.Move, move.CommandType);
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(100));
        Assert.AreEqual(new Vector3(2, 0, 0), move.CalcCurrentPosition(1100));
        Assert.AreEqual(new Vector3(3, 0, 0), move.CalcCurrentPosition(2100));
        Assert.AreEqual(new Vector3(3, 0, 0), move.CalcCurrentPosition(3100));
    }

    [Test]
    public void MoveCommandSpeedZero()
    {
        var move = new MoveCommand
        {
            From = new Vector3(1, 0, 0),
            To = new Vector3(3, 0, 0),
            StartTime = 100,
            Speed = 0,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.AreEqual(CommandType.Move, move.CommandType);
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(100));
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(1100));
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(2100));
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(3100));
    }

    [Test]
    public void MoveCommandGoaled()
    {
        var move = new MoveCommand
        {
            From = new Vector3(1, 0, 0),
            To = new Vector3(3, 0, 0),
            StartTime = 100,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.IsTrue(move.IsGoaled(move.GetGoalTime()));
        Assert.IsFalse(move.IsGoaled(100));
        Assert.IsFalse(move.IsGoaled(1000));
        Assert.IsFalse(move.IsGoaled(2000));
        Assert.IsFalse(move.IsGoaled(2099));
        Assert.IsTrue(move.IsGoaled(2100));
        Assert.IsTrue(move.IsGoaled(2101));
    }

    [Test]
    public void MoveCommandGoalTime()
    {
        var move = new MoveCommand
        {
            From = new Vector3(1, 0, 0),
            To = new Vector3(3, 0, 0),
            StartTime = 100,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.AreEqual(2100, move.GetGoalTime());
    }
}
