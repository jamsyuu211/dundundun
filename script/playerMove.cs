using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class playerMove : MonoBehaviour
{
    //�÷��̾� ���� �� hp
    [Header("Status")]
    public float Damage = 0.5f;
    public float hp = 3f;
    public float maxHp = 3f;
    public PlayerHealth hpBarScript;
    public float tmpHp;
    public float tmpDamage;

    [Header("Enemy")]
    //�� ��ũ��Ʈ
    public Dog dogScript;
    public Bird birdScript;
    public Bomber bomberScript;
    public GroundBomber groundBomberScript;
    public LongArm longArmScript;

    //�� ���� ���� ����
    [Header("Enemy Bool")]
    public bool OnBomber = false;
    public bool OnGroundBomber = false;
    public bool AttackLongArm = false;

    //Fx���� ����
    [Header("VFX")]
    public GameObject Attack;
    public GameObject shield;
    
    //�������� ����
    Rigidbody2D rb;
    BoxCollider2D ObjCollider;

    //�ִϸ����� ���� ����
    [Header("Material")]
    public Material material;
    Animator animator;

    //trail���� ����
    [Header("Trail Obj")]
    public GameObject Trail;

    //���� ���� ����
    [Header("Sound")]
    public AudioSource sound;
    public AudioClip[] audioclip;

    //ui�ؽ�Ʈ ���� ����
    [Header("Text UI")]
    public TextMeshProUGUI textAtk;
    public TextMeshProUGUI textHp;

    //���� ���� ����
    [Header("Shield")]
    public float shieldDuration;
    public bool DestroyedShield = false;
    public bool changedColor = true;
    float shieldT = 0f;

    //�߷� ���� ����
    float gravity;

    //�̵� ���� ����
    [Header("Moving")]
    public float moveSpeed;
    Vector3 moveDir;
    bool isMove;
    bool touchedGround = false;

    //�뽬 ���� ����
    [Header("Dash")]
    public float DashPower;
    bool CanDash = true;
    float tmpVertical;
    float tmpHorizontal;

    //�÷��̾� ���콺 ��ų ���� ����
    [Header("Attack Skill")]
    public float firstSkillDuration;
    public float scanLength;
    public float scanDuration;
    public GameObject nearestObj;
    RaycastHit2D hit;
    float firstSkillT;
    bool isFirstSkill;
    Vector3 mousePos;
    GameObject tmpObj;
    float nearestLength;

    //���� ���� ����
    bool BackAttack;
    float declineHPTime;
    bool isFreeze;
    bool isAttacked = false;

    //���浹 ���� ����
    bool TouchedWall;

    //public ���� �ӽ������� ���� ���� ����
    float tmpMoveSpeed;
    float tmpDashPower;
    float tmpFirstSkillDuration;
    float tmpScanLength;
    float tmpScanDuration;

    //�÷��̾� ����� ����� ���� ����
    [Header("Restart")]
    public bool enemyActive = false;
    bool isRestart = false;
    Vector3 checkPoint;


    //player��ġ ���� ����
    [Header("Position")]
    public bool isBossZone = false;

    //�÷��̾� ��ų ���� ����
    [Header ("Player Skill")]
    public float magicBallDuration = 5f;
    bool usedMagicBall = false;
    float magicBallT = 0f;

    //public ���� �ʱ�ȭ�� �Լ�
    public void publicReset()
    {
        //��ġ �ʱ�ȭ
        transform.position = checkPoint;

        //public ����
        moveSpeed = tmpMoveSpeed;
        DashPower = tmpDashPower;
        firstSkillDuration = tmpFirstSkillDuration;
        scanLength = tmpScanLength;
        scanDuration = tmpScanDuration;

        //���� �� ���� ����
        OnBomber = false;
        OnGroundBomber = false;

        //FX���� ����
        Attack.SetActive(false);
        shield.SetActive(true);

        //���� ���� ����
        shieldT = 0f;
        DestroyedShield = false;
        changedColor = true;

        //�̵� ���� ����
        Vector3 moveDir = Vector3.zero;
        isMove = false;
        touchedGround = false;

        //�뽬 ���� ����
        CanDash = true;
        
        tmpVertical = 0f;
        tmpHorizontal = 0f;

        //�÷��̾� ���콺 ��ų ���� ����
        firstSkillT = 0f;
        isFirstSkill = false;
        mousePos = Vector3.zero;
        tmpObj = null;
        nearestObj = null;
        nearestLength = 0f;

        //hp �� ���� ���� ����
        BackAttack = false;
        declineHPTime = 0f;
        isFreeze = false;
        isAttacked = false;

        //���浹 ���� ����
        TouchedWall = false;

        material.color = Color.white;

        //�ִϸ����� ���� �ʱ�ȭ
        animator.SetBool("isRun", false);
        animator.SetBool("isDash", false);

        //player ��ġ ���� ����
        isBossZone = false;

        //�÷��̾� ��ų ���� ����
        usedMagicBall = false;

        //hp�� �ʱ�ȭ
        hpBarScript.publicReset();

        //status �ʱ�ȭ
        textAtk.text = Damage.ToString("F1");
        textHp.text = hp.ToString("F1");
    }

    // Start is called before the first frame update
    void Start()
    {
        ObjCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();


        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        gravity = rb.gravityScale;
        checkPoint = transform.position;

        //������ �ʴ� public ����
        tmpMoveSpeed = moveSpeed;
        tmpDashPower = DashPower;
        tmpFirstSkillDuration = firstSkillDuration;
        tmpScanLength = scanLength;
        tmpScanDuration = scanDuration;
        tmpHp = hp;
        tmpDamage = Damage;

        //FX ���� �ʱ�ȭ
        Trail.SetActive(false);
        Attack.SetActive(false);
        shield.SetActive(true);
        material.color = Color.white;

        textAtk.text = Damage.ToString("F1");
        textHp.text = hp.ToString("F1");
    }
    // Update is called once per frame
    void Update()
    {
        if (hp > 0)
        {
            if (isFreeze)
            {
                NoFreezeRb();
                rb.freezeRotation = true;
            }
            if (isAttacked)
            {
                declineHPTime += Time.deltaTime;
            }

            if (declineHPTime > 1f)
            {
                isAttacked = false;
                declineHPTime = 0f;
            }
            else if (declineHPTime > 0.5f)
            {
                material.color = Color.white;
            }

            //���� ���� �ڵ�
            if(DestroyedShield)
            {
                shieldT += Time.deltaTime;
                if(shieldT > shieldDuration)
                {
                    shieldT = 0f;
                    DestroyedShield = false;
                    shield.SetActive(true);
                    changedColor = true;
                }
                else if(shieldT > shieldDuration / 2f)
                {
                    shield.SetActive(false);
                }
            }
        }
        else
        {
            if (!isRestart)
            {
                sound.clip = audioclip[4];
                sound.volume = 1f;
                sound.Play();
                isRestart = true;
                animator.SetTrigger("death");
            }
        }

        if (hp > 0)
        {
            if (!animator.GetBool("isDash"))
            {
                if (rb.velocity.y < -20f)
                {
                    animator.SetBool("isDash", true);
                }
            }

            if (Input.GetKey(KeyCode.W))
            {
                tmpVertical = DashPower;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                tmpVertical = -DashPower;
            }
            else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
                tmpVertical = 0f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                isMove = true;
                rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, moveSpeed, Time.deltaTime * 100), rb.velocity.y);
                tmpHorizontal = DashPower;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                animator.SetBool("isRun", true);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                isMove = true;
                rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, -moveSpeed, Time.deltaTime * 100), rb.velocity.y);
                tmpHorizontal = -DashPower;
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);

                animator.SetBool("isRun", true);
            }
            else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                tmpHorizontal = 0f;

                animator.SetBool("isRun", false);
            }
            else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                isMove = false;
            }

            //�뽬 ���� �ڵ�
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (CanDash && !Trail.activeInHierarchy)
                {
                    CanDash = false;
                    Trail.SetActive(true);
                    animator.SetBool("isDash", true);
                    Dash();
                }
            }

            //�÷��̾� ���콺 ��Ŭ�� ��ų���� �ڵ�
            if (!isFirstSkill)
            {
                firstSkillT += Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (firstSkillT > firstSkillDuration)
                {
                    firstSkillT = 0f;
                    isFirstSkill = true;
                    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    FirstSkill();
                    isFirstSkill = false;
                }
            }
        }

        if(!usedMagicBall && Input.GetKeyDown(KeyCode.R))
        {
            usedMagicBall = true;
            MagicBall();
        }

        if (usedMagicBall)
        {
            magicBallT += Time.deltaTime;
            if(magicBallT > magicBallDuration)
            {
                usedMagicBall = false;
                magicBallT = 0f;
            }
        }
    }
    private void LateUpdate() //�÷��̾� �̵� �Ŀ� ī�޶� �̵�
    {
        shield.transform.position = transform.position - new Vector3(0f, -0.07f, 0f);
    }
    void MagicBall()
    {

    }
    void Dash()
    {
        if (!sound.isPlaying)
        {
            sound.volume = 0.4f;
            sound.clip = audioclip[1];
            sound.Play();
        }
        if (isMove)
        {
            rb.AddForce(new Vector2(tmpHorizontal, tmpVertical) * DashPower * rb.mass, ForceMode2D.Impulse);
        }
        else//���� �뽬
        {
            rb.AddForce(new Vector2(0f, tmpVertical) * DashPower * rb.mass, ForceMode2D.Impulse);
        }
    }

    void FirstSkill()
    {
        for (float i = -scanLength; i <= scanLength; i += scanDuration)
        {
            for (float j = -scanLength; j <= scanLength; j += scanDuration)
            {
                Vector3 distanceVec = new Vector3(j, i);
                // ����ĳ��Ʈ �߻�
                hit = Physics2D.Raycast(mousePos + distanceVec, Vector2.zero);

                if (hit.collider != null)
                {
                    // ���̿� �浹�� ��� ������Ʈ�� ���� �ݺ�
                    if ((hit.collider.gameObject.tag == "Dog" || hit.collider.gameObject.tag == "Bird" || hit.collider.gameObject.tag == "Bomber"
                        || hit.collider.gameObject.tag == "GroundBomber" || hit.collider.gameObject.tag == "LongArm")
                        && hit.collider.gameObject.activeSelf == true)
                    {
                        tmpObj = hit.collider.gameObject;
                        Vector3 distance = tmpObj.transform.position - mousePos;
                        float tmpLength = Mathf.Abs(Mathf.Sqrt(distance.x * distance.x + distance.y * distance.y));
                        if (nearestObj == null)
                        {
                            nearestObj = tmpObj;
                            nearestLength = tmpLength;
                        }
                        else
                        {
                            if (tmpLength < nearestLength)
                            {
                                nearestObj = tmpObj;
                                nearestLength = tmpLength;
                            }
                        }
                    }
                }
            }
        }

        if (nearestObj != null)
        {
            BackAttack = true;
            firstSkillEffect();
        }
    }

    void firstSkillEffect()
    {
        sound.volume = 0.4f;
        sound.clip = audioclip[0];
        sound.Play();
        // ����Ʈ �ڵ� �߰�
        animator.SetTrigger("attack");
        Attack.transform.position = nearestObj.transform.position;
        Attack.SetActive(true);
        Trail.SetActive(false);
        CanDash = false;

        transform.position = nearestObj.transform.position + new Vector3(0f, 0.5f);
    }
    void FreezeRb()
    {
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }
    void NoFreezeRb()
    {
        isFreeze = false;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
    }
    public void EndOfAttack2()
    {
        animator.SetTrigger("attack");
        BackAttack = false;
    }
    public void EndOfDeath()
    {
        sound.clip = null;
        rb.gravityScale = gravity;
        isRestart = false;
        enemyActive = true;
        NoFreezeRb();
        animator.SetTrigger("death");
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Dog")
        {
            if (!BackAttack && !isAttacked) //0.5�� ����
            {
                if (!shield.activeInHierarchy)
                {
                    isAttacked = true;
                    FreezeRb();
                    isFreeze = true;
                    hp -= dogScript.Damage;
                    material.color = Color.red;
                    hpBarScript.TakeDamage();
                }
                else//���尡 Ȱ��ȭ �Ǿ�����
                {
                    isAttacked = true;
                    DestroyedShield = true;
                }
            }
        }
        else if (collision.gameObject.tag == "DeathBlock")
        {
            hp = 0f;
            hpBarScript.TakeDamage();
            FreezeRb();
        }
        else if (collision.gameObject.tag == "FlyBomb")
        {
            if (!isAttacked && !BackAttack)
            {
                if (!shield.activeInHierarchy)
                {
                    isAttacked = true;
                    FreezeRb();
                    isFreeze = true;
                    hp -= bomberScript.Damage;
                    material.color = Color.red;
                    hpBarScript.TakeDamage();
                }
                else
                {
                    isAttacked = true;
                    DestroyedShield = true;
                }
            }
        }
        else if (collision.gameObject.tag == "Bomber" && OnBomber)
        {
            if (!isAttacked && !BackAttack)
            {
                if (!shield.activeInHierarchy)
                {
                    isAttacked = true;
                    FreezeRb();
                    isFreeze = true;
                    hp -= bomberScript.Damage;
                    material.color = Color.red;
                    OnBomber = false;
                    hpBarScript.TakeDamage();
                }
                else
                {
                    isAttacked = true;
                    DestroyedShield = true;
                }
            }
        }
        
        
        if (collision.gameObject.tag == "Ground")
        {
            if(Trail.activeInHierarchy || !CanDash)
            {
                CanDash = true;

                animator.SetBool("isDash", false);
                Trail.SetActive(false);
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "StageWall")
        {
            if (!TouchedWall)
            {
                TouchedWall = true;
                rb.gravityScale = 80f;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (TouchedWall)
            {
                TouchedWall = false;
                rb.gravityScale = gravity;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Dog")
        {
            if (!BackAttack && !isAttacked) //0.5�� ����
            {
                if (!shield.activeInHierarchy)
                {
                    isAttacked = true;
                    FreezeRb();
                    isFreeze = true;
                    hp -= dogScript.Damage;
                    material.color = Color.red;
                    hpBarScript.TakeDamage();
                }
                else//���尡 Ȱ��ȭ �Ǿ�����
                {
                    isAttacked = true;
                    DestroyedShield = true;
                }
            }
        }
        else if (collision.tag == "BirdBullet")
        {
            if (!isAttacked && !BackAttack)
            {
                if (!shield.activeInHierarchy)
                {
                    isAttacked = true;
                    FreezeRb();
                    isFreeze = true;
                    hp -= birdScript.Damage;
                    material.color = Color.red;
                    hpBarScript.TakeDamage();
                }
                else
                {
                    isAttacked = true;
                    DestroyedShield = true;
                }
            }
        }
        else if (collision.tag == "BossSkill")
        {
            if (!isAttacked && !BackAttack)
            {
                if (!shield.activeInHierarchy)
                {
                    isAttacked = true;
                    FreezeRb();
                    isFreeze = true;
                    hp -= longArmScript.Damage;
                    material.color = Color.red;
                    hpBarScript.TakeDamage();
                }
                else
                {
                    isAttacked = true;
                    DestroyedShield = true;
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "LongArm" && AttackLongArm)
        {
            if (!isAttacked && !BackAttack)
            {
                if (!shield.activeInHierarchy)
                {
                    AttackLongArm = false;
                    isAttacked = true;
                    FreezeRb();
                    isFreeze = true;
                    hp -= longArmScript.Damage;
                    material.color = Color.red;
                    hpBarScript.TakeDamage();
                }
                else
                {
                    isAttacked = true;
                    DestroyedShield = true;
                }
            }
        }
        else if (collision.tag == "GroundBomber" && OnGroundBomber)
        {
            if (!isAttacked && !BackAttack)
            {
                if (!shield.activeInHierarchy)
                {
                    isAttacked = true;
                    FreezeRb();
                    isFreeze = true;
                    hp -= groundBomberScript.Damage;
                    material.color = Color.red;
                    OnGroundBomber = false;
                    hpBarScript.TakeDamage();
                }
                else
                {
                    isAttacked = true;
                    DestroyedShield = true;
                }
            }
        }
    }
}