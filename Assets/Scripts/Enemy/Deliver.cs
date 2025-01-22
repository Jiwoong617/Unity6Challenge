using UnityEngine;
using System.Collections;

public class Deliver : EnemyBase, IReceiveAttack
{
    [SerializeField] DeliverSummon shadow;

    SpriteRenderer sr;

    [SerializeField] float SkillTime = 2f;
    float skillTimer = 0f;

    protected override void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();

        target = GameManager.instance.player.transform;
    }

    protected override void OnMove()
    {
        base.OnMove();
        skillTimer += Time.deltaTime;
        if (skillTimer > SkillTime)
        {
            skillTimer = 0f;
            SkillTime = Random.Range(2.5f, 3.5f);
            state = EnemyState.Skill;
        }
    }

    protected override void OnSkill()
    {
        if (SkillCo != null) return;

        SkillCo = StartCoroutine(ChooseSkill(Skill_1(), Skill_2(), Skill_3()));
    }

    protected override void OnDie()
    {
        base.OnDie();
    }

    protected override void SpriteFlip()
    {
        base.SpriteFlip();
        sr.flipX = isFlip;
    }

    # region Skill
    IEnumerator Skill_1() //µ¹Áø
    {
        Vector3 dir = isFlip ? Vector2.right : Vector2.left;
        Vector3 originPos = transform.position;
        animator.SetTrigger("Charging");

        yield return new WaitForSeconds(2f);

        while(Mathf.Abs(transform.position.x) < 12)
        {
            rb.MovePosition(transform.position + dir * Speed * 5 * Time.deltaTime);
            yield return null;
        }

        transform.position = new Vector3(dir.x * -12, transform.position.y, transform.position.z);
        while(Vector3.Distance(transform.position, originPos) > 0.5f)
        {
            rb.MovePosition(transform.position + dir * Speed * 5 * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        state = EnemyState.Move;
        SkillCo = null;
    }

    IEnumerator Skill_2()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 3; i++)
        {
            DeliverSummon go = Instantiate(shadow);
            go.Init(new Vector3(-5 + i * 5, Random.Range(1.5f, 3f), 0), Vector3.zero, 0.3f, 0);
            yield return new WaitForSeconds(0.3f);
        }

        state = EnemyState.Move;
        SkillCo = null;
    }
    
    IEnumerator Skill_3()
    {
        animator.SetTrigger("Charging");
        yield return new WaitForSeconds(2f);

        transform.position = new Vector3(isFlip ? -patrolX + 1 : patrolX - 1, transform.position.y, 0);
        int rand = Random.Range(3, 6);
        float t = rand * 0.5f;
        for (int i = 0; i < rand; i++)
        {
            DeliverSummon go = Instantiate(shadow);
            go.Init(new Vector3(Random.Range(-0.5f, 0.5f) + transform.position.x, transform.position.y + 0.1f, 0), 
                isFlip ? Vector3.right : Vector3.left, t + 0.3f, 1);

            t -= 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        state = EnemyState.Move;
        SkillCo = null;
    }
    # endregion

    public void Attacked(Projectile proj = null)
    {
        if (proj != null)
            Destroy(proj.gameObject);

        Hp--;
        GameManager.instance.ChangeBossHpUI(Hp, MaxHp);
        if (Hp <= 0)
            state = EnemyState.Die;
    }
}
