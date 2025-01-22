using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class StartScenePlayer : MonoBehaviour
{
    const string JUMP = "Jump";
    const string MOVE = "Move";
    const string ISGROUND = "IsGround";
    const string DODGE = "Dodge";
    const string PARRY = "Parry";
    const string ISALIVE = "IsAlive";

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sr;

    public float jumpForce = 10f;
    public float speed = 5f;

    float xDir;
    bool isGround = true;
    bool isDodge = false;
    bool isParry = false;

    public float ParryCooltime = 3f;
    float elapsedParryTime = 0f;
    bool canParry = true;
    Coroutine parryCo = null;

    int _parryEnergy = 0;
    int parryEnergy
    {
        get { return _parryEnergy; }
        set
        {
            _parryEnergy = value;
            if (GameManager.instance != null)
                GameManager.instance.ChangePlayerEnergyUI(value);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        animator.SetBool(ISALIVE, true);
    }

    private void Update()
    {
        Move();
        Jump();
        DodgeFunc();
        ParryFunc();
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

        if (rb.linearVelocityY < 0f)
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

        {   //dodge trans
            Vector2 startPos = transform.position;
            Vector2 endPos = GetDir() * 3 + transform.position;

            float elapsedTime = 0f;
            while (elapsedTime < 0.3f)
            {
                rb.MovePosition(Vector3.Lerp(startPos, endPos, elapsedTime / 0.3f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(dodgeTime - 0.3f);
        isDodge = false;
        animator.SetBool(DODGE, isDodge);
    }

    private void ParryFunc()
    {
        if (!canParry || isParry || !isGround) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            parryCo = StartCoroutine(Parry());
            isParry = true;
            canParry = false;
        }
    }

    private IEnumerator Parry()
    {
        rb.linearVelocity = Vector3.zero;
        animator.SetTrigger(PARRY);
        yield return new WaitForSeconds(0.25f);
        isParry = false;

        elapsedParryTime = 0f;
        while (elapsedParryTime < ParryCooltime)
        {
            elapsedParryTime += Time.deltaTime;
            yield return null;
        }

        canParry = true;
        parryCo = null;
    }
    private Vector3 GetDir() => sr.flipX ? Vector3.left : Vector3.right;
}
