using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_Attack : MonoBehaviour
{
    Animator animator;
    bool OnAni = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (!OnAni)
            {
                OnAni = true;
                animator.SetBool("isGas", true);
            }
        }
    }
    
    public void EndOfGas()
    {
        OnAni = false;
        animator.SetBool("isGas", false);
        gameObject.SetActive(false);
    }
}
