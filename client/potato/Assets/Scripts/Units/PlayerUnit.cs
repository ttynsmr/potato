using Potato;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerUnit : IUnit, IHasStatus
{
    public UnitView Appearance { get; set; }

    private Queue<ICommand> inputQueue = new();
    private List<ICommand> history = new();
    private MoveCommand currentMove;
    private long simulatedNow = 0;
    private Potato.Avatar avatar;

    public Vector3 Position { get; private set; }

    public UnitId UnitId { get; private set; }
    public UnitService UnitService { get; set; }
    public Potato.UnitDirection Direction { get; set; }
    public Potato.CharacterStatus CharacterStatus { get; set; }

    public ICommand GetLastCommand { get { return history.Count > 0 ? history.Last() : null; } }

    public PlayerUnit(long initialNow, UnitId unitId, Vector3 position, Potato.UnitDirection direction, Potato.Avatar avatar)
    {
        UnitId = unitId;
        Position = position;
        Direction = direction;
        this.avatar = avatar;
        simulatedNow = initialNow;
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
            StopTime = initialNow,
            Direction = 0
        });
    }

    public void Start()
    {
        Appearance = GameObject.Instantiate(UnitService.TestAvatar).GetComponent<UnitView>();
        Appearance.transform.position = Position;
        Appearance.name = $"PlayerUnit({UnitId}) {avatar.Name}";
        Appearance.displayName.text = avatar.Name;
    }

    public void OnDespawn()
    {
        GameObject.Destroy(Appearance.gameObject);
    }

    public void InputMove(MoveCommand moveCommand)
    {
        if (moveCommand.CommandType != CommandType.Move)
        {
            Debug.LogError($"expected command type is {CommandType.Move} actual command type is {moveCommand.CommandType}");
        }
        moveCommand.LastMoveCommand = currentMove;
        inputQueue.Enqueue(moveCommand);
        //Debug.Log($"InputMove {moveCommand.From} {moveCommand.To}.\n");
    }

    public void InputStop(StopCommand stopCommand)
    {
        if (history.Count > 0)
        {
            var lastCommand = history.Last();
            if (lastCommand is MoveCommand command)
            {
                stopCommand.LastMoveCommand = command;
            }
        }
        inputQueue.Enqueue(stopCommand);
    }

    public void InputKnockback(KnockbackCommand knockbackCommand)
    {
        InterveneHistory(knockbackCommand);

        knockbackCommand.LastMoveCommand = currentMove;
        inputQueue.Enqueue(knockbackCommand);

        simulatedNow = knockbackCommand.GetActionTime() - 1;
    }

    // Update is called once per frame
    public void Update(long now)
    {
        ProcessCommand(now);
    }

    private void InterveneHistory(ICommand interveneCommand)
    {
        inputQueue.Clear();

        var index = history.FindIndex((command) => { return command.GetActionTime() >= interveneCommand.GetActionTime(); });
        if (index == -1)
        {
            return;
        }

        history.RemoveRange(index, history.Count - index);
    }

    private void ProcessCommand(long now)
    {
        while (simulatedNow < now)
        {
            if (inputQueue.Count > 0 && inputQueue.Peek().GetActionTime() <= now)
            {
                var command = inputQueue.Peek();
                if (command != currentMove && currentMove != null && currentMove.CommandType == CommandType.Knockback)
                {
                    if (currentMove.IsGoaled(simulatedNow) || (command.GetActionTime() >= currentMove.GetGoalTime()))
                    {
                        // ok
                    }
                    else
                    {
                        // blocked
                        Debug.Log($"knock back!! simulatedNow:{simulatedNow}({currentMove.GetGoalTime() - now}), input dropping until {currentMove.GetGoalTime()}, command action time is {command.GetActionTime()}.\n");
                        simulatedNow = now;
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

        if (currentMove == null)
        {
            var lastCommand = history.Count > 0 ? history.Last() : null;
            if (lastCommand != null)
            {
                if (lastCommand is StopCommand stopCommand && stopCommand.LastMoveCommand != null)
                {
                    Position = stopCommand.LastMoveCommand.CalcCurrentPosition(stopCommand.StopTime);
                }
            }
        }
        else
        {
            Position = currentMove.CalcCurrentPosition(simulatedNow);
        }

        if (Appearance)
        {
            Appearance.transform.position = Position;
            Appearance.Direction = Direction;
            Appearance.Moving = history.Last() is not StopCommand;
        }
    }

    public Vector3 GetTrackbackPosition(long now)
    {
        if (history.Count == 0)
        {
            return Vector3.zero;
        }

        if (now > history.Last().GetActionTime())
        {
            if (history.Last().CommandType == CommandType.Stop)
            {
                return history.Last().LastMoveCommand.CalcCurrentPosition(history.Last().GetActionTime());
            }
            else
            {
                return ((MoveCommand)history.Last()).CalcCurrentPosition(now);
            }
        }

        var commandAtTheTime = history.Find((command) => { return command.GetActionTime() >= now; });
        if (commandAtTheTime == null)
        {
            return Vector3.zero;
        }

        return commandAtTheTime.CalcCurrentPosition(now);
    }

    public void Destroy()
    {
        if (!Appearance)
        {
            return;
        }
        GameObject.Destroy(Appearance.gameObject);
    }
}
