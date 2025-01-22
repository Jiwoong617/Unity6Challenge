using System.Collections;
using UnityEngine;

public class RiderGrandmaSummon : Projectile
{
    public override void Init(Vector3 startPos, Vector3 targetPos, float time, float high, bool enemy = true)
    {
        base.Init(startPos, targetPos, time, high, enemy);

        StartCoroutine(MoveToPlayer(time));
    }


    IEnumerator MoveToPlayer(float time)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (height == 0) GetComponent<SpriteRenderer>().flipX = true;
        if (height == 0 || height == 2) col.excludeLayers = LayerMask.GetMask("Ground", "Enemy");

        col.enabled = false;
        float t = 0f;
        while (t < time - 1f)
        {
            if (height == 0 || height == 2) transform.position = new Vector3(start.x, GameManager.instance.player.transform.position.y, 0f);
            else transform.position = new Vector3(GameManager.instance.player.transform.position.x, start.y, 0f);
            t+= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        col.enabled = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = target * 20f;
    }
}
