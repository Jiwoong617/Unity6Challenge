using System.Collections;
using UnityEngine;

public class Grandma : EnemyBase, IReceiveAttack
{
    [SerializeField] Projectile[] foods = new Projectile[3];

    SpriteRenderer sr;
    [SerializeField]float throwTime = 2f;
    float timer = 0f;

    [SerializeField] float SkillTime = 5f;
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
        timer += Time.deltaTime;
        skillTimer += Time.deltaTime;
        if(timer >= throwTime)
        {
            timer = 0f;
            ThrowFood();
        }

        if(skillTimer > SkillTime)
        {
            skillTimer = 0f;
            SkillTime = Random.Range(4f, 6f);
            state = EnemyState.Skill;
        }
    }

    protected override void OnSkill()
    {
        if (SkillCo != null) return;

        SkillCo = StartCoroutine(ChooseSkill(Skill_1(), Skill_2()));
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

    private void ThrowFood()
    {
        Projectile go = Instantiate(foods[Random.Range(0, foods.Length)]);
        go.Init(transform.position, new Vector3(target.transform.position.x, -3.5f, 0), 2f, 4f);
    }

    # region Skill
    IEnumerator Skill_1()
    {
        int x = isFlip ? 10 : -10;
        for(int i = 0; i<11; i++)
        {
            Projectile go = Instantiate(foods[Random.Range(0, foods.Length)]);
            go.Init(transform.position, new Vector3(x, -3.5f, 0f), 3f, 5f);
            x = isFlip ? x - 2 : x + 2;

            yield return new WaitForSeconds(0.5f);
        }

        state = EnemyState.Move;
        SkillCo = null;
    }

    IEnumerator Skill_2()
    {
        animator.SetTrigger("Charging");
        yield return new WaitForSeconds(2.9f);

        for(int i = 0; i<10; i++)
        {
            Projectile go = Instantiate(foods[Random.Range(0, foods.Length)]);
            go.Init(transform.position, new Vector3(Random.Range(-9, 10), -3.5f, 0), Random.Range(3f, 4f), Random.Range(5f, 6f));
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
