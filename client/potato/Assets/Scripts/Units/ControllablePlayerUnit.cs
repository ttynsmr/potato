using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Torikime.Unit.Move;
using UnityEngine;

public class ControllablePlayerUnit : IUnit
{
    public GameObject Appearance { get; set; }

    private Potato.Network.NetworkService _networkService;
    private int prevInputX;
    private int prevInputY;
    private float moveSpeed = 0.005f;

    private List<ICommand> history = new List<ICommand>();

    private MoveCommand currentMove;


    public Vector3 Position { get; private set; }

    public UnitId UnitId { get; private set; }
    public UnitService UnitService { get; set; }

    public ControllablePlayerUnit(Potato.Network.NetworkService networkService, UnitId unitId, Vector3 position, float direction, Potato.Avatar avatar)
    {
        _networkService = networkService;
        UnitId = unitId;
    }

    public void Start()
    {
        Appearance = GameObject.Instantiate(UnitService.TestAvatar);
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
                    Direction = 0,
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
                // change move
                MoveCommand moveCommand = new MoveCommand
                {
                    LastMoveCommand = currentMove,
                    StartTime = now,
                    From = Position,
                    To = (Position + new Vector3(moveDirection.x, moveDirection.y) * 1000.0f),
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
                    MoveId = ++moveId,
                }, (response) => {
                    Debug.Log($"Request Move {response.Ok}");
                }); ;
                Debug.Log($"Request Move [{moveId}]");
            }

            history.Add(command);
        }
        prevInputX = moveX;
        prevInputY = moveY;
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
        }
    }

    public void InputMove(MoveCommand _)
    {
        throw new System.NotImplementedException();
    }

    public void InputStop(StopCommand _)
    {
        throw new System.NotImplementedException();
    }
}
