using UnityEngine;

interface ICommand { }

class MoveCommand : ICommand
{
    public MoveCommand LastMoveCommand;
    public long StartTime;
    public Vector3 From;
    public Vector3 To;
    public float Speed;
    public float Direction;

    public override string ToString()
    {
        var lastStartTime = LastMoveCommand != null ? LastMoveCommand.StartTime : 0;
        return $"last command: {lastStartTime}, StartTime:{StartTime}, From:{From}, To:{To}, Speed:{Speed}, Direction:{Direction}";
    }
}

class StopCommand : ICommand
{
    public MoveCommand LastMoveCommand;
    public long StopTime;
    public float Direction;

    public override string ToString()
    {
        var lastStartTime = LastMoveCommand != null ? LastMoveCommand.StartTime : 0;
        return $"last command: {lastStartTime}, StopTime:{StopTime}, Direction:{Direction}";
    }
}
