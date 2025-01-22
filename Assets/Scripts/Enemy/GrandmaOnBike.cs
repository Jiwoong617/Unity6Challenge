using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class GrandmaOnBike : EnemyBase, IReceiveAttack
{
    [SerializeField] Projectile[] foods = new Projectile[3];
    [SerializeField] RiderGrandmaSummon shadow;

    SpriteRenderer[] sr;

    [SerializeField] float throwTime = 2f;
    float throwTimer = 0f;

    [SerializeField] float SkillTime = 3f;
    float skillTimer = 0f;

    protected override void Start()
    {
        base.Start();
        sr = GetComponentsInChildren<SpriteRenderer>();

        target = GameManager.instance.player.transform;
    }

    protected override void OnMove()
    {
        base.OnMove();
        throwTimer += Time.deltaTime;
        if (throwTimer >= throwTime)
        {
            throwTimer = 0f;
            ThrowFood();
        }

        skillTimer += Time.deltaTime;
        if (skillTimer > SkillTime)
        {
            skillTimer = 0f;
            SkillTime = Random.Range(3.5f, 5f);
            state = EnemyState.Skill;
        }
    }

    protected override void OnSkill()
    {
        if (SkillCo != null) return;

        SkillCo = StartCoroutine(ChooseSkill(Skill_1(), Skill_2(), Skill_3(), Skill_4()));
    }

    protected override void OnDie()
    {
        base.OnDie();
    }

    protected override void SpriteFlip()
    {
        base.SpriteFlip();
        for(int i = 0; i< sr.Length; i++)
        {
            sr[i].flipX = isFlip;
        }
    }

    #region Skill
    private void ThrowFood()
    {
        Projectile go = Instantiate(foods[Random.Range(0, foods.Length)]);
        go.Init(transform.position, target.transform.position + Vector3.down, 2f, 4f);
    }

    IEnumerator Skill_1()
    {
        Vector3 originPos = transform.position;

        animator.SetTrigger("Teleport");
        yield return new WaitForSeconds(0.5f);

        transform.position = Vector3.up * 2;

        int rand = Random.Range(20, 30);
        animator.SetBool("Sloshing", true);
        for(int i = 0; i<rand; i++)
        {
            float randX = Random.Range(-9f, 9f);
            Projectile go = Instantiate(foods[Random.Range(0, foods.Length)]);
            go.Init(transform.position, new Vector3(randX, -3.5f, 0), 1f, 0f);
            yield return new WaitForSeconds(0.1f);
        }
        animator.SetBool("Sloshing", false);

        animator.SetTrigger("Teleport");
        yield return new WaitForSeconds(0.5f);
        transform.position = originPos;

        state = EnemyState.Move;
        SkillCo = null;
    }

    IEnumerator Skill_2()
    {
        animator.SetTrigger("Teleport");
        yield return new WaitForSeconds(0.5f);

        transform.position = new Vector3(isFlip ? -patrolX + 1 : patrolX - 1, transform.position.y, 0);
        int rand = Random.Range(5, 9);
        animator.SetBool("Sloshing", true);

        int pos = rand;
        for(int i = 0; i<rand; i++)
        {
            pos %= 3;
            RiderGrandmaSummon go = Instantiate(shadow);
            switch (pos)
            { 
                case 0: //¿Þ
                    go.Init(Vector3.right * -9, Vector3.right, 3f, 0);
                    break;
                case 1: //À§
                    go.Init(Vector3.up * 3.5f, Vector3.down, 3f, 1);
                    break;
                case 2: //¿À
                    go.Init(Vector3.right * 9, Vector3.left, 3f, 2);
                    break;
            }
            pos++;
            yield return new WaitForSeconds(1f);
        }
        animator.SetTrigger("Teleport"); animator.SetBool("Sloshing", false);

        state = EnemyState.Move;
        SkillCo = null;
    }

    IEnumerator Skill_3()
    {
        float wheelieTime = 5f;
        float t = 0f;
        Quaternion q = isFlip ? Quaternion.Euler(0, 0, 60f) : Quaternion.Euler(0, 0, -60f);
        while (t < 2f)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, q, t / 2f);
            yield return null;
        }

        Speed *= 4f;
        t = 0f;
        while (t < wheelieTime)
        {
            base.OnMove();
            base.SpriteFlip();
            transform.Rotate(Vector3.up * Speed / 2f, Space.World);

            t += Time.deltaTime;
            yield return null;
        }

        Speed /= 4f;
        t = 0f;
        transform.rotation = q;
        while(t < 1f)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero), t);
            yield return null;
        }
        state = EnemyState.Move;
        SkillCo = null;
    }

    IEnumerator Skill_4()
    {
        Vector3 originPos = transform.position;

        for(int i = 0; i < 3; i++)
        {
            animator.SetTrigger("Teleport");
            yield return new WaitForSeconds(0.5f);

            float x, y;
            x = Random.Range(-8.5f, 8.5f);
            y = Random.Range(0, 10) < 5 ? Random.Range(1f, 3f) : Random.Range(-3.5f, -4f);
            transform.position = new Vector3(x, y, 0f);

            for (int j = 0; j < sr.Length; j++)
                sr[j].flipX = (target.position - transform.position).x > 0 ? true : false;

            yield return new WaitForSeconds(0.5f);

            rb.linearVelocity = (target.position - transform.position).normalized * Speed * 2.5f;

            while (transform.position.x < 9.5f && transform.position.x > -9.5f && transform.position.y < 3f && transform.position.y > -4f)
                yield return null;
            rb.linearVelocity = Vector3.zero;
            yield return new WaitForSeconds(0.2f);
        }
        animator.SetTrigger("Teleport");
        yield return new WaitForSeconds(0.5f);

        transform.position = originPos;
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
