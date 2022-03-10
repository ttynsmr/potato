public struct UnitId
{
    private readonly ulong value;
    public UnitId(ulong value) { this.value = value; }
    public static explicit operator UnitId(ulong value) { return new UnitId(value); }
    public ulong RawValue => value;

    public override string ToString()
    {
        return value.ToString();
    }
}

public interface IUnit
{
    UnitId UnitId { get; }
    UnitService UnitService { get; set; }
    Potato.UnitDirection Direction { get; set; }

    void Start();
    void Update(float deltaTime);
    void InputMove(MoveCommand moveCommand);
    void InputStop(StopCommand stopCommand);
    void OnDespawn();
}
