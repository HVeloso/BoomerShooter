using System;
using UnityEngine;

public interface IMovementInputHandler
{
    public Vector2 MovementDirection { get; }

    public event Action<bool> JumpPressed;
}
