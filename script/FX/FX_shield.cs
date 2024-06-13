using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_shield : MonoBehaviour
{
    public Material material;
    GameObject player;
    playerMove playerScript;
    Color oriColor;
    bool isChanged = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerMove>();

        oriColor = new Color(0f, 2f, 2f);

        material.color = oriColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerScript.changedColor)
        {
            playerScript.changedColor = false;
            isChanged = false;
            material.color = oriColor;
        }
        if (gameObject.activeInHierarchy && playerScript.DestroyedShield)
        {
            if (!isChanged)
            {
                isChanged = true;
                material.color = Color.red;
            }
        }
    }
}
