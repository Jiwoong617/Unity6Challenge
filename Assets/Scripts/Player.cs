using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour, IReceiveAttack
{
    const string JUMP = "Jump";
    const string MOVE = "Move";
    const string ISGROUND = "IsGround";
    const string DODGE = "Dodge";
    const string PARRY = "Parry";
    const string ISALIVE = "IsAlive";

    [SerializeField] ParticleSystem parryParticle;
    [SerializeField] Image parryCooltimeUI;
    [SerializeField] Projectile ShootEnergyPrefab;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sr;

    public int MaxHp = 3;
    private int _Hp = 3;
    public int Hp
    {
        get { return _Hp; }
        set 
        {
            _Hp = value; 
            if(GameManager.instance != null)
                GameManager.instance.ChangePlayerHpUI(Hp);
        }
    }

    public float jumpForce = 10f;
    public float speed = 5f;
    public float dodgeDistance = 3f;

    float xDir;
    bool isAlive = true;
    bool isGround = true;
    bool isDodge = false;
    bool isParry = false;
    bool invincible = false;
    Coroutine HitCoroutine = null;

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
            if(GameManager.instance != null)
                GameManager.instance.ChangePlayerEnergyUI(value);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        Init();
        if(GameManager.instance != null )
            GameManager.instance.player = this;
    }

    private void Update()
    {
        if (GameManager.instance.isCutScenePlaying || isAlive == false) return;

        Move();
        Jump();
        DodgeFunc();
        ParryFunc();
        Shoot();
        CheckGround();
    }

    public void Init()
    {
        MaxHp = 3;
        Hp = 3;
        jumpForce = 10f;
        speed = 5f;
        ParryCooltime = 3f;
        elapsedParryTime = 0f;
        parryEnergy = 0;
        dodgeDistance = 3f;

        isAlive = true;
        animator.SetBool(ISALIVE, isAlive);

        sr.transform.position = Vector3.zero;
        sr.transform.rotation = Quaternion.Euler(Vector3.zero);
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

        if(rb.linearVelocityY <= 0f)
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
            Vector2 endPos = GetDir() * dodgeDistance + transform.position;

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

    private void ParryFunc()
    {
        if (!canParry || isParry || !isGround) return;

        if(Input.GetKeyDown(KeyCode.Q))
        {
            parryCo = StartCoroutine(Parry());
            isParry = true;
            canParry = false;
        }
    }

    private IEnumerator Parry()
    {
        rb.linearVelocity= Vector3.zero;
        animator.SetTrigger(PARRY);
        yield return new WaitForSeconds(0.25f);
        isParry = false;

        elapsedParryTime = 0f;
        parryCooltimeUI.gameObject.SetActive(true);
        while (elapsedParryTime < ParryCooltime)
        {
            parryCooltimeUI.fillAmount = elapsedParryTime / ParryCooltime;
            elapsedParryTime += Time.deltaTime;
            yield return null;
        }
        parryCooltimeUI.gameObject.SetActive(false);

        canParry = true;
        parryCo = null;
    }

    private void Shoot()
    {
        if (parryEnergy < 3) return;

        if(Input.GetMouseButtonDown(0))
        {
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Projectile go = Instantiate(ShootEnergyPrefab);
            go.Init(transform.position, targetPos, 0, 0, false);

            parryEnergy = 0;
        }
    }

    private Vector3 GetDir() => sr.flipX ? Vector3.left : Vector3.right;

    public void Attacked(Projectile proj = null)
    {
        if (!isAlive) return;

        if (isParry)
        {
            if(proj != null)
            {
                proj.Init(proj.transform.position, GameManager.instance.nowBoss.transform.position, 0, 0, false);
            }
            parryEnergy = Mathf.Min(parryEnergy + 1, 3);
            if (!parryParticle.isPlaying)
                parryParticle.Play();
            return;
        }
        if (invincible)
            return;

        if(HitCoroutine == null)
            HitCoroutine = StartCoroutine(HitCo());

        if(proj != null)
            Destroy(proj.gameObject);
    }

    IEnumerator HitCo()
    {
        Hp--;
        if(Hp <= 0)
        {
            OnDead();
            HitCoroutine = null;
            yield break;
        }

        int n = 0;
        while(n < 10)
        {
            if(n % 2 ==0) sr.color = new Color(1f, 1f, 1f, 0.5f);
            else sr.color = new Color(1f, 1f, 1f, 0.8f);
            n++;
            yield return new WaitForSeconds(0.1f);
        }
        sr.color = Color.white;


        HitCoroutine = null;
    }

    private void OnDead()
    {
        isAlive = false;
        animator.SetBool(ISALIVE, isAlive);
        GameManager.instance.OnGameFailed();
    }
}
