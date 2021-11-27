using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody2D ;
    Animator myAnimator;
    Collider2D myCollider2D;
    LayerMask groundLayer;
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider2D = GetComponent<Collider2D>();
        groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        Run();
        FlipSprite();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * speed, myRigidbody2D.velocity.y);
        myRigidbody2D.velocity = playerVelocity ;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody2D.velocity.x)> Mathf.Epsilon;

            myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody2D.velocity.x)> Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
        transform.localScale = new Vector2 (Mathf.Sign(myRigidbody2D.velocity.x), 1f);
        }
    }

    void OnJump(InputValue value)
    {
        if(!myCollider2D.IsTouchingLayers(groundLayer)) {return;}
        
        if(value.isPressed)
        {
            myRigidbody2D.velocity += new Vector2(0f, jumpSpeed);
        }
    }
}
