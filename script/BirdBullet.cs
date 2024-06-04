using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;

public class BirdBullet : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    GameObject player;
    playerMove playerScript;
    GameObject gm;
    Manager gmScript;

    Vector2 moveDir;
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerMove>();
        gm = GameObject.FindWithTag("Manager");
        gmScript = gm.GetComponent<Manager>();

        moveDir = player.transform.position - transform.position;

        if (moveDir.x > 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));            
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }

        rb.velocity = moveDir * moveSpeed;
    }
    private void Update()
    {
        if (gmScript.Restart)
        {
            Destroy(gameObject);
        }
    }
    void EndOfBomb()
    {
        Destroy(gameObject);
    }

    public void freezeRb()
    {
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            animator.SetBool("isBomb", true);
            freezeRb();
        }
        else if(collision.tag != "Bird")
        {
            animator.SetBool("isBomb", true);
            freezeRb();
        }
    }
}
