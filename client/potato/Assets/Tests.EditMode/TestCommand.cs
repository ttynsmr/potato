using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestCommand
{
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(1648084343000)]
    [TestCase(4475783471000)]
    public void MoveCommand(long timeOffset)
    {
        var move = new MoveCommand
        {
            From = new Vector3(0, 0, 0),
            To = new Vector3(2, 0, 0),
            StartTime = timeOffset,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.AreEqual(CommandType.Move, move.CommandType);
        Assert.AreEqual(new Vector3(0, 0, 0), move.CalcCurrentPosition(timeOffset + 0));
        Assert.AreEqual(new Vector3(1, 0, 0), move.CalcCurrentPosition(timeOffset + 1000));
        Assert.AreEqual(new Vector3(2, 0, 0), move.CalcCurrentPosition(timeOffset + 2000));

        move.To = new Vector3(1, 1, 1).normalized * 2;
        Assert.AreEqual(new Vector3(0, 0, 0), move.CalcCurrentPosition(timeOffset + 0));
        Assert.AreEqual(new Vector3(1, 1, 1).normalized * 1, move.CalcCurrentPosition(timeOffset + 1000));
        Assert.AreEqual(new Vector3(1, 1, 1).normalized * 2, move.CalcCurrentPosition(timeOffset + 2000));
        Assert.AreEqual(new Vector3(1, 1, 1).normalized * 2, move.CalcCurrentPosition(timeOffset + 3000));
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
    [TestCase(1 / 1000.0f, 2100, 0)]
    [TestCase(1 / 10000.0f, 20100, 0)]
    [TestCase(1 / 100000.0f, 200100, 0)]
    [TestCase(1 / 1000000.0f, 2000100, 0)]
    [TestCase(1 / 1000.0f, 2100, 5)]
    [TestCase(1 / 10000.0f, 20100, 5)]
    [TestCase(1 / 100000.0f, 200100, 5)]
    [TestCase(1 / 1000000.0f, 2000100, 5)]
    [TestCase(1 / 1000.0f, 2100, 1648084343000)]
    [TestCase(1 / 10000.0f, 20100, 1648084343000)]
    [TestCase(1 / 100000.0f, 200100, 1648084343000)]
    [TestCase(1 / 1000000.0f, 2000100, 1648084343000)]
    [TestCase(1 / 1000.0f, 2100, 4475783471000)]
    [TestCase(1 / 10000.0f, 20100, 4475783471000)]
    [TestCase(1 / 100000.0f, 200100, 4475783471000)]
    [TestCase(1 / 1000000.0f, 2000100, 4475783471000)]
    public void MoveCommandGoalTime(float speed, long expectTime, long timeOffset)
    {
        var move = new MoveCommand
        {
            From = new Vector3(1, 0, 0),
            To = new Vector3(3, 0, 0),
            StartTime = timeOffset + 100,
            Speed = speed,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.That(move.GetGoalTime(), Is.GreaterThanOrEqualTo(timeOffset + expectTime - 1).And.LessThanOrEqualTo(timeOffset + expectTime));
    }

    [Test]
    public void StopCommand()
    {
        var move = new MoveCommand
        {
            From = new Vector3(0, 0, 0),
            To = new Vector3(2, 0, 0),
            StartTime = 0,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };
        var stop = new StopCommand
        {
            LastMoveCommand = move,
            StopTime = 1000,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.AreEqual(CommandType.Stop, stop.CommandType);
        Assert.AreEqual(new Vector3(0, 0, 0), stop.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 0, 0), stop.CalcCurrentPosition(1000));
        Assert.AreEqual(new Vector3(1, 0, 0), stop.CalcCurrentPosition(2000));

        var stopAsICommand = stop as ICommand;
        Assert.AreEqual(CommandType.Stop, stopAsICommand.CommandType);
        Assert.AreEqual(new Vector3(0, 0, 0), stopAsICommand.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 0, 0), stopAsICommand.CalcCurrentPosition(1000));
        Assert.AreEqual(new Vector3(1, 0, 0), stopAsICommand.CalcCurrentPosition(2000));
    }

    [Test]
    public void KnockbackCommand()
    {
        var knockback = new KnockbackCommand
        {
            From = new Vector3(0, 0, 0),
            To = new Vector3(2, 0, 0),
            StartTime = 0,
            EndTime = 1000,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.AreEqual(CommandType.Knockback, knockback.CommandType);
        Assert.AreEqual(new Vector3(0, 0, 0), knockback.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 0, 0), knockback.CalcCurrentPosition(1000));
        Assert.AreEqual(new Vector3(1, 0, 0), knockback.CalcCurrentPosition(2000));

        var knockbackAsICommand = knockback as ICommand;
        Assert.AreEqual(CommandType.Knockback, knockbackAsICommand.CommandType);
        Assert.AreEqual(new Vector3(0, 0, 0), knockbackAsICommand.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 0, 0), knockbackAsICommand.CalcCurrentPosition(1000));
        Assert.AreEqual(new Vector3(1, 0, 0), knockbackAsICommand.CalcCurrentPosition(2000));

        var knockbackAsMove = knockback as MoveCommand;
        Assert.AreEqual(CommandType.Knockback, knockbackAsMove.CommandType);
        Assert.AreEqual(new Vector3(0, 0, 0), knockbackAsMove.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 0, 0), knockbackAsMove.CalcCurrentPosition(1000));
        Assert.AreEqual(new Vector3(1, 0, 0), knockbackAsMove.CalcCurrentPosition(2000));
    }

    [Test]
    public void KnockbackCommandAndStop()
    {
        var knockback = new KnockbackCommand
        {
            From = new Vector3(0, 0, 0),
            To = new Vector3(2, 0, 0),
            StartTime = 0,
            EndTime = 1000,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };
        var stop = new StopCommand
        {
            LastMoveCommand = knockback,
            StopTime = 1000,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.AreEqual(CommandType.Stop, stop.CommandType);
        Assert.AreEqual(new Vector3(0, 0, 0), stop.CalcCurrentPosition(0));
        Assert.AreEqual(new Vector3(1, 0, 0), stop.CalcCurrentPosition(1000));
        Assert.AreEqual(new Vector3(1, 0, 0), stop.CalcCurrentPosition(2000));
    }

    [Test]
    [TestCase(2, 0, 0)]
    [TestCase(1000, 0, 0)]
    [TestCase(2000, 0, 0)]
    public void KnockbackCommandGoaled(float toX, float toY, float toZ)
    {
        var knockback = new KnockbackCommand
        {
            From = new Vector3(1, 0, 0),
            To = new Vector3(toX, toY, toZ),
            StartTime = 100,
            EndTime = 2100,
            Speed = 1 / 1000.0f,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.IsFalse(knockback.IsGoaled(long.MinValue));
        Assert.IsFalse(knockback.IsGoaled(-1));
        Assert.IsFalse(knockback.IsGoaled(0));
        Assert.IsFalse(knockback.IsGoaled(100));
        Assert.IsFalse(knockback.IsGoaled(1000));
        Assert.IsFalse(knockback.IsGoaled(2000));
        Assert.IsFalse(knockback.IsGoaled(2098));
        // 2099 is unstable;
        Assert.IsTrue(knockback.IsGoaled(2100));
        Assert.IsTrue(knockback.IsGoaled(2101));
        Assert.IsTrue(knockback.IsGoaled(2102));
        Assert.IsTrue(knockback.IsGoaled(int.MaxValue));
        Assert.IsTrue(knockback.IsGoaled(long.MaxValue));

        Assert.IsTrue(knockback.IsGoaled(knockback.GetGoalTime()));
    }

    [Test]
    [TestCase(1 / 1000.0f, 2100)]
    [TestCase(1 / 10000.0f, 20100)]
    [TestCase(1 / 100000.0f, 200100)]
    [TestCase(1 / 1000000.0f, 2000100)]
    public void KnockbackCommandGoalTime(float speed, long expectTime)
    {
        var move = new KnockbackCommand
        {
            From = new Vector3(1, 0, 0),
            To = new Vector3(3, 0, 0),
            StartTime = 100,
            EndTime = expectTime,
            Speed = speed,
            Direction = Potato.UnitDirection.Down,
        };

        Assert.That(move.GetGoalTime(), Is.GreaterThanOrEqualTo(expectTime - 1).And.LessThanOrEqualTo(expectTime));
    }
}
