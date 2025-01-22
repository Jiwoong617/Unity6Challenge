using UnityEngine;

public class PlayerProjectile : Projectile
{
    public override void Init(Vector3 startPos, Vector3 targetPos, float time, float high, bool enemy = true)
    {
        base.Init(startPos, targetPos, time, high, enemy);

        transform.right = (targetPos - startPos).normalized;
        PlayerParryed();
    }
}
