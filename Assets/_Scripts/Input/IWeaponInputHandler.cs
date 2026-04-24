using System;

public interface IWeaponInputHandler
{
    public event Action<bool> AttackInputed;
    public event Action<float> ScrollInputed;
}
