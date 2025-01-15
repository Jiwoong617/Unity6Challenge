using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour, IReceiveAttack
{
    const string JUMP = "Jump";
    const string MOVE = "Move";
    const string ISGROUND = "IsGround";
    const string DODGE = "Dodge";

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sr;

    private int MaxHp = 5;
    private int Hp = 5;

    public float jumpForce = 10f;
    public float speed = 5f;

    float xDir;
    bool isGround = true;
    bool isDodge = false;
    bool isParry = false;
    bool invincible = false;

    Coroutine ParryCo;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        GameManager.instance.player = this;
    }

    private void Update()
    {
        if (GameManager.instance.isCutScenePlaying) return;

        Move();
        Jump();
        DodgeFunc();
        CheckGround();
    }

    private void Move()
    {
        if (isDodge || isParry) return;

        xDir = Input.GetAxisRaw("Horizontal");
        rb.linearVelocityX = xDir * speed;

        if (xDir != 0)
            sr.flipX = xDir < 0;

        animator.SetBool(MOVE, Math.Abs(xDir) > 0);
        Global.IsPlayerMoveRight?.Invoke((int)xDir);
    }

    private void Jump()
    {
        if (!isGround || isDodge || isParry) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger(JUMP);
            isGround = false;
        }
    }

    private void CheckGround()
    {
        animator.SetBool(ISGROUND, isGround);
        if (isGround) return;

        if(rb.linearVelocityY < 0f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                isGround = true;
            }
        }
    }

    private void DodgeFunc()
    {
        if (isDodge || isParry)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1))
        {
            StartCoroutine(DodgeCoroutine(0.55f));
        }
    }

    private IEnumerator DodgeCoroutine(float dodgeTime)
    {
        isDodge = true;
        animator.SetBool(DODGE, isDodge);

        invincible = true;
        {   //dodge trans
            Vector2 startPos = transform.position;
            Vector2 endPos = GetDir() * 3 + transform.position;

            float elapsedTime = 0f;
            while (elapsedTime < 0.3f)
            {
                rb.MovePosition(Vector3.Lerp(startPos, endPos, elapsedTime / 0.3f));
                elapsedTime+= Time.deltaTime;
                yield return null;
            }
        }
        invincible = false;

        yield return new WaitForSeconds(dodgeTime - 0.3f);
        isDodge = false;
        animator.SetBool(DODGE, isDodge);
    }

    private IEnumerator Parry()
    {
        isParry = true;

        yield return null;

        isParry = false;
    }

    private Vector3 GetDir() => sr.flipX ? Vector3.left : Vector3.right;

    public void Attacked(Projectile proj = null)
    {
        if (isParry)
        {
            if(proj != null)
                proj.Init(transform.position, GameManager.instance.nowBoss.transform.position, 0, 0, false);
            return;
        }
        if (invincible)
            return;

        Hp--;
        Destroy(proj.gameObject);
    }
}
