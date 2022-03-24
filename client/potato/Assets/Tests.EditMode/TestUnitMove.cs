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
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
        Assert.AreEqual(Potato.UnitDirection.Down, unit1.Direction);
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(1000);
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));

        PlayerUnit unit2 = new PlayerUnit(0, new UnitId(7), Vector3.one, Potato.UnitDirection.Left, null);
        Assert.IsNotNull(unit2);
        Assert.AreEqual(new UnitId(7), unit2.UnitId);
        Assert.That(Vector3.one, Is.AreApproximatelyEqual(unit2.Position));
        Assert.AreEqual(Potato.UnitDirection.Left, unit2.Direction);
    }

    [Test]
    public void PlayerUnitMoveAndStop()
    {
        PlayerUnit unit1 = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(1000);
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
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
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(2000);
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(2500);
        Assert.That(new Vector3(0.5f, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(3000);
        Assert.That(new Vector3(1, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(4000);
        Assert.That(new Vector3(1, 0, 0), Is.AreApproximatelyEqual(unit1.Position));

        unit1.Update(5500);
        Assert.That(new Vector3(1, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
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
        Assert.That(new Vector3(1, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(7000);
        Assert.That(new Vector3(2, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(8000);
        Assert.That(new Vector3(2, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
    }

    [Test]
    public void PlayerUnitMoveAndKnockback()
    {
        PlayerUnit unit1 = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(1000);
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
        unit1.InputMove(new MoveCommand
        {
            From = Vector3.zero,
            To = new Vector3(1000, 0, 0),
            StartTime = 2000,
            Speed = 1 / 1000.0f,
        });
        unit1.InputStop(new StopCommand
        {
            StopTime = 10000,
        });
        unit1.Update(1500);
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(2000);
        Assert.That(Vector3.zero, Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(2500);
        Assert.That(new Vector3(0.5f, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(3000);
        Assert.That(new Vector3(1, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(4000);
        Assert.That(new Vector3(2, 0, 0), Is.AreApproximatelyEqual(unit1.Position));

        unit1.Update(4500);
        Assert.That(new Vector3(2.5f, 0, 0), Is.AreApproximatelyEqual(unit1.Position));

        unit1.InputKnockback(new KnockbackCommand
        {
            From = unit1.Position,
            To = unit1.Position + new Vector3(1000, 0, 0),
            StartTime = 5000,
            EndTime = 6000,
            Speed = 1 / 1000.0f,
        });
        unit1.InputStop(new StopCommand
        {
            StopTime = 6000,
        });
        unit1.Update(5000);
        Assert.That(new Vector3(2.5f, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(5500);
        Assert.That(new Vector3(3, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(6000);
        Assert.That(new Vector3(3.5f, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
        unit1.Update(7000);
        Assert.That(new Vector3(3.5f, 0, 0), Is.AreApproximatelyEqual(unit1.Position));
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

        Assert.IsFalse(move.IsGoaled(long.MinValue));
        Assert.IsFalse(move.IsGoaled(-1));
        Assert.IsFalse(move.IsGoaled(0));
        Assert.IsFalse(move.IsGoaled(100));
        Assert.IsFalse(move.IsGoaled(1000));
        Assert.IsFalse(move.IsGoaled(2000));
        Assert.IsFalse(move.IsGoaled(2098));
        // 2099 is unstable;
        Assert.IsTrue(move.IsGoaled(2100));
        Assert.IsTrue(move.IsGoaled(2101));
        Assert.IsTrue(move.IsGoaled(2102));
        Assert.IsTrue(move.IsGoaled(int.MaxValue));
        Assert.IsTrue(move.IsGoaled(long.MaxValue));

        Assert.IsTrue(move.IsGoaled(move.GetGoalTime()));
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

        Assert.That(move.GetGoalTime(), Is.GreaterThanOrEqualTo(2099).And.LessThanOrEqualTo(2100));
        move.Speed = 1 / 10000.0f;
        Assert.That(move.GetGoalTime(), Is.GreaterThanOrEqualTo(20099).And.LessThanOrEqualTo(20100));
        move.Speed = 1 / 100000.0f;
        Assert.That(move.GetGoalTime(), Is.GreaterThanOrEqualTo(200099).And.LessThanOrEqualTo(200100));
        move.Speed = 1 / 1000000.0f;
        Assert.That(move.GetGoalTime(), Is.GreaterThanOrEqualTo(2000099).And.LessThanOrEqualTo(2000100));
    }
}
