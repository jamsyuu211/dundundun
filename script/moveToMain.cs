using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveToMain : MonoBehaviour
{
    public Transform mainZone;
    GameObject player;
    playerMove playerScript;
    GameObject mainCamera;
    CameraFX cameraScript;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerMove>();
        mainCamera = GameObject.FindWithTag("MainCamera");
        cameraScript = mainCamera.GetComponent<CameraFX>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.gameObject.transform.position = mainZone.position;
            playerScript.isBossZone = false;
            cameraScript.isMove = true;
            cameraScript.blur.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }
}
