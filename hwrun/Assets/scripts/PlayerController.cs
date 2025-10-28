using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public bool jump = false;

    public bool slide = false;

    public Animator playerAnim;

    [SerializeField] Transform Foot;

    [SerializeField] LayerMask ground;

    void Start()
    {
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       transform.Translate(0, 0, 2f * Time.deltaTime);

       // transform.Translate(0, 0, 0.05f);


        bool IsGrounded()
           {
               return Physics.CheckSphere(Foot.position, .1f, ground);
           }
 

        if (Input.GetKey (KeyCode.Space) && IsGrounded())
        {
            jump = true;
        }
        else
        {
            jump = false;
        }


        if (Input.GetKey(KeyCode.DownArrow))
        {
            slide = true;
        }
        else
        {
            slide = false;
        }


        if (jump == true)
        {
            playerAnim.SetBool("IsJump", jump);

            transform.Translate(0, 0.1f, 0.05f);
        }

        else if(jump == false)
        {
            playerAnim.SetBool("IsJump", jump);
        }

        if (slide == true)
        {
            playerAnim.SetBool("IsSlide", slide);

            transform.Translate(0, 0, 0.05f);
        }

        else if (jump == false)
        {
            playerAnim.SetBool("IsSlide", slide);
        }

    }


}
