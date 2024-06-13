using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeWeapon : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject player;
    Vector3 attackDir; //플레이어와 weapon사이의 거리 벡터
    float SpeedX; //시작벡터 x값
    float SpeedY; //시작벡터 y값
    Vector2 startVector; //던지는 속도 벡터
    float length; //플레이어와 weapon사이의 거리 float값임
    float degree;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        attackDir = player.transform.position - transform.position;

        //공식에서 t는 2이라고 가정함
        length = Mathf.Sqrt(Mathf.Pow(attackDir.x, 2) + Mathf.Pow(attackDir.y, 2));//빗변 / 밑면으로 해서 수정해야 할수도
        degree = Mathf.Acos(Mathf.Abs(attackDir.x) / length); //길이라서 절댓값을 취함
        SpeedX = attackDir.x / Mathf.Cos(degree);
        SpeedY = (Mathf.Abs(transform.position.y - attackDir.y) + rb.gravityScale) / Mathf.Sin(degree);

        startVector = new Vector2(SpeedX, SpeedY);
        
        //구한 속도로 impulse로 힘 추가
        rb.AddForce(startVector * rb.mass, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Player")
        {
            //피격 설정
            Destroy(gameObject);
        }
    }
}
