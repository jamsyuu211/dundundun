using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LongArm : MonoBehaviour
{
    //longArm 스텟
    public float Damage = 2f;
    public float hp = 10f;
    public Image hpBar;
    LongArmHealth hpBarScript;
    
    Rigidbody2D rb;
    CapsuleCollider2D objCollider;

    SpriteRenderer sr;

    Animator animator;

    GameObject player;
    playerMove playerScript;

    GameObject manager;
    Manager gmScript;

    //스킬 오브젝트 변수
    public GameObject HandObj;
    public Transform spawnPos;

    AudioSource sound;

    Vector3 oriPos;

    //public변수
    public Vector3 attackLength;
    public Vector3 moveLength;
    public float basicMoveSpeed;
    public float attackMoveSpeed;

    //public 변수 저장용 변수
    public float tmpHp;
    public float tmpDamage;

    //이동 관련 변수
    Vector3 PlusMoveDir;
    Vector3 MinusMoveDir;
    bool EndOfBlock = false;
    bool isMove = true;
    Vector3 moveDir;
    bool isSpawnPoint = false;

    //피격 관련 변수
    bool isAttacked = false;
    float AttackedT = 0f;

    //사망관련 변수
    bool isDeath = false;

    //handskill관련 변수
    bool usedHandSkill = false;
    int handCount = 0;
    float SpawnHandT;
    float SpawnHandDuration = 1.5f;

    //공격 관련 변수
    int attackCount = 0;
    public void publicReset()
    {
        sound.mute = true;

        //위치 초기화
        transform.position = oriPos;

        //애니메이션 초기화
        animator.SetBool("isDeath", false);

        //hp, damage 관련 변수
        hp = tmpHp;
        Damage = tmpDamage;
        
        //물리 초기화
        objCollider.isTrigger = false;
        NoFreezeRb();

        //sr초기화
        sr.color = Color.white;

        //이동 관련 변수
        EndOfBlock = false;
        isMove = true;
        moveDir = new Vector3(Random.Range(-1f, 1f), 0f, 0f);
        isSpawnPoint = false;

        //피격 관련 변수 
        isAttacked = false;
        AttackedT = 0f;

        //사망관련 변수
        isDeath = false;

        //handskill관련 변수
        handCount = 0;
        usedHandSkill = false;
        SpawnHandT = SpawnHandDuration;

        //공격 관련 변수
        attackCount = 0;

        //hp바 초기 설정
        hpBarScript.publicReset();
        hpBar.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        objCollider = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerMove>();
        hpBarScript = GetComponent<LongArmHealth>();
        manager = GameObject.FindWithTag("Manager");
        gmScript = manager.GetComponent<Manager>();
        sound = GetComponent<AudioSource>();

        oriPos = transform.position;
        PlusMoveDir = oriPos + moveLength;
        MinusMoveDir = oriPos - moveLength;

        moveDir = new Vector3(Random.Range(-1f, 1f), 0f, 0f);

        tmpHp = hp;
        tmpDamage = Damage;
        SpawnHandT = SpawnHandDuration;

        hpBar.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp > 0f)
        {
            Vector3 distance = player.transform.position - transform.position;
            if (Mathf.Abs(distance.x) < attackLength.x && Mathf.Abs(distance.y) < attackLength.y)
            {
                isMove = false; //플레이어가 거리안에 들어왔으므로 플레이어를 향해 이동
            }
            else
            {
                isMove = true;//기본이동 활성화
            }
            if (!isAttacked)
            {
                //플레이어 공격 대상이 되었을때, update문에서 피격처리 수행
                if (gameObject == playerScript.nearestObj && !animator.GetBool("isSkill"))
                {
                    if (!hpBar.enabled)
                    {
                        hpBar.enabled = true;
                    }
                    playerScript.nearestObj = null;
                    isAttacked = true;
                    sr.color = Color.red;
                    freezeRb();
                    objCollider.isTrigger = true;
                    hp -= playerScript.Damage;
                    hpBarScript.TakeDamage(playerScript.Damage);
                }

                //이동관련 코드
                if (isMove)//기본이동
                {
                    if (!(transform.position.x < PlusMoveDir.x && transform.position.y < PlusMoveDir.y
                        && transform.position.x > MinusMoveDir.x && transform.position.y > MinusMoveDir.y))
                    {
                        if (isSpawnPoint)
                        {
                            isSpawnPoint = false;
                        }
                    }
                    else if (transform.position.x < oriPos.x + 0.5f && transform.position.y < oriPos.y + 0.5f
                        && transform.position.x > oriPos.x - 0.5f && transform.position.y > oriPos.y - 0.5f)
                    {
                        if (!isSpawnPoint)
                        {
                            isSpawnPoint = true;
                            moveDir = new Vector3(Random.Range(-1f, 1f), 0f, 0f);
                        }
                    }

                    if (!animator.GetBool("isAttack"))//공격하는 동안에는 움직이지 않도록
                    {
                        if (isSpawnPoint)
                        {
                            Move();
                        }
                        else
                        {
                            MovetoSpawn();
                        }
                    }
                }
                else//공격 !isMove
                {
                    if (!animator.GetBool("isAttack"))
                    {
                        moveToPlayer();
                    }
                }
            }
            else//피격을 받았으면 실행 isAttacked
            {
                AttackedT += Time.deltaTime;
                if (AttackedT > 0.5f)
                {
                    AttackedT = 0f;
                    sr.color = Color.white;
                    objCollider.isTrigger = false;
                    NoFreezeRb();
                    isAttacked = false;
                }
            }

            if (EndOfBlock)//endofBlock에 접촉할때 실행 EndOfBlock
            {
                MovetoSpawn();
                if (transform.position.x < oriPos.x + 0.5f && transform.position.y < oriPos.y + 0.5f
                     && transform.position.x > oriPos.x - 0.5f && transform.position.y > oriPos.y - 0.5f)
                {
                    EndOfBlock = false;
                    isSpawnPoint = true;
                    moveDir = new Vector3(Random.Range(-1f, 1f), 0f, 0f);
                }
            }

            //hp가 0보다 클때, 스킬실행 관련 코드
            if (usedHandSkill)
            {
                if(playerScript.nearestObj == gameObject)
                {
                    playerScript.nearestObj = null;
                }
                SpawnHandT += Time.deltaTime;
                if (SpawnHandT > SpawnHandDuration)
                {
                    if (handCount < 3)
                    {
                        SpawnHandT = 0f;
                        Instantiate(HandObj, new Vector3(player.transform.position.x, spawnPos.position.y, 0f), Quaternion.identity);
                        handCount++;
                    }
                    else//스킬 사용 종료
                    {
                        SpawnHandT = SpawnHandDuration;
                        handCount = 0;
                        objCollider.isTrigger = false;
                        usedHandSkill = false;
                        animator.SetBool("isSkill", false);
                        attackCount = 0;
                        NoFreezeRb();
                    }
                }
            }
        }
        else //hp가 0이하
        {
            if (!isDeath)
            {
                sound.mute = false;
                sound.Play();
                playerScript.nearestObj = null;
                freezeRb();
                isDeath = true;
                animator.SetBool("isDeath", true);
            }
            if (gameObject == playerScript.nearestObj)
            {
                playerScript.nearestObj = null;
            }
        }

    }
    void Move()
    {
        if (moveDir.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        rb.velocity = moveDir.normalized * basicMoveSpeed;
    }

    void MovetoSpawn()
    {
        Vector3 dir = oriPos - transform.position;
        if (dir.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        rb.velocity = dir.normalized * basicMoveSpeed;
    }

    //애니메이션 적용 함수

    public void usingSkill1()
    {
        usedHandSkill = true;
    }
    public void StartOfSkill()
    {
        objCollider.isTrigger = true;
    }

    public void EndOfDeath() //보스 사망시 스테이지 1증가
    {
        playerScript.Damage += 0.2f * gmScript.Stage; //플레이어 데미지 증가
        playerScript.tmpDamage = playerScript.Damage;
        playerScript.textAtk.text = playerScript.Damage.ToString();

        //플레이어 hp증가
        playerScript.maxHp += 0.2f * gmScript.Stage;
        playerScript.tmpHp = playerScript.maxHp;
        playerScript.hp = playerScript.maxHp;
        playerScript.hpBarScript.TakeDamage();
        playerScript.sound.volume = 0.2f;
        playerScript.sound.clip = playerScript.audioclip[2];
        playerScript.sound.Play();

        hpBar.enabled = false;
        gmScript.Stage++;
        NoFreezeRb();
        gameObject.SetActive(false);
    }

    public void MiddleOfAttack()
    {
        freezeRb();
        objCollider.isTrigger = true;
        playerScript.AttackLongArm = true;
        attackCount++;
    }

    public void EndOfAttack()
    {
        NoFreezeRb();
        animator.SetBool("isAttack", false);
        objCollider.isTrigger = false;
        playerScript.AttackLongArm = false;
    }

    //여기까지

    void moveToPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0f;
        dir.z = 0;
        if (dir.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        rb.velocity = dir.normalized * attackMoveSpeed;
    }
    void freezeRb()
    {
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }

    void NoFreezeRb()
    {
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!isAttacked && gameObject != playerScript.nearestObj)
            {
                if (!animator.GetBool("isAttack") && !animator.GetBool("isSkill"))
                {
                    freezeRb();
                    objCollider.isTrigger = true;
                    if (attackCount < 3)
                    {
                        animator.SetBool("isAttack", true);
                    }
                    else
                    {
                        animator.SetBool("isSkill", true);
                    }
                }
            }
        }
        else if (collision.gameObject.tag == "StageWall")
        {
            if (!EndOfBlock)
            {
                EndOfBlock = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EndOfBlock")
        {
            if (!EndOfBlock)
            {
                EndOfBlock = true;
            }
        }
    }
}
