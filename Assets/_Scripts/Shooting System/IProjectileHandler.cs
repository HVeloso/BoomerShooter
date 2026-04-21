using System.Collections.Generic;

public interface IProjectileHandler
{
    public void ActiveBullet(ProjectileParameters parameters, Queue<IProjectileHandler> queue);
}
