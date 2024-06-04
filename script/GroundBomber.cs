using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundBomber : MonoBehaviour
{
    //GoundBomber ����
    public float Damage = 1.5f;
    GroundBomberHealth hpBarScript;
    public float hp = 1.5f;
    public Image hpBar;

    Rigidbody2D rb;
    CapsuleCollider2D objCollider;

    SpriteRenderer sr;

    Animator animator;

    GameObject player;
    playerMove playerScript;

    GameObject gm;
    Manager gmScript;

    AudioSource sound;

    Vector3 oriPos;

    //public ����
    public Vector3 AttackDistance;
    public Vector3 moveDistance;
    public float MoveSpeed;
    public float AttackSpeed;

    //public ������ ����� ����
    public float tmpHp;
    public float tmpDamage;

    //�̵� ���� ����
    Vector3 BasicMoveVec;
    Vector3 BasicMinusMoveVec;
    
    //�ǰ� ���� ����
    bool isAttacked = false;
    float AttackedT = 0f;

    //�̵� ���� ����
    Vector3 moveDir;
    bool isMove = false;
    bool EndOfBlock = false;
    bool isSpawnPoint = false;

    //��� ���� ����
    public bool isDeath = false;

    //������ ���� ����
    public float SpawnT = 0f;
    public float SpawnDuration;
    public void publicReset()
    {
        sound.mute = true;

        //������Ʈ ��Ȱ��ȭ�� �ʱ���ġ ����
        transform.position = oriPos;

        //�ִϸ��̼� �ʱ�ȭ
        animator.SetBool("isBomb", false);

        //sr �� �ʱ�ȭ
        sr.color = Color.white;

        //rb ���� ����
        NoFreezeRb();
        objCollider.isTrigger = false;
        
        //hp �ʱ�ȭ
        hp = tmpHp;
        Damage = tmpDamage;

        //�̵����� ����
        moveDir = new Vector3(Random.Range(-1f, 1f), 0f, 0f);
        isMove = false;
        EndOfBlock = false;
        isSpawnPoint = false;

        //�ǰ� ���� ����
        isAttacked = false;
        AttackedT = 0f;

        //������� ����
        isDeath = false;

        //������ ���� ����
        SpawnT = 0f;

        //hp �ʱ� ����
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
        hpBarScript = GetComponent<GroundBomberHealth>();
        gm = GameObject.FindWithTag("Manager");
        gmScript = gm.GetComponent<Manager>();
        sound = GetComponent<AudioSource>();

        oriPos = transform.position;
        BasicMoveVec = oriPos + moveDistance;
        BasicMinusMoveVec = oriPos - moveDistance;

        tmpHp = hp;
        tmpDamage = Damage;

        moveDir = new Vector3(Random.Range(-1f, 1f), 0f, 0f);
        hpBar.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp > 0)
        {
            Vector3 distance = player.transform.position - transform.position;
            if (Mathf.Abs(distance.x) < AttackDistance.x && Mathf.Abs(distance.y) < AttackDistance.y)
            {
                isMove = false; //�÷��̾ �Ÿ��ȿ� �������Ƿ� �⺻�̵� ��Ȱ��ȭ
            }
            else
            {
                isMove = true;//�⺻�̵� Ȱ��ȭ
            }

            if (!isAttacked)
            {
                //�÷��̾� ������� ����� �Ǿ��� ���, �ǰ� �ڵ� ����
                if (gameObject == playerScript.nearestObj)
                {
                    if (!hpBar.enabled)
                    {
                        hpBar.enabled = true;
                    }
                    isAttacked = true;
                    playerScript.nearestObj = null;
                    sr.color = Color.red;
                    freezeRb();
                    objCollider.isTrigger = true;
                    hp -= playerScript.Damage;
                    hpBarScript.TakeDamage(playerScript.Damage);
                }
            }
            else //�ǰ� �޾����� ���� isAttacked
            {
                AttackedT += Time.deltaTime;
                if (AttackedT > 0.5f)
                {
                    AttackedT = 0f;
                    NoFreezeRb();
                    sr.color = Color.white;
                    isAttacked = false;
                    objCollider.isTrigger = false;
                }
            }

            //�̵����� �ڵ�
            if (!EndOfBlock)
            {
                if (isMove)//�⺻ �̵�
                {
                    if (!(transform.position.x < BasicMoveVec.x && transform.position.y < BasicMoveVec.y
                        && transform.position.x > BasicMinusMoveVec.x && transform.position.y > BasicMinusMoveVec.y))
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

                    if (isSpawnPoint)
                    {
                        basicMove();
                    }
                    else
                    {
                        moveToSpawn();
                    }
                }
                else//�÷��̾� ��ġ�� �̵�
                {
                    moveToPlayer();
                }
            }
            else //end��Ͽ� ������ �������� �̵�
            {
                moveToSpawn();
                if (transform.position.x < oriPos.x + 0.5f && transform.position.y < oriPos.y + 0.5f
                     && transform.position.x > oriPos.x - 0.5f && transform.position.y > oriPos.y - 0.5f)
                {
                    EndOfBlock = false;
                    isSpawnPoint = true;
                    moveDir = new Vector3(Random.Range(-1f, 1f), 0f, 0f);
                }
            }
        }
        else
        {
            if (!isDeath)
            {
                sound.mute = false;
                sound.Play();
                hpBar.enabled = false;
                isDeath = true;
                animator.SetBool("isBomb", true);
            }
            if (gameObject == playerScript.nearestObj)
            {
                playerScript.nearestObj = null;
            }
        }
    }

    void basicMove()
    {
        if(moveDir.x > 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        rb.velocity = moveDir.normalized * MoveSpeed;
    }

    void moveToSpawn()
    {
        Vector3 SpawnMove = oriPos - transform.position;
        if(SpawnMove.x > 0f)
        {
            transform.rotation = Quaternion.Euler(0f,180f,0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f,0f,0f);
        }
        rb.velocity = SpawnMove.normalized * MoveSpeed;
    }
    
    void moveToPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0f;
        dir.z = 0f;
        if(dir.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f,180f,0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f,0f,0f);
        }
        rb.velocity = dir.normalized * AttackSpeed;
    }
    
    public void MiddleOfExplosion()
    {
        sr.color = Color.white;
        if (!playerScript.OnGroundBomber)
        {
            playerScript.OnGroundBomber = true;
        }
    }
    public void EndOfExplosion()
    {
        if ((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 1.4611f)//�÷��̾� ���ݷ� ���� ����
        {
            playerScript.Damage += 0.15f;
            playerScript.tmpDamage = playerScript.Damage;
            playerScript.textAtk.text = playerScript.Damage.ToString();
            playerScript.sound.volume = 0.3f;
            playerScript.sound.clip = playerScript.audioclip[3];
            playerScript.sound.Play();
        }
        if ((Random.Range(0f, 100f)) <= 2.3801f * Mathf.Log10(gmScript.Stage) + 1.4611f)//�÷��̾� ���� ���� ����
        {
            playerScript.hp += 0.15f;
            playerScript.maxHp += 0.15f;
            playerScript.tmpHp = playerScript.hp;
            playerScript.hpBarScript.UpdateHealthBar();
            playerScript.sound.volume = 0.2f;
            playerScript.sound.clip = playerScript.audioclip[2];
            playerScript.sound.Play();
        }

        objCollider.isTrigger = false;
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
        if (collision.gameObject.tag == "Player")
        {
            if (!isAttacked && gameObject != playerScript.nearestObj)
            {
                freezeRb();
                objCollider.isTrigger = true;
                hp = -1f;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "EndOfBlock")
        {
            if(!EndOfBlock)
            {
                EndOfBlock = true;
            }
        }
    }
}
