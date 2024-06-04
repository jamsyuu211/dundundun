using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dog : MonoBehaviour
{
    //Dog ����, hp ����
    public float Damage = 0.5f;
    public float hp = 3.2f;
    DogHealth hpBarScript;
    public Image hpBar;

    //���� ���� ����
    public Rigidbody2D rb;
    CapsuleCollider2D ObjCollider;

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

    //y�� ���� ���� ����
    public bool endOfBlock = false;
    float oriLength = 3f;

    //�̵� ���� ����
    public float BasicmoveSpeed;
    public float moveLength = 5f;
    bool moveDirection = false;

    //���� ����, �Ÿ� ���� ����
    public Vector2 attackLength;
    Vector2 distance;
    Vector3 oriPos;

    //�ȴ� �ִϸ��̼� ���� ����
    bool isWalk = true;

    //����1 ���� ����
    public float moveSpeed1;
    bool isAttack1 = false;
    int attack1Count = 0;

    //����2 ���� ����
    public float moveSpeed2;
    Vector2 DashDir;
    bool isDashOver = true;

    //������� ����
    bool isDeath = false;
    bool isAttacked = false;
    float ColorT = 0f;


    //public �ӽ����� ����
    bool tmpEndOfBlock;
    float tmpBasicMoveSpeed;
    Vector2 tmpAttackLength;
    float tmpMoveSpeed1;
    float tmpMoveSpeed2;
    public float tmpHp;
    public float tmpDamage;

    //������ ���� ����
    public float SpawnT = 0f;
    public float SpawnDuration;
    public void publicReset()
    { 
        endOfBlock = tmpEndOfBlock;
        BasicmoveSpeed = tmpBasicMoveSpeed + Random.Range(-3f, 3f);
        attackLength = tmpAttackLength;
        moveSpeed1 = tmpMoveSpeed1 + Random.Range(-3f, 3f);
        moveSpeed2 = tmpMoveSpeed2 + Random.Range(-5f, 5f);
        hp = tmpHp;
        Damage = tmpDamage;

        //�Ҹ� ���� ����
        sound.mute = true;

        //y�� ���� ���� ����
        oriLength = 3f;

        //�̵� ���� ����
        moveDirection = false;

        //���� ����, �Ÿ� ���� ����
        distance = Vector2.zero;
        
        //�ȴ� �ִϸ��̼� ���� ����
        isWalk = true;

        //����1 ���� ����
        isAttack1 = false;
        attack1Count = 0;

        //����2 ���� ����
        DashDir = Vector2.zero;
        isDashOver = true;

        //hp �� ������� ����
        isDeath = false;
        isAttacked = false;
        ColorT = 0f;

        //������ ���� ����
        SpawnT = 0f;

        sr.color = Color.white;
        ObjCollider.isTrigger = false;
        transform.position = oriPos;
        NoFreezeRb();

        //hp�� �ʱ� ����
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
        ObjCollider = GetComponent<CapsuleCollider2D>();
        hpBarScript = GetComponent<DogHealth>();
        gm = GameObject.FindWithTag("Manager");
        gmScript = gm.GetComponent<Manager>();
        sound = GetComponent<AudioSource>();

        oriPos = transform.position;
        

        tmpEndOfBlock = endOfBlock;
        tmpBasicMoveSpeed = BasicmoveSpeed;
        tmpAttackLength = attackLength;
        tmpMoveSpeed1 = moveSpeed1;
        tmpMoveSpeed2 = moveSpeed2;
        tmpHp = hp;
        tmpDamage = Damage;

        hpBar.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp > 0)
        {
            if (!(transform.position.y < oriPos.y + 1.5f && transform.position.y > oriPos.y - 1.5f))
            {
                transform.position = oriPos; // �̵����� ������ ������ ������������ �̵�
            }
            if (!endOfBlock)
            {
                if (!isAttacked)
                {
                    //�÷��̾� ��ų ����� �Ǿ� �ǰ��� ���ϸ� ����
                    if (gameObject == playerScript.nearestObj)
                    {
                        if(!hpBar.enabled)
                        {
                            hpBar.enabled = true;
                        }
                        freezeRb();
                        playerScript.nearestObj = null;
                        isAttacked = true;
                        hp -= playerScript.Damage;
                        hpBarScript.TakeDamage(playerScript.Damage);
                        sr.color = Color.red;
                        ObjCollider.isTrigger = true;
                    }


                    //�̵����� �ڵ�
                    if (isWalk) //�ȴ� ���¸� == �ǰݹ����� ����x
                    {
                        distance = player.transform.position - transform.position;
                        if ((distance.x < attackLength.x && distance.x > -attackLength.x) && (distance.y < attackLength.y && distance.y > -attackLength.y))
                        {
                            if (attack1Count < 3)
                            {
                                Attack1Move();
                                if (isAttack1 && isDashOver)
                                {
                                    isWalk = false; //������ ���� �̵�x
                                    Attack1();
                                }
                            }
                            else //�⺻���� 3ȸ�� ��ų2�� �뽬 ����
                            {
                                isWalk = false; //�ѹ��� �����ϵ��� ����
                                isDashOver = false;
                                attack1Count = 0;
                                Attack2();
                            }
                        }
                        else
                        {
                            spawnMove();
                        }
                    }
                    if (!isDashOver)//�뽬�߿� ���� �ٲܼ� �ֵ���
                    {
                        Vector2 moveDir = player.transform.position - transform.position;
                        if (moveDir.x > 0)
                        {
                            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        }
                        else if (moveDir.x < 0)
                        {
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }
                    }
                }
                //�ǰ� ó��
                else
                {
                    ColorT += Time.deltaTime;
                    if (ColorT > 0.5f)
                    {
                        ColorT = 0f;
                        isAttacked = false;
                        sr.color = Color.white;
                        ObjCollider.isTrigger = false;
                        if (isDashOver)
                        {
                            NoFreezeRb();
                        }
                    }
                }
            }
            else //���� ���� �����ϸ� == �ȶ���������
            {
                EndOfBlockMove();
            }
        }
        else   //dog ������� �ڵ�
        {
            if (!isDeath)
            {
                sound.mute = false;
                sound.Play();
                playerScript.nearestObj = null;
                isDeath = true;
                freezeRb();
                ObjCollider.isTrigger = true;
                animator.SetBool("isDeath", true);
            }
            if(gameObject == playerScript.nearestObj)
            {
                playerScript.nearestObj = null;
            }
        }
    }

    void spawnMove()
    {
        if (transform.position.x > oriPos.x + moveLength)
        {
            moveDirection = true;
        }
        else if(transform.position.x < oriPos.x - moveLength)
        {
            moveDirection = false;
        }
        
        if (moveDirection)
        {
            rb.velocity = Vector2.left * BasicmoveSpeed;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            rb.velocity = Vector2.right * BasicmoveSpeed;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
    void EndOfBlockMove()
    {
        //���� ���� �����ϸ� ������������ ���ư�
        NoFreezeRb();
        Vector3 length = oriPos - transform.position;
        if (length.x < oriLength && length.x > -oriLength)
        {
            endOfBlock = false;
        }


        if (length.x < 0)
        {
            rb.velocity = new Vector2(-BasicmoveSpeed * 3, rb.velocity.y);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (length.x > 0)
        {
            rb.velocity = new Vector2(BasicmoveSpeed * 3, rb.velocity.y);
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    void Attack1Move()
    {
        if (isWalk && gameObject != playerScript.nearestObj)
        {
            Vector2 moveDir = player.transform.position - transform.position;
            
            if (moveDir.x < 0)
            {
                rb.velocity = new Vector2(-moveSpeed1, rb.velocity.y);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (moveDir.x > 0)
            {
                rb.velocity = new Vector2(moveSpeed1, rb.velocity.y);
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }
    }
    void Attack1()
    {
        freezeRb();
        ObjCollider.isTrigger = true;
        animator.SetBool("isAttack1", true);
    }

    public void Attack1End()
    {
        ObjCollider.isTrigger = false;
        NoFreezeRb();
        attack1Count++;
        isAttack1 = false;
        animator.SetBool("isAttack1", false);
        isWalk = true;
    }

    void Attack2()
    {
        freezeRb();
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Attack2");
    }

    public void Attack2Dash()
    {
        NoFreezeRb();
        DashDir = (player.transform.position - transform.position).normalized;
        DashDir.y = 0f;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
        rb.velocity = DashDir * moveSpeed2;
    }
        
    public void Attack2End()
    {
        NoFreezeRb();
        animator.SetTrigger("Attack2");
        isWalk = true;
        isDashOver = true;
    }

    public void EndOfDeath()
    {
        hpBar.enabled = false;
        if((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 0.4611f)//�÷��̾� ���ݷ� ���� ����
        {
            playerScript.Damage += 0.05f;
            playerScript.tmpDamage = playerScript.Damage;
            playerScript.textAtk.text = playerScript.Damage.ToString();
            playerScript.sound.volume = 0.3f;
            playerScript.sound.clip = playerScript.audioclip[3];
            playerScript.sound.Play();
        }
        if ((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 0.4611f)//�÷��̾� ���� ���� ����
        {
            playerScript.hp += 0.05f;
            playerScript.maxHp += 0.05f;
            playerScript.tmpHp = playerScript.hp;
            playerScript.hpBarScript.UpdateHealthBar();
            playerScript.sound.volume = 0.2f;
            playerScript.sound.clip = playerScript.audioclip[2];
            playerScript.sound.Play();
        }
        
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (!isAttacked && !isAttack1)
            {
                isAttack1 = true;
            }
        }
    }
}