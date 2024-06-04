using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFX : MonoBehaviour
{
    Camera mainCamera;
    public SpriteRenderer blur;
    public bool isMove = false;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
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
}
