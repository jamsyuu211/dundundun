using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBlock : MonoBehaviour
{
    Rigidbody2D rb;
    Vector3 oriPos;
    Vector3 moveDir;
    public Transform Target;
    public float moveSpeed;
    bool isMove;
    public void publicReset()
    {
        transform.position = oriPos;
        isMove = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        oriPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)//목표 위치로 이동
        {
            moveDir = Target.position - transform.position;
            if (moveDir.y > 0f)
            {
                transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
            }
        }
        else //스폰위치로 이동
        {
            moveDir = oriPos - transform.position;
            if(moveDir.y < 0f)
            {
                transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!isMove)
            {
                isMove = true;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (isMove)
            {
                isMove = false;
            }
        }
    }
}
