using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class moveHand : MonoBehaviour
{
    Rigidbody2D rb;
    PolygonCollider2D objCollider;
    public float moveSpeed;
    Vector3 oriPos;
    Vector3 moveRange = new Vector3(0f, 15f, 0f);
    Vector3 moveDistance;
    bool isTop = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        objCollider = GetComponent<PolygonCollider2D>();
        oriPos = transform.position;
        moveDistance = oriPos + moveRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTop)//시작지점이라 상승
        {
            if(transform.position.y < moveDistance.y)
            {
                rb.velocity = Vector3.up * moveSpeed;
            }
            else
            {
                isTop = true;
            }
        }
        else //최고지점이라 하강
        {
            if(transform.position.y > oriPos.y)
            {
                rb.velocity = Vector3.down * moveSpeed;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
