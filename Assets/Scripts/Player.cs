using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour
{
    const string JUMP = "Jump";
    const string MOVE = "Move";
    const string ISGROUND = "IsGround";
    const string DODGE = "Dodge";

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sr;

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
    }

    private void Update()
    {
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
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
                transform.position = Vector3.Lerp(startPos, endPos, elapsedTime/0.3f);
                elapsedTime+= Time.deltaTime;
                yield return null;
            }
        }
        invincible = false;

        yield return new WaitForSeconds(dodgeTime - 0.3f);
        isDodge = false;
        animator.SetBool(DODGE, isDodge);
    }

    private void Parry()
    {

    }

    private Vector3 GetDir() => sr.flipX ? Vector3.left : Vector3.right;
}
