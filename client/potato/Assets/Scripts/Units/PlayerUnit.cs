using Potato;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerUnit : IUnit
{
    public UnitView Appearance { get; set; }

    private Potato.Network.NetworkService _networkService;
    private Queue<ICommand> inputQueue = new Queue<ICommand>();
    private List<ICommand> history = new List<ICommand>();
    private MoveCommand currentMove;
    private long simulatedNow = 0;
    private Potato.Avatar avatar;

    public Vector3 Position { get; private set; }

    public UnitId UnitId { get; private set; }
    public UnitService UnitService { get; set; }
    public Potato.UnitDirection Direction { get; set; }

    public PlayerUnit(Potato.Network.NetworkService networkService, UnitId unitId, Vector3 position, Potato.UnitDirection direction, Potato.Avatar avatar)
    {
        _networkService = networkService;
        UnitId = unitId;
        Position = position;
        Direction = direction;
        this.avatar = avatar;
    }

    public void Start()
    {
        simulatedNow = _networkService.Now;
        Appearance = GameObject.Instantiate(UnitService.TestAvatar).GetComponent<UnitView>();
        Appearance.transform.position = Position;
        Appearance.name = $"PlayerUnit({UnitId}) {avatar.Name}";
        Appearance.displayName.text = avatar.Name;
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
        moveCommand.LastMoveCommand = currentMove;
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

    public void InputKnockback(KnockbackCommand knockbackCommand)
    {
        knockbackCommand.LastMoveCommand = currentMove;
        inputQueue.Enqueue(knockbackCommand);
    }

    // Update is called once per frame
    public void Update(long now)
    {
        ProcessInput(now);
        ProcessCommand(now);
        if (currentMove != null)
        {
            //Debug.Log(currentMove);
        }
    }

    private void ProcessInput(long now)
    {
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
                        Position = stopCommand.LastMoveCommand.CalcCurrentPosition(stopCommand.StopTime);
                        if (inputQueue.Count == 0)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                Position = currentMove.CalcCurrentPosition(simulatedNow);
            }

            if (inputQueue.Count > 0 && inputQueue.Peek().GetActionTime() <= now)
            {
                var command = inputQueue.Peek();
                if (command.GetActionTime() > simulatedNow)
                {
                    simulatedNow = now;
                    break;
                }

                if (currentMove != null && currentMove.CommandType == CommandType.Knockback)
                {
                    if (currentMove.IsGoaled(simulatedNow) || (command.GetActionTime() >= currentMove.GetGoalTime()))
                    {
                        // ok
                    }
                    else
                    {
                        // blocked
                        Debug.Log($"knock back!! input dropping until {currentMove.GetGoalTime()}, command action time is {command.GetActionTime()}.\n");
                        break;
                    }
                }
                inputQueue.Dequeue();

                switch (command.CommandType)
                {
                    case CommandType.Move:
                        {
                            var moveCommand = command as MoveCommand;
                            simulatedNow = moveCommand.StartTime;
                            moveCommand.LastMoveCommand = currentMove;
                            history.Add(moveCommand);
                            currentMove = moveCommand;
                            Direction = moveCommand.Direction;
                        }
                        break;
                    case CommandType.Knockback:
                        {
                            var knockbackCommand = command as KnockbackCommand;
                            simulatedNow = knockbackCommand.StartTime;
                            knockbackCommand.LastMoveCommand = currentMove;
                            history.Add(knockbackCommand);
                            currentMove = knockbackCommand;
                            Direction = knockbackCommand.Direction;
                        }
                        break;
                    case CommandType.Stop:
                        {
                            var stopCommand = command as StopCommand;
                            simulatedNow = stopCommand.StopTime;
                            stopCommand.LastMoveCommand = currentMove;
                            Direction = stopCommand.Direction;
                            history.Add(stopCommand);
                            currentMove = null;
                        }
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
            Appearance.transform.position = Position;
            Appearance.Direction = Direction;
            Appearance.Moving = !(history.Last() is StopCommand);
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(Appearance.gameObject);
    }
}
