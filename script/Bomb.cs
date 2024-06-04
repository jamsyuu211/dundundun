using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    GameObject player;
    playerMove playerScript;
    CapsuleCollider2D objCollider;
    GameObject gm;
    Manager gmScript;

    bool isBomb = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerMove>();
        objCollider = GetComponent<CapsuleCollider2D>();
        gm = GameObject.FindWithTag("Manager");
        gmScript = gm.GetComponent<Manager>();
    }
    private void Update()
    {
        if (gmScript.Restart)
        {
            Destroy(gameObject);
        }
    }

    public void EndOfBomb()
    {
        Destroy(gameObject);
    }
    void freezeRb()
    {
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground" || collision.tag == "DeathBlock") {
            if (!isBomb)
            {
                freezeRb();
                isBomb = true;
                animator.SetTrigger("Explosion");
                objCollider.isTrigger = false;
            }
        }
    }
}
