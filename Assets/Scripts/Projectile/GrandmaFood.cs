using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GrandmaFood : Projectile
{
    private void Update()
    {
        if (isEnemy)
            transform.position = CurvePathToPlayer();
    }

    private Vector2 CurvePathToPlayer()
    {
        elapseTime += Time.deltaTime;
        float t = elapseTime / duration;

        float x = Mathf.Lerp(start.x, target.x, t);
        float y = Mathf.Lerp(start.y, target.y, t);

        y += height * Mathf.Sin(Mathf.PI * t);

        return new Vector2(x, y);
    }

}
