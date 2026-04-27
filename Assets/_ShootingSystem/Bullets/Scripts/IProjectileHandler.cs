using System;
using System.Collections.Generic;

public interface IProjectileHandler
{
    public event Action<IProjectileHandler> ObjectDisabled;
    public void ActiveBullet(ProjectileParameters parameters);
}
