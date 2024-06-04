using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class playerMove : MonoBehaviour
{
    //플레이어 공격 및 hp
    public float Damage = 0.5f;
    public float hp = 3f;
    public float maxHp = 3f;
    public PlayerHealth hpBarScript;
    public float tmpHp;
    public float tmpDamage;

    //적 스크립트
    public Dog dogScript;
    public Bird birdScript;
    public Bomber bomberScript;
    public GroundBomber groundBomberScript;
    public LongArm longArmScript;

    //적 자폭 관련 변수
    public bool OnBomber = false;
    public bool OnGroundBomber = false;
    public bool AttackLongArm = false;

    //Fx관련 변수
    public GameObject Attack;
    public GameObject shield;

    //카메라 관련 변수
    GameObject MainCamera;
    
    //물리관련 변수
    Rigidbody2D rb;
    BoxCollider2D ObjCollider;
    
    //애니메이터 관련 변수
    Animator animator;
    public Material material;

    //trail관련 변수
    public GameObject Trail;

    //사운드 관련 변수
    public AudioSource sound;
    public AudioClip[] audioclip;

    //ui텍스트 관련 변수
    public TextMeshProUGUI textAtk;
    public TextMeshProUGUI textHp;

    //쉴드 관련 변수
    float shieldT = 0f;
    public float shieldDuration;
    public bool DestroyedShield = false;
    public bool changedColor = true;

    //중력 관련 변수
    float gravity;

    //이동 관련 변수
    public float moveSpeed;
    Vector3 moveDir;
    bool isMove;
    bool touchedGround = false;

    //대쉬 관련 변수
    bool CanDash = true;
    public float DashPower;
    float tmpVertical;
    float tmpHorizontal;

    //플레이어 마우스 스킬 관련 변수
    RaycastHit2D hit;
    float firstSkillT;
    bool isFirstSkill;
    public float firstSkillDuration;
    Vector3 mousePos;
    GameObject tmpObj;
    public GameObject nearestObj;
    float nearestLength;
    public float scanLength;
    public float scanDuration;

    //공격 관련 변수
    bool BackAttack;
    float declineHPTime;
    bool isFreeze;
    bool isAttacked = false;

    //벽충돌 관련 변수
    bool TouchedWall;

    //public 변수 임시저장을 위해 만든 변수
    float tmpMoveSpeed;
    float tmpDashPower;
    float tmpFirstSkillDuration;
    float tmpScanLength;
    float tmpScanDuration;

    //플레이어 사망시 재시작 관련 변수
    bool isRestart = false;
    Vector3 checkPoint;
    public bool enemyActive = false;


    //player위치 관련 변수
    public bool isBossZone = false;

    //public 변수 초기화용 함수
    public void publicReset()
    {
        //위치 초기화
        transform.position = checkPoint;

        //public 변수
        moveSpeed = tmpMoveSpeed;
        DashPower = tmpDashPower;
        firstSkillDuration = tmpFirstSkillDuration;
        scanLength = tmpScanLength;
        scanDuration = tmpScanDuration;

        //자폭 적 관련 변수
        OnBomber = false;
        OnGroundBomber = false;

        //FX관련 변수
        Attack.SetActive(false);
        shield.SetActive(true);

        //쉴드 관련 변수
        shieldT = 0f;
        DestroyedShield = false;
        changedColor = true;

        //이동 관련 변수
        Vector3 moveDir = Vector3.zero;
        isMove = false;
        touchedGround = false;

        //대쉬 관련 변수
        CanDash = true;
        
        tmpVertical = 0f;
        tmpHorizontal = 0f;

        //플레이어 마우스 스킬 관련 변수
        firstSkillT = 0f;
        isFirstSkill = false;
        mousePos = Vector3.zero;
        tmpObj = null;
        nearestObj = null;
        nearestLength = 0f;

        //hp 및 공격 관련 변수
        BackAttack = false;
        declineHPTime = 0f;
        isFreeze = false;
        isAttacked = false;

        //벽충돌 관련 변수
        TouchedWall = false;

        material.color = Color.white;

        //애니메이터 변수 초기화
        animator.SetBool("isRun", false);
        animator.SetBool("isDash", false);

        //player 위치 관련 변수
        isBossZone = false;

        //hp바 초기화
        hpBarScript.publicReset();

        //status 초기화
        textAtk.text = Damage.ToString();
        textHp.text = hp.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        ObjCollider = GetComponent<BoxCollider2D>();
        MainCamera = GameObject.FindWithTag("MainCamera");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();


        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        gravity = rb.gravityScale;
        checkPoint = transform.position;

        //변하지 않는 public 변수
        tmpMoveSpeed = moveSpeed;
        tmpDashPower = DashPower;
        tmpFirstSkillDuration = firstSkillDuration;
        tmpScanLength = scanLength;
        tmpScanDuration = scanDuration;
        tmpHp = hp;
        tmpDamage = Damage;

        //FX 변수 초기화
        Trail.SetActive(false);
        Attack.SetActive(false);
        shield.SetActive(true);
        material.color = Color.white;

        textAtk.text = Damage.ToString();
        textHp.text = hp.ToString();
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

            //쉴드 관련 코드
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
                sound.volume = 0.5f;
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

            //대쉬 관련 코드
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

            //플레이어 마우스 우클릭 스킬관련 코드
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
    }
    private void LateUpdate() //플레이어 이동 후에 카메라 이동
    {
        Vector3 move_pos = transform.position - MainCamera.transform.position;
        move_pos.z = 0;
        MainCamera.transform.Translate(move_pos * Time.deltaTime * moveSpeed);
        shield.transform.position = transform.position - new Vector3(0f, -0.07f, 0f);
    }
    void Dash()
    {
        if (!sound.isPlaying)
        {
            sound.volume = 0.2f;
            sound.clip = audioclip[1];
            sound.Play();
        }
        if (isMove)
        {
            rb.AddForce(new Vector2(tmpHorizontal, tmpVertical) * DashPower * rb.mass, ForceMode2D.Impulse);
        }
        else//수직 대쉬
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
                // 레이캐스트 발사
                hit = Physics2D.Raycast(mousePos + distanceVec, Vector2.zero);

                if (hit.collider != null)
                {
                    // 레이와 충돌한 모든 오브젝트에 대해 반복
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
        sound.volume = 0.2f;
        sound.clip = audioclip[0];
        sound.Play();
        // 이펙트 코드 추가
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
            if (!BackAttack && !isAttacked) //0.5초 무적
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
                else//쉴드가 활성화 되었으면
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
            if (!BackAttack && !isAttacked) //0.5초 무적
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
                else//쉴드가 활성화 되었으면
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