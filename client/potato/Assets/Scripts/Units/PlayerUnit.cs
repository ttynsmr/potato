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

    public Vector3 Positoin { get; private set; }

    public UnitId UnitId { get; private set; }
    public UnitService UnitService { get; set; }

    public PlayerUnit(Potato.Network.NetworkService networkService, UnitId unitId, Vector3 position, float direction, Potato.Avatar avatar)
    {
        _networkService = networkService;
        UnitId = unitId;
    }

    public void Start()
    {
        simulatedNow = _networkService.Now;
        Appearance = GameObject.Instantiate(UnitService.TestAvatar);
        history.Add( new StopCommand
        {
            LastMoveCommand = null,
            StopTime = simulatedNow,
            Direction = 0
        });
    }

    public void InputMove(Torikime.Unit.Move.Notification notification)
    {
        inputQueue.Enqueue(new MoveCommand {
            StartTime = notification.Time,
            From = notification.From.ToVector3(),
            To = notification.To.ToVector3(),
            Speed = notification.Speed,
            Direction = 0,
        });
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

    void ProcessCommand(long now)
    {
        while (simulatedNow < now)
        {
            var lastCommand = history.Count > 0 ? history.Last() : null;
            if (currentMove == null)
            {
                if (lastCommand is StopCommand)
                {
                    return;
                }

                currentMove = (MoveCommand)history.Last();
            }

            var distance = (currentMove.To - currentMove.From).magnitude;
            var progress = (simulatedNow - currentMove.StartTime) / (distance / currentMove.Speed);
            Debug.Log($"distance:{distance}, progress:{progress}, estimate time:{(distance / currentMove.Speed)}");
            Positoin = Vector3.Lerp(currentMove.From, currentMove.To, progress);

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

        if (Appearance)
        {
            Appearance.transform.position = Positoin;
        }
    }
}
