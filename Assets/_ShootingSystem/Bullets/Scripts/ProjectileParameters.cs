using UnityEngine;

public struct ProjectileParameters
{
    public Vector3 OriginPoint { get; private set; }
    public Vector3 HitPoint { get; private set; }
    public Vector3 Direction { get; private set; }

    public float Damage { get; private set; }
    public float Range { get; private set; }
    public float Speed { get; private set; }
    
    public ProjectileParameters(Vector3 originPoint, Vector3 direction, Vector3 hitPoint, GunParameters gunParameters)
        : this(originPoint, direction, hitPoint,
              gunParameters.TotalDamagePerProjectile, gunParameters.TotalRange, gunParameters.TotalSpeed)
    { }

    public ProjectileParameters(Vector3 originPoint, Vector3 direction, GunParameters gunParameters)
        : this(originPoint, direction,
              gunParameters.TotalDamagePerProjectile, gunParameters.TotalRange, gunParameters.TotalSpeed)
    { }

    public ProjectileParameters(Vector3 originPoint, Vector3 direction, Vector3 hitPoint,
        float damage, float range, float speed)
    {
        HitPoint = hitPoint;
        OriginPoint = originPoint;
        Direction = direction;

        Damage = damage;
        Range = range;
        Speed = speed;
    }

    public ProjectileParameters(Vector3 originPoint, Vector3 direction,
        float damage, float range, float speed)
    {
        HitPoint = originPoint;
        OriginPoint = originPoint;
        Direction = direction;

        Damage = damage;
        Range = range;
        Speed = speed;
    }

    public void SetHitPoint(Vector3 hitPoint)
    {
        HitPoint = hitPoint;
    }
}
