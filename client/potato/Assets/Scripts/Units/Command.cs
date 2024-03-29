using System;
using UnityEngine;

public enum CommandType
{
    Stop,
    Move,
    Knockback
}

public interface ICommand
{
    abstract CommandType CommandType { get; }
    abstract MoveCommand LastMoveCommand { get; set; }

    abstract bool IsExpired(long now);

    abstract long GetActionTime();

    abstract Vector3 CalcCurrentPosition(long now);
}

public class MoveCommand : ICommand
{
    public virtual CommandType CommandType { get => CommandType.Move; }

    public MoveCommand LastMoveCommand { get; set; }

    public long StartTime;
    public Vector3 From;
    public Vector3 To;
    public float Speed;
    public Potato.UnitDirection Direction;

    public bool IsExpired(long now)
    {
        throw new System.NotImplementedException();
    }

    public long GetActionTime()
    {
        return StartTime;
    }

    public bool IsGoaled(long now)
	{
        return GetGoalTime() <= now;
	}

    virtual public long GetGoalTime()
    {
        var t = ((To - From).magnitude / (double)Speed) + StartTime;
        return (long)t;
    }

    public override string ToString()
    {
        var lastStartTime = LastMoveCommand != null ? LastMoveCommand.StartTime : 0;
        return $"last command: {lastStartTime}, StartTime:{StartTime}, From:{From}, To:{To}, Speed:{Speed}, Direction:{Direction}";
    }

    public virtual Vector3 CalcCurrentPosition(long now)
    {
        var distance = (To - From).magnitude;
        var progress = Speed > 0 && distance > 0 ? (now - StartTime) / (distance / Speed) : 0;
        return Vector3.Lerp(From, To, progress);
    }
}

public class KnockbackCommand : MoveCommand
{
    public override CommandType CommandType { get => CommandType.Knockback; }

    public override long GetGoalTime()
    {
        return EndTime;
    }
    public long EndTime;

    public override Vector3 CalcCurrentPosition(long now)
    {
        var distance = (To - From).magnitude;
        var progress = Speed > 0 && distance > 0 ? (Math.Min(EndTime, now) - StartTime) / (distance / Speed) : 0;
        return Vector3.Lerp(From, To, progress);
    }
}

public class StopCommand : ICommand
{
    public CommandType CommandType { get => CommandType.Stop; }
    public MoveCommand LastMoveCommand { get; set; }

    public long StopTime;
    public Potato.UnitDirection Direction;

    public bool IsExpired(long now)
    {
        throw new System.NotImplementedException();
    }

    public long GetActionTime()
    {
        return StopTime;
    }

    public Vector3 CalcCurrentPosition(long now)
    {
        var distance = (LastMoveCommand.To - LastMoveCommand.From).magnitude;
        var progress = LastMoveCommand.Speed > 0 && distance > 0 ? (Math.Min(StopTime, now) - LastMoveCommand.StartTime) / (distance / LastMoveCommand.Speed) : 0;
        return Vector3.Lerp(LastMoveCommand.From, LastMoveCommand.To, progress);
    }

    public override string ToString()
    {
        var lastStartTime = LastMoveCommand != null ? LastMoveCommand.StartTime : 0;
        return $"last command: {lastStartTime}, StopTime:{StopTime}, Direction:{Direction}";
    }
}
