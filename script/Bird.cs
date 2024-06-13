using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bird : MonoBehaviour
{
    //Dog 공격, hp 스텟
    public float Damage = 0.7f;
    public float hp = 2.3f;
    BirdHealth hpBarScript;
    public Image hpBar;

    //물리관련 변수
    Rigidbody2D rb;
    CapsuleCollider2D objCollider;

    //애니메이션 관련 변수
    Animator animator;

    //플레이어 관련 변수
    GameObject player;
    playerMove playerScript;

    //sr
    SpriteRenderer sr;

    GameObject gm;
    Manager gmScript;

    //사운드 관련 변수
    AudioSource sound;

    //공격 제어, 거리 관련 변수
    Vector3 oriPos;

    //public 변수
    public float moveSpeed;
    public float moveToSpawnSpeed;
    public Vector2 AttackLength;
    public Vector3 moveRange;
    public float AttackDuration;
    public Transform firePos;
    public GameObject bullet;

    //public 변수값 임시저장 변수
    float tmpMoveSpeed;
    float tmpMoveToSpawnSpeed;
    bool tmpEndOfBlock;
    Vector2 tmpAttackLength;
    public float tmpHp;
    public float tmpDamage;
    Vector3 tmpMoveRange;
    float tmpAttackDuration;

    //기본 이동 관련 변수
    bool isMove;
    Vector3 targetPos;
    Vector3 moveDistance;
    Vector3 moveMinusDistance;
    bool OutOfRange = false;
    Vector3 basicMove;

    //피격 및 hp관련 변수
    bool isAttacked = false;
    float AttackedT = 0f;
    bool isDeath = false;

    //공격 관련 변수
    float attackT = 0f;
    Vector3 distance;

    //리스폰 관련 변수
    public float SpawnT = 0f;
    public float SpawnDuration;

    public void publicReset()
    {
        sound.mute = true;

        //오브젝트 재활성화시 초기위치 설정
        transform.position = oriPos;

        //애니메이션 초기화
        animator.SetBool("isDeath", false);

        //sr 색 초기화
        sr.color = Color.white;

        //rb 고정 해제
        NoFreezeRb();
        objCollider.isTrigger = false;

        //public 변수 초기화
        moveSpeed = tmpMoveSpeed;
        AttackLength = tmpAttackLength;
        hp = tmpHp;
        Damage = tmpDamage;
        moveRange = tmpMoveRange;
        moveToSpawnSpeed = tmpMoveToSpawnSpeed;
        AttackDuration = tmpAttackDuration;

        //이동관련 변수
        isMove = true;
        targetPos = Vector3.zero;
        OutOfRange = false;
        basicMove = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        //피격 및 hp관련 변수
        isAttacked = false;
        AttackedT = 0f;
        isDeath = false;

        //공격 관련 변수
        attackT = 0f;
        distance = Vector3.zero;

        //리스폰 관련 변수
        SpawnT = 0f;

        //hp바 초기 상태 설정
        hpBarScript.publicReset();
        hpBar.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerMove>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        objCollider = GetComponent<CapsuleCollider2D>();
        hpBarScript = GetComponent<BirdHealth>();
        gm = GameObject.FindWithTag("Manager");
        gmScript = gm.GetComponent<Manager>();
        sound = GetComponent<AudioSource>();

        //오브젝트 생성시 초기 위치 저장
        oriPos = transform.position;
        moveDistance = oriPos + moveRange;
        moveMinusDistance = oriPos - moveRange;

        tmpMoveSpeed = moveSpeed;
        tmpAttackLength = AttackLength;
        tmpHp = hp;
        tmpDamage = Damage;
        tmpMoveRange = moveRange;
        tmpMoveToSpawnSpeed = moveToSpawnSpeed;
        tmpAttackDuration = AttackDuration;

        basicMove = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);

        hpBar.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp > 0)
        {
            if (!isAttacked)
            {
                //플레이어 스킬 적용 대상이되면 피격
                if (gameObject == playerScript.nearestObj)
                {
                    if (!hpBar.enabled)
                    {
                        hpBar.enabled = true;
                    }
                    freezeRb();
                    playerScript.nearestObj = null;
                    isAttacked = true;
                    hp -= playerScript.Damage;
                    hpBarScript.TakeDamage(playerScript.Damage);
                    sr.color = Color.red;
                    objCollider.isTrigger = true;
                }
          

                //이동 관련 코드
                distance = player.transform.position - transform.position;

                if (isMove)
                {
                    if (Mathf.Abs(distance.x) < AttackLength.x && Mathf.Abs(distance.y) < AttackLength.y) //플레이어가 범위안으로 들어오면 공격으로 변경
                    {
                        isMove = false;
                        freezeRb();
                    }
                    else
                    {
                        move();
                    }
                }
                else
                {
                    if (Mathf.Abs(distance.x) >= AttackLength.x || Mathf.Abs(distance.y) >= AttackLength.y) //플레이어가 범위를 벗어나면 이동으로 변경
                    {
                        isMove = true;
                        NoFreezeRb();
                    }
                    else
                    {
                        if (distance.x > 0f)
                        {
                            transform.rotation = Quaternion.Euler(new Vector2(0f, 180f));
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(new Vector2(0f, 0f));
                        }
                        attackT += Time.deltaTime;
                        if (attackT > AttackDuration)
                        {
                            Attack();
                            attackT = 0f;
                        }
                    }
                }

            }
            else//피격을 당했으면 isAttacked
            {
                AttackedT += Time.deltaTime;
                if (AttackedT > 0.5f)
                {
                    AttackedT = 0f;
                    isAttacked = false;
                    sr.color = Color.white;
                    objCollider.isTrigger = false;
                    NoFreezeRb();
                }
            }
        }
        else
        {
            if (!isDeath)
            {
                sound.mute = false;
                sound.Play();
                isDeath = true;
                animator.SetBool("isDeath", true);
                playerScript.nearestObj = null;
            }
            if(gameObject == playerScript.nearestObj)
            {
                playerScript.nearestObj = null;
            }
        }
    }

    void move()
    {
        if(!OutOfRange)//이동범위 안에 있으면
        {
            //이동할 위치가 범위를 벗어나면 변경
            if (transform.position.x < moveDistance.x && transform.position.y < moveDistance.y
                && transform.position.x > moveMinusDistance.x && transform.position.y > moveMinusDistance.y)//moveDistance는 스폰지점에서부터 움직일 수 있는 범위를 나타내는 벡터임
            {
                rb.velocity = basicMove.normalized * moveSpeed;
                if(rb.velocity.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0f,0f,0f);
                }
            }
            else
            {
                OutOfRange = true;
            }
        }
        else //위치가 이동가능범위를 넘어가면
        {
            Vector3 moveDirection = oriPos - transform.position;
            if (Mathf.Abs(moveDirection.x) < 0.5f && Mathf.Abs(moveDirection.y) < 0.5f)
            {
                OutOfRange = false;
                basicMove = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
            else
            {
                moveToSpawn(moveDirection);
            }
        }
    }

    void moveToSpawn(Vector3 moveDir)
    {
        rb.velocity = moveDir.normalized * moveToSpawnSpeed;
        if (rb.velocity.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    void Attack()
    {
        Instantiate(bullet, firePos.position, Quaternion.identity);
    }
    public void StartOfDeath()
    {
        NoFreezeRb();
        rb.gravityScale = 2f;
        objCollider.isTrigger = true;
    }

    public void EndOfDeath1()
    {
        freezeRb();
        rb.gravityScale = 0f;
        animator.SetTrigger("Death2");
    }

    public void EndOfDeath()
    {
        hpBar.enabled = false;

        if ((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 0.4611f)//플레이어 공격력 스텟 증가
        {
            playerScript.Damage += 0.05f;
            playerScript.tmpDamage = playerScript.Damage;
            playerScript.textAtk.text = playerScript.Damage.ToString("F1");
            playerScript.sound.volume = 0.6f;
            playerScript.sound.clip = playerScript.audioclip[3];
            playerScript.sound.Play();
        }
        if ((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 0.4611f)//플레이어 생명 스텟 증가
        {
            playerScript.hp += 0.05f;
            playerScript.maxHp += 0.05f;
            playerScript.tmpHp = playerScript.hp;
            playerScript.hpBarScript.UpdateHealthBar();
            playerScript.sound.volume = 0.4f;
            playerScript.sound.clip = playerScript.audioclip[2];
            playerScript.sound.Play();
        }

        objCollider.isTrigger = false;
        animator.SetBool("isDeath", false);
        gameObject.SetActive(false);
    }
    public void freezeRb()
    {
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }

    public void NoFreezeRb()
    {
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Flow")
        {
            if (!isAttacked && hp > 0f)
            {
                if (!hpBar.enabled)
                {
                    hpBar.enabled = true;
                }
                freezeRb();
                playerScript.nearestObj = null;
                isAttacked = true;
                hp -= playerScript.Damage;
                hpBarScript.TakeDamage(playerScript.Damage);
                sr.color = Color.red;
                objCollider.isTrigger = true;
            }
        }
        else if (collision.tag == "Ground" || collision.tag == "DeathBlock")
        {
            freezeRb();
            rb.gravityScale = 0f;
            animator.SetTrigger("Death2");
        }
    }
}
