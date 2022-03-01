using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : IUnit
{
    public UnitId UnitId { get; private set; }

    public PlayerUnit(UnitId unitId, Vector3 position, float direction)
    {
        UnitId = unitId;
    }

    public void Start()
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
