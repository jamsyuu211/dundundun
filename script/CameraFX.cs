using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFX : MonoBehaviour
{
    GameObject player;
    public SpriteRenderer blur;
    public bool isMove = false;
    public float moveSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    
    // Update is called once per frame
    void Update()
    {
        //isMove는 포탈 스크립트에서 변경됨
        if(isMove)
        { 
            blur.color = Color.Lerp(blur.color, new Color(1f,1f,1f,0f) , Time.deltaTime * 10);
            if(blur.color == Color.clear)
            {
                isMove = false;
            }
        }    
    }

    private void LateUpdate()
    {
        transform.position = player.transform.position - new Vector3(0f, 0f, 20f);
    }
}
