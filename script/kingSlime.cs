using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class kingSlime : MonoBehaviour
{
    //���� �Ŵ��� ���� ����
    GameObject manager;
    Manager managerScript;

    //���� ���� ����
    int pattern = 1;

    //������ �̵� �� �ִϸ��̼� ���� ����
    Animator animator;
    public bool isMove = false;
    public float moveSpeed;
    public float moveT = 0f;
    public float moveDuration;
    GameObject player;
    Vector2 moveDir;
    Rigidbody2D rb;

    //������ ���� ���� ����
    public GameObject[] weapon;
    float weaponT = 0f;
    public Transform attackpos;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("Manager");
        managerScript = manager.GetComponent<Manager>();    
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMove)
        {
            moveDir = (player.transform.position - transform.position).normalized;
            moveDir.y = 0;
            moveT += Time.deltaTime;
            if (moveT > moveDuration)
            {
                moveT = 0f;
                rb.AddForce(moveDir * moveSpeed * rb.mass, ForceMode2D.Impulse);
                animator.SetBool("isMove", true);
                isMove = true;
            }
        }
        
        if (pattern == 1)
        {
            weaponT += Time.deltaTime;
            if (weaponT > 0.5f)
            {
                weaponAttackFunc();
                weaponT = 0f;
            }
        }
    }
    
    void weaponAttackFunc()
    {
        int weaponIndex = Random.Range(0, 2);
        Instantiate(weapon[weaponIndex], attackpos.position, Quaternion.identity);
    }
    public void OnAnimationEnd()
    {
        isMove = false;
        animator.SetBool("isMove", false);
        rb.velocity = Vector2.zero;
    }
}
