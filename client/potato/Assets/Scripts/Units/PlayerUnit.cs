using Potato;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerUnit : IUnit
{
    public GameObject Appearance { get; set; }

    private Potato.Network.NetworkService _networkService;
    private Queue<ICommand> inputQueue = new Queue<ICommand>();
    private List<ICommand> history = new List<ICommand>();
    private MoveCommand currentMove;
    private long simulatedNow = 0;

    public Vector3 Position { get; private set; }

    public UnitId UnitId { get; private set; }
    public UnitService UnitService { get; set; }

    public PlayerUnit(Potato.Network.NetworkService networkService, UnitId unitId, Vector3 position, float direction, Potato.Avatar avatar)
    {
        _networkService = networkService;
        UnitId = unitId;
        Position = position;
    }

    public void Start()
    {
        simulatedNow = _networkService.Now;
        Appearance = GameObject.Instantiate(UnitService.TestAvatar);
        Appearance.transform.position = Position;
        Appearance.name = $"PlayerUnit({UnitId})";
        history.Add(new StopCommand
        {
            LastMoveCommand = new MoveCommand
            {
                LastMoveCommand = null,
                StartTime = 0,
                From = Position,
                To = Position,
                Direction = 0,
                Speed = 1,
            },
            StopTime = _networkService.Now,
            Direction = 0
        });
    }

    public void OnDespawn()
    {
        GameObject.Destroy(Appearance.gameObject);
    }

    public void InputMove(MoveCommand moveCommand)
    {
        inputQueue.Enqueue(moveCommand);
    }

    public void InputStop(StopCommand stopCommand)
    {
        if (history.Count > 0)
        {
            var lastCommand = history.Last();
            if (lastCommand is MoveCommand)
            {
                stopCommand.LastMoveCommand = (MoveCommand)lastCommand;
            }
        }
        inputQueue.Enqueue(stopCommand);
    }

    // Update is called once per frame
    public void Update(float deltaTime)
    {
        var now = _networkService.Now;
        ProcessInput(now);
        ProcessCommand(now);
        if (currentMove != null)
        {
            Debug.Log(currentMove);
        }
    }

    private void ProcessInput(long now)
    {
    }

    Vector3 CalcCurrentPosition(MoveCommand currentMove, long now)
    {
        var distance = (currentMove.To - currentMove.From).magnitude;
        var progress = (now - currentMove.StartTime) / (distance / currentMove.Speed);
        //Debug.Log($"distance:{distance}, progress:{progress}, estimate time:{(distance / currentMove.Speed)}");
        return Vector3.Lerp(currentMove.From, currentMove.To, progress);
    }

    private void ProcessCommand(long now)
    {
        while (simulatedNow < now)
        {
            if (currentMove == null)
            {
                var lastCommand = history.Count > 0 ? history.Last() : null;
                if (lastCommand != null)
                {
                    if (lastCommand is StopCommand)
                    {
                        var last = history.Last();
                        var stopCommand = (StopCommand)last;
                        Position = CalcCurrentPosition(stopCommand.LastMoveCommand, stopCommand.StopTime);
                    }
                }
            }
            else
            {
                Position = CalcCurrentPosition(currentMove, simulatedNow);
            }

            if (inputQueue.Count > 0)
            {
                var command = inputQueue.Dequeue();
                switch (command)
                {
                    case MoveCommand moveCommand:
                        simulatedNow = moveCommand.StartTime;
                        moveCommand.LastMoveCommand = currentMove;
                        history.Add(moveCommand);
                        currentMove = moveCommand;
                        break;
                    case StopCommand stopCommand:
                        simulatedNow = stopCommand.StopTime;
                        stopCommand.LastMoveCommand = currentMove;
                        history.Add(stopCommand);
                        currentMove = null;
                        break;
                }

            }
            else
            {
                simulatedNow = now;
            }
        }

        //while (simulatedNow < now)
        //{
        //    var lastCommand = history.Count > 0 ? history.Last() : null;
        //    if (currentMove == null)
        //    {
        //        if (lastCommand is StopCommand)
        //        {
        //            return;
        //        }

        //        currentMove = (MoveCommand)history.Last();
        //    }

        //    Positoin = CalcCurrentPosition(currentMove, simulatedNow);

        //    if (inputQueue.Count > 0)
        //    {
        //        var command = inputQueue.Dequeue();
        //        switch (command)
        //        {
        //            case MoveCommand moveCommand:
        //                simulatedNow = moveCommand.StartTime;
        //                moveCommand.LastMoveCommand = currentMove;
        //                history.Add(moveCommand);
        //                currentMove = moveCommand;
        //                break;
        //            case StopCommand stopCommand:
        //                stopCommand.LastMoveCommand = currentMove;
        //                history.Add(stopCommand);
        //                currentMove = null;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        simulatedNow = now;
        //    }
        //}

        if (Appearance)
        {
            Appearance.transform.position = Position;
        }
    }
}
