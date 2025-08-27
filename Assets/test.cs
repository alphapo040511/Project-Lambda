using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public KeyCode openKey = KeyCode.Space;
    public KeyCode closeKey = KeyCode.G;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            animator.SetInteger("State", 1);
        }
        if (Input.GetKeyDown(closeKey))
        {
            animator.SetInteger("State", 2);
        }
    }
}
