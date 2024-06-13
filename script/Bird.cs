using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bird : MonoBehaviour
{
    //Dog ����, hp ����
    public float Damage = 0.7f;
    public float hp = 2.3f;
    BirdHealth hpBarScript;
    public Image hpBar;

    //�������� ����
    Rigidbody2D rb;
    CapsuleCollider2D objCollider;

    //�ִϸ��̼� ���� ����
    Animator animator;

    //�÷��̾� ���� ����
    GameObject player;
    playerMove playerScript;

    //sr
    SpriteRenderer sr;

    GameObject gm;
    Manager gmScript;

    //���� ���� ����
    AudioSource sound;

    //���� ����, �Ÿ� ���� ����
    Vector3 oriPos;

    //public ����
    public float moveSpeed;
    public float moveToSpawnSpeed;
    public Vector2 AttackLength;
    public Vector3 moveRange;
    public float AttackDuration;
    public Transform firePos;
    public GameObject bullet;

    //public ������ �ӽ����� ����
    float tmpMoveSpeed;
    float tmpMoveToSpawnSpeed;
    bool tmpEndOfBlock;
    Vector2 tmpAttackLength;
    public float tmpHp;
    public float tmpDamage;
    Vector3 tmpMoveRange;
    float tmpAttackDuration;

    //�⺻ �̵� ���� ����
    bool isMove;
    Vector3 targetPos;
    Vector3 moveDistance;
    Vector3 moveMinusDistance;
    bool OutOfRange = false;
    Vector3 basicMove;

    //�ǰ� �� hp���� ����
    bool isAttacked = false;
    float AttackedT = 0f;
    bool isDeath = false;

    //���� ���� ����
    float attackT = 0f;
    Vector3 distance;

    //������ ���� ����
    public float SpawnT = 0f;
    public float SpawnDuration;

    public void publicReset()
    {
        sound.mute = true;

        //������Ʈ ��Ȱ��ȭ�� �ʱ���ġ ����
        transform.position = oriPos;

        //�ִϸ��̼� �ʱ�ȭ
        animator.SetBool("isDeath", false);

        //sr �� �ʱ�ȭ
        sr.color = Color.white;

        //rb ���� ����
        NoFreezeRb();
        objCollider.isTrigger = false;

        //public ���� �ʱ�ȭ
        moveSpeed = tmpMoveSpeed;
        AttackLength = tmpAttackLength;
        hp = tmpHp;
        Damage = tmpDamage;
        moveRange = tmpMoveRange;
        moveToSpawnSpeed = tmpMoveToSpawnSpeed;
        AttackDuration = tmpAttackDuration;

        //�̵����� ����
        isMove = true;
        targetPos = Vector3.zero;
        OutOfRange = false;
        basicMove = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        //�ǰ� �� hp���� ����
        isAttacked = false;
        AttackedT = 0f;
        isDeath = false;

        //���� ���� ����
        attackT = 0f;
        distance = Vector3.zero;

        //������ ���� ����
        SpawnT = 0f;

        //hp�� �ʱ� ���� ����
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

        //������Ʈ ������ �ʱ� ��ġ ����
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
                //�÷��̾� ��ų ���� ����̵Ǹ� �ǰ�
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
          

                //�̵� ���� �ڵ�
                distance = player.transform.position - transform.position;

                if (isMove)
                {
                    if (Mathf.Abs(distance.x) < AttackLength.x && Mathf.Abs(distance.y) < AttackLength.y) //�÷��̾ ���������� ������ �������� ����
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
                    if (Mathf.Abs(distance.x) >= AttackLength.x || Mathf.Abs(distance.y) >= AttackLength.y) //�÷��̾ ������ ����� �̵����� ����
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
            else//�ǰ��� �������� isAttacked
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
        if(!OutOfRange)//�̵����� �ȿ� ������
        {
            //�̵��� ��ġ�� ������ ����� ����
            if (transform.position.x < moveDistance.x && transform.position.y < moveDistance.y
                && transform.position.x > moveMinusDistance.x && transform.position.y > moveMinusDistance.y)//moveDistance�� ���������������� ������ �� �ִ� ������ ��Ÿ���� ������
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
        else //��ġ�� �̵����ɹ����� �Ѿ��
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

        if ((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 0.4611f)//�÷��̾� ���ݷ� ���� ����
        {
            playerScript.Damage += 0.05f;
            playerScript.tmpDamage = playerScript.Damage;
            playerScript.textAtk.text = playerScript.Damage.ToString("F1");
            playerScript.sound.volume = 0.6f;
            playerScript.sound.clip = playerScript.audioclip[3];
            playerScript.sound.Play();
        }
        if ((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 0.4611f)//�÷��̾� ���� ���� ����
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
