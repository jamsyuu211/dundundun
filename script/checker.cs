using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checker : MonoBehaviour
{
    BoxCollider2D myCollider;
    public GameObject dog;
    Dog dogScript;
    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        dogScript = dog.GetComponent<Dog>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EndOfBlock")
        {
            dogScript.freezeRb();
            dogScript.endOfBlock = true;
        }
    }
}
