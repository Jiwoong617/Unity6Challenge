using System.Collections;
using UnityEngine;

public class DeliverSummon : Projectile
{
    private bool readyToFire = false;
    private float speed = 30f;

    private void Update()
    {
        if(isEnemy)
        {
            if (readyToFire)
                transform.position += target * speed * Time.deltaTime;
        }
    }

    public override void Init(Vector3 startPos, Vector3 targetPos, float time, float high, bool enemy = true)
    {
        base.Init(startPos, targetPos, time, high, enemy);

        if (high == 0) StartCoroutine(LockOnAndShoot(time));
        else if (high == 1) StartCoroutine(ShadowPartner(time));
    }

    IEnumerator LockOnAndShoot(float time)
    {
        GetComponent<SpriteRenderer>().flipX = true;

        float t = 2f;
        float elapsedTime = 0f;
        while(elapsedTime < t)
        {
            elapsedTime += Time.deltaTime;
            transform.position = start;
            transform.right = GameManager.instance.player.transform.position;
            target = GameManager.instance.player.transform.position;
            yield return null;
        }
        target = Vector3.Normalize(target - transform.position);
        yield return new WaitForSeconds(time);
        readyToFire = true;
    }

    IEnumerator ShadowPartner(float time)
    {
        if(transform.position.x < 0f) GetComponent<SpriteRenderer>().flipX = true;

        yield return new WaitForSeconds(time);
        readyToFire = true;
    }
}
