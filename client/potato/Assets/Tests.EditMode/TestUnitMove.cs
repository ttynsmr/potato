using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitMove
{
    [Test]
    public void PlayerUnitGeneration()
    {
        PlayerUnit unit1 = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
        Assert.IsNotNull(unit1);
        Assert.AreEqual(new UnitId(0), unit1.UnitId);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        Assert.AreEqual(Potato.UnitDirection.Down, unit1.Direction);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        unit1.Update(1000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));

        PlayerUnit unit2 = new PlayerUnit(0, new UnitId(7), Vector3.one, Potato.UnitDirection.Left, null);
        Assert.IsNotNull(unit2);
        Assert.AreEqual(new UnitId(7), unit2.UnitId);
        Assert.That(unit2.Position, Is.AreApproximatelyEqual(Vector3.one));
        Assert.AreEqual(Potato.UnitDirection.Left, unit2.Direction);
    }

    [Test]
    public void PlayerUnitMoveAndStop()
    {
        PlayerUnit unit1 = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        unit1.Update(1000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        unit1.InputMove(new MoveCommand
        {
            From = Vector3.zero,
            To = new Vector3(10, 0, 0),
            StartTime = 2000,
            Speed = 1 / 1000.0f,
        });
        unit1.InputStop(new StopCommand
        {
            StopTime = 3000,
        });
        unit1.Update(1500);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        unit1.Update(2000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        unit1.Update(2500);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(0.5f, 0, 0)));
        unit1.Update(3000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
        unit1.Update(4000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));

        unit1.Update(5500);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
        unit1.InputMove(new MoveCommand
        {
            From = Vector3.zero,
            To = new Vector3(10, 0, 0),
            StartTime = 5000,
            Speed = 1 / 1000.0f,
        });
        unit1.InputStop(new StopCommand
        {
            StopTime = 7000,
        });
        unit1.Update(6000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
        unit1.Update(7000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(2, 0, 0)));
        unit1.Update(8000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(2, 0, 0)));
    }

    [Test]
    public void PlayerUnitMoveAndKnockback()
    {
        PlayerUnit unit1 = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        unit1.Update(1000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        unit1.InputMove(new MoveCommand
        {
            From = Vector3.zero,
            To = new Vector3(1000, 0, 0),
            StartTime = 2000,
            Speed = 1 / 1000.0f,
        });
        unit1.InputStop(new StopCommand
        {
            StopTime = 10000,
        });
        unit1.Update(1500);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        unit1.Update(2000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
        unit1.Update(2500);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(0.5f, 0, 0)));
        unit1.Update(3000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
        unit1.Update(4000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(2, 0, 0)));

        unit1.Update(4500);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(2.5f, 0, 0)));

        unit1.InputKnockback(new KnockbackCommand
        {
            From = unit1.Position,
            To = unit1.Position + new Vector3(1000, 0, 0),
            StartTime = 5000,
            EndTime = 6000,
            Speed = 1 / 1000.0f,
        });
        unit1.InputStop(new StopCommand
        {
            StopTime = 6000,
        });
        unit1.Update(5000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(2.5f, 0, 0)));
        unit1.Update(5500);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(3, 0, 0)));
        unit1.Update(6000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(3.5f, 0, 0)));
        unit1.Update(7000);
        Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(3.5f, 0, 0)));
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(1648084343000)]
    [TestCase(4475783471000)]
    public void PlayerUnitGetTrackbackPositionWithStop(long timeOffset)
    {
        static PlayerUnit createUnit(long timeOffset)
        {
            var unit = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
            unit.InputMove(new MoveCommand
            {
                From = Vector3.zero,
                To = new Vector3(1000, 0, 0),
                StartTime = timeOffset + 1,
                Speed = 1,
            });
            unit.InputStop(new StopCommand
            {
                StopTime = timeOffset + 2,
            });
            unit.InputMove(new MoveCommand
            {
                From = new Vector3(1, 0, 0),
                To = new Vector3(1, 1000, 0),
                StartTime = timeOffset + 3,
                Speed = 1,
            });
            unit.InputStop(new StopCommand
            {
                StopTime = timeOffset + 4,
            });
            return unit;
        }

        {
            PlayerUnit unit1 = createUnit(timeOffset);
            unit1.Update(timeOffset + 0);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
            unit1.Update(timeOffset + 1);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
            unit1.Update(timeOffset + 2);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            unit1.Update(timeOffset + 3);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            unit1.Update(timeOffset + 4);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
            unit1.Update(timeOffset + 5);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
        }

        {
            PlayerUnit unit1 = createUnit(timeOffset);
            for (int i = 0; i <= 5; i++)
            {
                Assert.That(unit1.GetTrackbackPosition(timeOffset + i), Is.AreApproximatelyEqual(Vector3.zero));
            }
        }

        {
            PlayerUnit unit1 = createUnit(timeOffset);
            unit1.Update(timeOffset + 4);
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 0), Is.AreApproximatelyEqual(Vector3.zero));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 1), Is.AreApproximatelyEqual(Vector3.zero));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 2), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 3), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 4), Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 5), Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
        }
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(1648084343000)]
    [TestCase(4475783471000)]
    public void PlayerUnitGetTrackbackPositionWithMove(long timeOffset)
    {
        static PlayerUnit createUnit(long timeOffset)
        {
            var unit = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
            unit.InputMove(new MoveCommand
            {
                From = Vector3.zero,
                To = new Vector3(1000, 0, 0),
                StartTime = timeOffset + 1,
                Speed = 1,
            });
            unit.InputStop(new StopCommand
            {
                StopTime = timeOffset + 2,
            });
            unit.InputMove(new MoveCommand
            {
                From = new Vector3(1, 0, 0),
                To = new Vector3(1, 1000, 0),
                StartTime = timeOffset + 3,
                Speed = 1,
            });
            return unit;
        }

        {
            PlayerUnit unit1 = createUnit(timeOffset);
            unit1.Update(timeOffset + 0);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
            unit1.Update(timeOffset + 1);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
            unit1.Update(timeOffset + 2);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            unit1.Update(timeOffset + 3);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            unit1.Update(timeOffset + 4);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
            unit1.Update(timeOffset + 5);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 2, 0)));
        }

        {
            PlayerUnit unit1 = createUnit(timeOffset);
            for (int i = 0; i <= 5; i++)
            {
                Assert.That(unit1.GetTrackbackPosition(timeOffset + i), Is.AreApproximatelyEqual(Vector3.zero));
            }
        }

        {
            PlayerUnit unit1 = createUnit(timeOffset);
            unit1.Update(timeOffset + 4);
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 0), Is.AreApproximatelyEqual(Vector3.zero));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 1), Is.AreApproximatelyEqual(Vector3.zero));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 2), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 3), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 4), Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 5), Is.AreApproximatelyEqual(new Vector3(1, 2, 0)));
        }
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(1648084343000)]
    [TestCase(4475783471000)]
    public void PlayerUnitGetTrackbackPositionWithKnockback(long timeOffset)
    {
        static PlayerUnit createUnit(long timeOffset)
        {
            var unit = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
            unit.InputMove(new MoveCommand
            {
                From = Vector3.zero,
                To = new Vector3(1000, 0, 0),
                StartTime = timeOffset + 1,
                Speed = 1,
            });
            unit.InputStop(new StopCommand
            {
                StopTime = timeOffset + 2,
            });
            unit.InputMove(new MoveCommand
            {
                From = new Vector3(1, 0, 0),
                To = new Vector3(1, 1000, 0),
                StartTime = timeOffset + 3,
                Speed = 1,
            });
            unit.InputStop(new StopCommand
            {
                StopTime = timeOffset + 10,
            });
            return unit;
        }

        {
            PlayerUnit unit1 = createUnit(timeOffset);
            unit1.Update(timeOffset + 0);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
            unit1.Update(timeOffset + 1);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
            unit1.Update(timeOffset + 2);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            unit1.Update(timeOffset + 3);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            unit1.Update(timeOffset + 4);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
            unit1.Update(timeOffset + 5);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 2, 0)));

            unit1.InputKnockback(new KnockbackCommand
            {
                From = new Vector3(1, 1, 0),
                To = new Vector3(1, -1000, 0),
                StartTime = timeOffset + 4,
                EndTime = timeOffset + 6,
                Speed = 1,
            });
            unit1.InputStop(new StopCommand
            {
                StopTime = timeOffset + 6,
            });

            unit1.Update(timeOffset + 6);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, -1, 0)));
            unit1.Update(timeOffset + 7);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, -1, 0)));

            Assert.That(unit1.GetTrackbackPosition(timeOffset + 0), Is.AreApproximatelyEqual(Vector3.zero));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 1), Is.AreApproximatelyEqual(Vector3.zero));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 2), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 3), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 4), Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 5), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 6), Is.AreApproximatelyEqual(new Vector3(1, -1, 0)));
            Assert.That(unit1.GetTrackbackPosition(timeOffset + 7), Is.AreApproximatelyEqual(new Vector3(1, -1, 0)));
        }
    }

    [Test]
    public void PlayerUnitGetTrackbackPositionWithMoveStopMoveKnockback()
    {
        static PlayerUnit createUnit()
        {
            var unit = new PlayerUnit(0, new UnitId(0), Vector3.zero, Potato.UnitDirection.Down, null);
            unit.InputMove(new MoveCommand
            {
                From = Vector3.zero,
                To = new Vector3(1000, 0, 0),
                StartTime = 1,
                Speed = 1,
            });
            unit.InputStop(new StopCommand
            {
                StopTime = 2,
            });
            unit.InputMove(new MoveCommand
            {
                From = new Vector3(1, 0, 0),
                To = new Vector3(1, 1000, 0),
                StartTime = 3,
                Speed = 1,
            });
            return unit;
        }

        {
            PlayerUnit unit1 = createUnit();
            unit1.Update(0);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
            unit1.Update(1);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(Vector3.zero));
            unit1.Update(2);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            unit1.Update(3);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            unit1.Update(4);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
            unit1.Update(5);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, 2, 0)));

            unit1.InputKnockback(new KnockbackCommand
            {
                From = new Vector3(1, 1, 0),
                To = new Vector3(1, -1000, 0),
                StartTime = 4,
                EndTime = 6,
                Speed = 1,
            });
            unit1.InputStop(new StopCommand
            {
                StopTime = 6,
            });

            unit1.Update(6);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, -1, 0)));
            unit1.Update(7);
            Assert.That(unit1.Position, Is.AreApproximatelyEqual(new Vector3(1, -1, 0)));

            Assert.That(unit1.GetTrackbackPosition(0), Is.AreApproximatelyEqual(Vector3.zero));
            Assert.That(unit1.GetTrackbackPosition(1), Is.AreApproximatelyEqual(Vector3.zero));
            Assert.That(unit1.GetTrackbackPosition(2), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(3), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(4), Is.AreApproximatelyEqual(new Vector3(1, 1, 0)));
            Assert.That(unit1.GetTrackbackPosition(5), Is.AreApproximatelyEqual(new Vector3(1, 0, 0)));
            Assert.That(unit1.GetTrackbackPosition(6), Is.AreApproximatelyEqual(new Vector3(1, -1, 0)));
            Assert.That(unit1.GetTrackbackPosition(7), Is.AreApproximatelyEqual(new Vector3(1, -1, 0)));
        }
    }
}
