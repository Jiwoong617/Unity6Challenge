using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Collider2D col;

    protected Transform target;

    [SerializeField] protected EnemyState state = EnemyState.Move;
    protected Coroutine SkillCo = null;

    protected float patrolX = 9.5f;
    [SerializeField] protected int MaxHp;
    [SerializeField] protected int Hp;
    [SerializeField] protected float Speed;
    [SerializeField] protected bool isFlip = false; //true - right, false - left
    [SerializeField] protected bool isAlive = true;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        //target = GameManager.instance.player.transform;
    }

    void Update()
    {
        if (GameManager.instance.isCutScenePlaying) return;

        switch (state)
        {
            case EnemyState.Move:
                SpriteFlip();
                OnMove();
                break;
            case EnemyState.Skill:
                OnSkill();
                break;
            case EnemyState.Die:
                OnDie();
                break;
        }
    }

    public void Init(int hp, float speed)
    {
        MaxHp = hp;
        Hp = hp;
        Speed = speed;
    }

    protected virtual IEnumerator ChooseSkill(params IEnumerator[] Skills)
    {
        int rand = Random.Range(0, Skills.Length);
        //Debug.Log($"{Skills.Length},  {rand}");
        return Skills[rand];
    }

    protected virtual void OnMove()
    {
        if (!isFlip)
            rb.MovePosition(transform.position + Vector3.left * Speed * Time.deltaTime);
        else
            rb.MovePosition(transform.position + Vector3.right * Speed * Time.deltaTime);
    }

    protected virtual void SpriteFlip()
    {
        if (transform.position.x > patrolX) isFlip = false;
        else if (transform.position.x < -patrolX) isFlip = true;
    }

    protected virtual void OnSkill() { }
    protected virtual void OnDie() 
    {
        if (!isAlive) return;

        if (SkillCo != null) StopCoroutine(SkillCo);
        isAlive = false;
        animator.SetTrigger("Dead");
        StartCoroutine(DeadCo());
    }

    protected IEnumerator DeadCo()
    {
        if(SkillCo != null) StopCoroutine(SkillCo);
        col.enabled = false;

        yield return new WaitForSeconds(2f);
        GameManager.instance.OnRoundEnd();

        Destroy(gameObject);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        IReceiveAttack hitted = collision.GetComponent<IReceiveAttack>();
        if (hitted == null)
            return;

        hitted.Attacked();
    }
}
