using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected Vector3 start, target;

    protected float height; // 던지는 높이
    protected float duration; // 도달하는 시간
    protected float elapseTime = 0f;

    [SerializeField] protected bool isEnemy = true;
    [SerializeField] LayerMask playerLayer;

    public virtual void Init(Vector3 startPos, Vector3 targetPos, float time, float high, bool enemy = true)
    {
        isEnemy = enemy;
        start = startPos;
        target = targetPos;
        duration = time;
        height = high;

        transform.position = startPos;

        if(isEnemy == false)
        {
            GetComponent<Collider2D>().excludeLayers = playerLayer;
            PlayerParryed();
        }
    }

    protected void PlayerParryed()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = (target - start).normalized * 20f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Maps")
        {
            Destroy(gameObject);
            return;
        }

        IReceiveAttack hitted = collision.GetComponent<IReceiveAttack>();
        if (hitted == null)
            return;

        hitted.Attacked(this);
    }
}
