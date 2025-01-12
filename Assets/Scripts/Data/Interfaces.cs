using UnityEngine;

public interface IReceiveAttack
{
    public abstract void Attacked(Projectile proj = null);
}
