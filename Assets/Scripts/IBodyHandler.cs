using UnityEngine;

public interface IBodyHandler
{
    public void SetMovementDirection(Vector3 velocity);
    public void UpdateJumpInput(bool isJumpPressed);
}
