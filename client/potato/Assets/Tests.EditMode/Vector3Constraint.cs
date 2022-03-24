using NUnit.Framework.Constraints;
using UnityEngine;

public class Vector3Constraint : Constraint
{
    private Vector3 _expected;
    private float _slack;

    public Vector3Constraint(Vector3 expected, float slack)
    {
        _expected = expected;
        _slack = slack;
        Description = expected.ToString();
    }

    public override ConstraintResult ApplyTo(object actual)
    {
        return new ConstraintResult(this, actual, (_expected - (Vector3)actual).magnitude < _slack);
    }
}

public class Is : NUnit.Framework.Is
{
    public static Vector3Constraint AreApproximatelyEqual(Vector3 expected, float slack = 0.00001f)
    {
        return new Vector3Constraint(expected, slack);
    }
}
