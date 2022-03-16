using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class ControllablePlayerUnit : IUnit
{
    public UnitView Appearance { get; set; }

    private Potato.Network.NetworkService _networkService;
    private int prevInputX;
    private int prevInputY;
    private float moveSpeed = 0.0025f;

    private bool prevAttackButton = false;

    private Queue<ICommand> inputQueue = new Queue<ICommand>();
    private List<ICommand> history = new List<ICommand>();

    private MoveCommand currentMove;


    public Vector3 Position { get; private set; }

    public UnitId UnitId { get; private set; }
    public UnitService UnitService { get; set; }
    public Potato.UnitDirection Direction { get; set; }

    public ControllablePlayerUnit(Potato.Network.NetworkService networkService, UnitId unitId, Vector3 position, Potato.UnitDirection direction, Potato.Avatar avatar)
    {
        _networkService = networkService;
        UnitId = unitId;
        Direction = direction;
    }

    public void Start()
    {
        Appearance = GameObject.Instantiate(UnitService.TestAvatar).GetComponent<UnitView>();
        Appearance.name = $"ControllablePlayerUnit({UnitId})";
        history.Add(new StopCommand
        {
            LastMoveCommand = new MoveCommand {
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

    Vector3 CalcCurrentPosition(MoveCommand currentMove, long now)
    {
        var distance = (currentMove.To - currentMove.From).magnitude;
        var progress = (now - currentMove.StartTime) / (distance / currentMove.Speed);
        //Debug.Log($"distance:{distance}, progress:{progress}, estimate time:{(distance / currentMove.Speed)}");
        return Vector3.Lerp(currentMove.From, currentMove.To, progress);
    }

    private ulong moveId;
    private void ProcessInput(long now)
    {
        int moveX = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        int moveY = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        if (inputQueue.Count > 0)
        {
            var command = inputQueue.Peek();
            if (command.GetActionTime() <= now)
            {
                switch (command.CommandType)
                {
                    case CommandType.Knockback:
                        currentMove = command as MoveCommand;
                        break;
                    case CommandType.Stop:
                        var stopCommand = command as StopCommand;
                        stopCommand.LastMoveCommand = currentMove;
                        currentMove = null;
                        break;
                }
                history.Add(command);
                inputQueue.Dequeue();
            }
        }

        if (currentMove != null && currentMove.CommandType == CommandType.Knockback)
        {
            if (currentMove.IsGoaled(now))
            {
                // ok
            }
            else
            {
                // blocked
                if (moveX != 0 || moveY != 0)
                {
                    Debug.Log($"knock back!! input dropping until {currentMove.GetGoalTime()}, command action time is {now}.\n");
                }
                return;
            }
        }

        var moveDirection = new Vector2(moveX, moveY).normalized;

        if (prevInputX != moveX || prevInputY != moveY)
        {
            ICommand command;

            // update move or stop
            if (moveX == 0 && moveY == 0)
            {
                // stop
                var stopCommand = new StopCommand
                {
                    LastMoveCommand = (MoveCommand)history.Last(),
                    StopTime = now,
                    Direction = 0
                };
                command = stopCommand;
                currentMove = null;
                var currentMoveCommand = (MoveCommand)history.Last();
                var unitStop = _networkService.Session.GetRpc<Torikime.Unit.Stop.Rpc>();
                unitStop.Request(new Torikime.Unit.Stop.Request
                {
                    UnitId = UnitId.RawValue,
                    Time = currentMoveCommand.StartTime,
                    StopTime = now,
                    Direction = Direction,
                    MoveId = ++moveId,
                }, (response) => {
                    Debug.Log($"Request Stop {response.Ok}");
                });;
                Debug.Log("Request Stop");
                var expectedStopPosition = CalcCurrentPosition(stopCommand.LastMoveCommand, stopCommand.StopTime);
                Debug.Log($"expected stop [{moveId}] stop time:{now} position: ({expectedStopPosition.x}, {expectedStopPosition.y}, {expectedStopPosition.z})");
            }
            else
            {
                if (moveY == 0)
                {
                    Direction = moveX > 0 ? Potato.UnitDirection.Right : Potato.UnitDirection.Left;
                }
                else
                {
                    Direction = moveY < 0 ? Potato.UnitDirection.Down : Potato.UnitDirection.Up;
                }

                // change move
                MoveCommand moveCommand = new MoveCommand
                {
                    LastMoveCommand = currentMove,
                    StartTime = now,
                    From = Position,
                    To = (Position + new Vector3(moveDirection.x, moveDirection.y) * 1000.0f),
                    Direction = Direction,
                    Speed = moveSpeed
                };
                command = moveCommand;
                currentMove = moveCommand;
                var unitMove = _networkService.Session.GetRpc<Torikime.Unit.Move.Rpc>();
                unitMove.Request(new Torikime.Unit.Move.Request
                {
                    UnitId = UnitId.RawValue,
                    Time = moveCommand.StartTime,
                    From = moveCommand.From.ToVector3(),
                    To = moveCommand.To.ToVector3(),
                    Speed = moveCommand.Speed,
                    Direction = moveCommand.Direction,
                    MoveId = ++moveId,
                }, (response) =>
                {
                    Debug.Log($"Request Move {response.Ok}");
                });
                Debug.Log($"Request Move [{moveId}]");
            }

            history.Add(command);
        }
        prevInputX = moveX;
        prevInputY = moveY;

        if (!prevAttackButton && Input.GetKey(KeyCode.P))
        {
            var context = SynchronizationContext.Current;
            prevAttackButton = true;
            var skillCast = _networkService.Session.GetRpc<Torikime.Battle.SkillCast.Rpc>();
            skillCast.Request(new Torikime.Battle.SkillCast.Request
            {
                SkillId = 0,
                TargetUnitId = 0,
                TriggerTime = now,
            }, (response) =>
            {
                //context.Post((_) =>
                //{
                    Debug.Log($"Request SkillCast {response.Ok} {response.AttackId}");
                //}, null);
            });
        }
        else if(prevAttackButton && !Input.GetKey(KeyCode.P))
        {
            prevAttackButton = false;
        }
    }

    private void ProcessCommand(long now)
    {
        var lastCommand = history.Count > 0 ? history.Last() : null;
        if (currentMove == null)
        {
            if (lastCommand is StopCommand)
            {
                var stopCommand = (StopCommand)lastCommand;
                Position = CalcCurrentPosition(stopCommand.LastMoveCommand, stopCommand.StopTime);
            }
        }
        else
        {
            Position = CalcCurrentPosition(currentMove, now);
        }

        if (Appearance)
        {
            Appearance.transform.position = Position;
            Appearance.Direction = Direction;
            Appearance.Moving = !(lastCommand is StopCommand);
        }
    }

    public void InputMove(MoveCommand _)
    {
        throw new System.NotImplementedException();
    }

    public void InputStop(StopCommand stopCommand)
    {
        stopCommand.LastMoveCommand = currentMove;
        inputQueue.Enqueue(stopCommand);
    }

    public void InputKnockback(KnockbackCommand knockbackCommand)
    {
        knockbackCommand.LastMoveCommand = currentMove;
        inputQueue.Enqueue(knockbackCommand);
    }

    public void Destroy()
    {
        if (Appearance != null)
        {
            GameObject.Destroy(Appearance.gameObject);
        }
    }
}
