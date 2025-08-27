using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ttttt : MonoBehaviour
{
    
        public KeyCode openKey = KeyCode.Space;
        public KeyCode closeKey = KeyCode.C;
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
            animator.SetInteger("stats", 1);
            }
            if (Input.GetKeyDown(closeKey))
            {
                animator.SetInteger("stats", 2);
            }
        }
}
