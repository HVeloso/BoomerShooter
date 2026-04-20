using UnityEngine;

public interface IHittable
{
    public void Hit(Transform player, float damage);
}
