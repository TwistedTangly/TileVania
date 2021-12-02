using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody2D ;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider2D;
    BoxCollider2D feetCollider2d;
    SpriteRenderer mySpriteRenderer;
    LayerMask groundLayer;
    LayerMask climbableLayer;
    LayerMask DamageLayer;
    float beginingGravityScale;
    float beginingAnimationSpeed;
    bool isAlive = true;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float flashTime = .3f;
    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        feetCollider2d = GetComponent<BoxCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        groundLayer = LayerMask.GetMask("Ground");
        climbableLayer = LayerMask.GetMask("Climbing");
        DamageLayer = LayerMask.GetMask("Enemies", "Hazards");

        beginingGravityScale = myRigidbody2D.gravityScale; 
        beginingAnimationSpeed = myAnimator.speed;
    }

    void Update()
    {
        if(!isAlive){return;}

        Run();
        FlipSprite();
        ClimbLadder();  
        Die();      
    }

    void OnMove(InputValue value)
    {
        if(!isAlive){return;}
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody2D.velocity.y);
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
        if(!isAlive){return;}
        if(!feetCollider2d.IsTouchingLayers(groundLayer)) { return;}
        
        if(value.isPressed)
        {
            myRigidbody2D.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void ClimbLadder()
    {
        if(!feetCollider2d.IsTouchingLayers(climbableLayer)) 
        {
            myAnimator.SetBool("isClimbing", false);
            myRigidbody2D.gravityScale = beginingGravityScale;
            myAnimator.speed = beginingAnimationSpeed;
            return;
         }

        Vector2 climbVelocity = new Vector2(myRigidbody2D.velocity.x, moveInput.y * climbSpeed);
        myRigidbody2D.velocity = climbVelocity ;
        myRigidbody2D.gravityScale = 0;

        myAnimator.SetBool("isClimbing", true);
        myAnimator.speed = 0;
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody2D.velocity.y)> Mathf.Epsilon;
        if(playerHasVerticalSpeed)
        {
            myAnimator.speed = beginingAnimationSpeed;
        }   
    }

    private void Die() 
    {
        if(myBodyCollider2D.IsTouchingLayers(DamageLayer))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody2D.velocity += new Vector2(0f, jumpSpeed);
            StartCoroutine(ColourFlash());            
        }
    }

    IEnumerator ColourFlash()
    {
        mySpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        mySpriteRenderer.color = Color.white;
    }


}
