using UnityEngine;

public interface IHittable
{
    public void Hit(Vector3 bulletOriginPosition, float damage);
}
