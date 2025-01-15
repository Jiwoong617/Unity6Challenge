using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class GrandmaOnBike : EnemyBase
{
    [SerializeField] Projectile[] foods = new Projectile[3];

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

        SkillCo = StartCoroutine(ChooseSkill(/*Skill_1(), Skill_2(),*/ Skill_3()));
    }

    protected override void OnDie()
    {
        if (!isAlive) return;

        if (SkillCo != null) StopCoroutine(SkillCo);
        isAlive = false;
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
        go.Init(transform.position, target.transform.position, 2f, 4f);
    }

    IEnumerator Skill_1()
    {
        yield return null;
    }

    IEnumerator Skill_2()
    {
        yield return null;
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
    # endregion

    public void Attacked(Projectile proj = null)
    {
        if (proj != null)
            Destroy(proj.gameObject);

        Hp--;
        if (Hp <= 0)
            state = EnemyState.Die;
    }
}
