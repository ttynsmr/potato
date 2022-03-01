
public struct UnitId
{
    private readonly ulong value;
    public UnitId(ulong value) { this.value = value; }
    public static explicit operator UnitId(ulong value) { return new UnitId(value); }
}

public interface IUnit
{
    UnitId UnitId { get; }

    void Start();
    void Update();
}
