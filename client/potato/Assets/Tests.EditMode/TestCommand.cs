using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestCommand
{
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
