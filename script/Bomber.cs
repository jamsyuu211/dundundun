using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomber : MonoBehaviour
{
    //bomber ����, hp ����
    public float Damage = 1.0f;
    public float hp = 1f;
    BomberHealth hpBarScript;
    public Image hpBar;

    Rigidbody2D rb;
    CapsuleCollider2D objCollider;

    Animator animator;
    
    GameObject player;
    playerMove playerScript;

    SpriteRenderer sr;

    public GameObject bomb;
    public Transform bombPos;

    GameObject gm;
    Manager gmScript;

    AudioSource sound;

    //���� ��ġ���� ����
    Vector3 oriPos;

    //public ����
    public float basicMoveSpeed;
    public float moveSpeed;
    public bool isDeath;
    public Vector3 AttackLength;
    public Vector3 moveLength;
    public Vector3 BombAttackLength;
    public float BombDuration;

    //public ���� �� ����� ����
    public float tmpHp;
    public float tmpDamage;

    //�ǰ� ���� ����
    bool isAttacked;
    float AttackedT;

    //�̵����� ����
    bool isMove;
    Vector3 basicMoveLength;
    Vector3 basicMinusMoveLength;
    Vector3 basicMoveDir;
    bool isSpawn = false;

    //��ź ���ݰ��� ����
    float BombT = 0f;

    //������Ʈ ������� ����
    bool SelfBomb = false;

    //������ ���� ����
    public float spawnT = 0f;
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

        //�߷� �ʱ�ȭ
        rb.gravityScale = 0f;

        //public ����
        hp = tmpHp;
        Damage = tmpDamage;

        //�ǰ� ���ú���
        isAttacked = false;
        AttackedT = 0f;

        //������Ʈ ������� ����
        isDeath = false;

        //�̵� ���� ����
        isMove = false;
        basicMoveDir = new Vector3(Random.Range(-1f, 1f), 0f, 0f);
        isSpawn = false;

        //������ ���� ����
        spawnT = 0f;

        //��ź ���� ���� ����
        BombT = 0f;

        //��� ���� ����
        SelfBomb = false;

        //hpBar �ʱ���� ����
        hpBarScript.publicReset();
        hpBar.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        objCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerMove>();
        sr = GetComponent<SpriteRenderer>();
        hpBarScript = GetComponent<BomberHealth>();
        gm = GameObject.FindWithTag("Manager");
        gmScript = gm.GetComponent<Manager>();
        sound = GetComponent<AudioSource>();

        oriPos = transform.position;
        basicMoveLength = oriPos + moveLength;
        basicMinusMoveLength = oriPos - moveLength;
        basicMoveDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);

        tmpHp = hp;
        tmpDamage = Damage;

        hpBar.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(hp > 0)
        {
            if (!isAttacked) //���ݹ��� �ʾ������� ������ �� �ֵ���
            {
                //�ǰ� ���� �ڵ�
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
                Vector3 distance = player.transform.position - transform.position;
                if(Mathf.Abs(distance.x) < AttackLength.x && Mathf.Abs(distance.y) < AttackLength.y) //�÷��̾ ���������� ������
                {
                    isMove = false;
                }
                else
                {
                    isMove = true;
                }

                if(isMove)//�⺻�̵�
                {
                    if(!(transform.position.x < basicMoveLength.x && transform.position.y < basicMoveLength.y 
                        && transform.position.x > basicMinusMoveLength.x && transform.position.y > basicMinusMoveLength.y))
                    {
                        if (isSpawn)
                        {
                            isSpawn = false;
                        }
                    }
                    else if(transform.position.x < oriPos.x + 0.5f && transform.position.y < oriPos.y + 0.5f
                        && transform.position.x > oriPos.x - 0.5f && transform.position.y > oriPos.y - 0.5f)
                    {
                        if (!isSpawn)
                        {
                            isSpawn = true;
                            basicMoveDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                        }
                    }
                    
                    if(isSpawn)
                    {
                        BasicMove();
                    }
                    else
                    {
                        moveToSpawn();
                    }
                }
                else//�÷��̾ ���� ������ ������ ����
                {
                    moveToPlayer();
                    Vector3 length = player.transform.position - transform.position;
                    if (Mathf.Abs(length.x) < BombAttackLength.x)
                    {
                        BombT += Time.deltaTime;
                    }
                    if(BombT > BombDuration)
                    {
                        BombT = 0f;
                        BombAttack();
                    }
                }
            }
            else//�ǰ� ���� ó�� �ڵ�
            {
                AttackedT += Time.deltaTime;
                if (AttackedT > 0.5f)
                {
                    AttackedT = 0f;
                    isAttacked = false;
                    sr.color = Color.white;
                    NoFreezeRb();
                    objCollider.isTrigger = false;
                }
            }
        }
        else
        {
            if(!isDeath)
            {
                sr.color = Color.white;
                sound.mute = false;
                sound.Play();
                hpBar.enabled = false;
                playerScript.nearestObj = null;
                isDeath = true;
                objCollider.isTrigger = false;
                rb.gravityScale = 3f;
                animator.SetBool("isDeath", true);
                NoFreezeRb();
                rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
                rb.freezeRotation = true;
            }
            if(gameObject == playerScript.nearestObj)
            {
                playerScript.nearestObj = null;
            }
        }
    }
    
    void BasicMove()
    {
        if(basicMoveDir.x > 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        rb.velocity = basicMoveDir.normalized * basicMoveSpeed;
    }

    void moveToSpawn()
    {
        Vector3 spawnMove = oriPos - transform.position;
        if(spawnMove.x > 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        rb.velocity = spawnMove.normalized * basicMoveSpeed;
    }
    void moveToPlayer()
    {
        Vector3 moveDir = player.transform.position - transform.position;
        moveDir.y = 0f;
        moveDir.z = 0f;
        if(moveDir.x > 1f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }
        else if(moveDir.x < -1f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
        rb.velocity = moveDir.normalized * moveSpeed;
    }

    void BombAttack()
    {
        Instantiate(bomb, bombPos.position, Quaternion.identity);
    }
    public void MiddleOfBomb()
    {
        sr.color = Color.white;       
    }
    
    public void EndOfBomb()
    {
        if ((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 1.4611f)//�÷��̾� ���ݷ� ���� ����
        {
            playerScript.Damage += 0.1f;
            playerScript.tmpDamage = playerScript.Damage;
            playerScript.textAtk.text = playerScript.Damage.ToString("F1");
            playerScript.sound.volume = 0.6f;
            playerScript.sound.clip = playerScript.audioclip[3];
            playerScript.sound.Play();
        }
        if ((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 1.4611f)//�÷��̾� ���� ���� ����
        {
            playerScript.hp += 0.1f;
            playerScript.maxHp += 0.1f;
            playerScript.tmpHp = playerScript.hp;
            playerScript.hpBarScript.UpdateHealthBar();
            playerScript.sound.volume = 0.4f;
            playerScript.sound.clip = playerScript.audioclip[2];
            playerScript.sound.Play();
        }

        NoFreezeRb();
        gameObject.SetActive(false);
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
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "DeathBlock" || collision.gameObject.tag == "Player")
        {
            if (!SelfBomb)
            {
                objCollider.isTrigger = false;
                SelfBomb = true;
                freezeRb();
                animator.SetTrigger("Explosion");
                if (!playerScript.OnBomber)
                {
                    playerScript.OnBomber = true;
                }
            }
        }
        else if (collision.gameObject.tag == "StageWall" || collision.gameObject.tag == "Ground")
        {
            basicMoveDir = new Vector3(Random.Range(-1f, 1f), 0f, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            sr.color = Color.white;
            isAttacked = false;
            objCollider.isTrigger = false;
            NoFreezeRb();
        }
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
    }
}
