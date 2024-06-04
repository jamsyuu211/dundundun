using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeWeapon : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject player;
    Vector3 attackDir; //�÷��̾�� weapon������ �Ÿ� ����
    float SpeedX; //���ۺ��� x��
    float SpeedY; //���ۺ��� y��
    Vector2 startVector; //������ �ӵ� ����
    float length; //�÷��̾�� weapon������ �Ÿ� float����
    float degree;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        attackDir = player.transform.position - transform.position;

        //���Ŀ��� t�� 2�̶�� ������
        length = Mathf.Sqrt(Mathf.Pow(attackDir.x, 2) + Mathf.Pow(attackDir.y, 2));//���� / �ظ����� �ؼ� �����ؾ� �Ҽ���
        degree = Mathf.Acos(Mathf.Abs(attackDir.x) / length); //���̶� ������ ����
        SpeedX = attackDir.x / Mathf.Cos(degree);
        SpeedY = (Mathf.Abs(transform.position.y - attackDir.y) + rb.gravityScale) / Mathf.Sin(degree);

        startVector = new Vector2(SpeedX, SpeedY);
        
        //���� �ӵ��� impulse�� �� �߰�
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
            //�ǰ� ����
            Destroy(gameObject);
        }
    }
}
