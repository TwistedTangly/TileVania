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
    LayerMask waterLayer;
    LayerMask portalLayer;
    float beginingGravityScale;
    float beginingAnimationSpeed;
    bool isAlive = true;
    float currentScale = 1;
    float startWhiteOpacity = 0;
    float currentRotation = 0f;
    [SerializeField] float timeBetweenShrinking = 0.2f;
    [SerializeField] float timeBetweenOpacityChange = 0.1f;
    [SerializeField] float shrinkAmount = 0.1f;
    [SerializeField] float opacityIncrease = 0.1f;
    [SerializeField] float baseGravity = 6f;
    [SerializeField] float WaterGravity = .1f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float swimJumpSpeed = 1f;
    [SerializeField] float flashTime = .3f;
    [SerializeField] GameObject arrow;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject whiteBackground;
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
        waterLayer = LayerMask.GetMask("Water");
        portalLayer = LayerMask.GetMask("Portal");

        beginingGravityScale = myRigidbody2D.gravityScale; 
        beginingAnimationSpeed = myAnimator.speed;

    }

    void Update()
    {
        if(!isAlive){return;}

        if(!feetCollider2d.IsTouchingLayers(waterLayer) || !feetCollider2d.IsTouchingLayers(climbableLayer))
         {
             myRigidbody2D.gravityScale = baseGravity;
             runSpeed = 7f;
         }

        Run();
        FlipSprite();
        ClimbLadder();  
        Die(); 
        Swim(); 
        LevelTransistion();            
    }

    void LevelTransistion()
    {
        if(myBodyCollider2D.IsTouchingLayers(portalLayer))
        {
            StartCoroutine(TrasitionEffects());
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator TrasitionEffects()
    {
        isAlive = false;
        myAnimator.SetTrigger("Float");
        myRigidbody2D.velocity = new Vector2(0.3f,0);
        myRigidbody2D.gravityScale = -0.03f;
        while(currentScale > 0)
        {
            yield return new WaitForSecondsRealtime(timeBetweenShrinking);
            currentScale -= shrinkAmount;
            
            transform.localScale = new Vector3(currentScale,currentScale,0);
            mySpriteRenderer.color = new Color(1,1,1,currentScale);
                                   
        }
    }

    IEnumerator FadeOut()
    {
        while(startWhiteOpacity < 1)
        {
            yield return new WaitForSecondsRealtime(timeBetweenOpacityChange);
            currentRotation += 30;
            transform.localRotation = Quaternion.Euler(0,0,currentRotation);
            startWhiteOpacity += opacityIncrease;
            whiteBackground.GetComponent<SpriteRenderer>().color = new Color(1,1,1,startWhiteOpacity); 
        }

    }

    void Swim()
    {
         if(feetCollider2d.IsTouchingLayers(waterLayer)) 
         { 
            myRigidbody2D.gravityScale = WaterGravity;
            runSpeed = 2f;
         }    
         
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
        if(!feetCollider2d.IsTouchingLayers(groundLayer) && !feetCollider2d.IsTouchingLayers(waterLayer)) { return;}
        
        if(value.isPressed)
        {
            if(feetCollider2d.IsTouchingLayers(waterLayer))
            {
                myRigidbody2D.velocity += new Vector2(0f, swimJumpSpeed);
            }
            else
            {
                myRigidbody2D.velocity += new Vector2(0f, jumpSpeed);
            }
            
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

    void OnFire(InputValue value)
    {
        if(!isAlive){return;}  

        if(transform.localScale.x == -1)
        {
            arrow.transform.localScale = new Vector3(-1,1,1);
        }
        else
        {
            arrow.transform.localScale = new Vector3(1,1,1);
        }

        Instantiate(arrow, spawnPoint.position, spawnPoint.rotation);
        StartCoroutine(FireAnim());
    }

    IEnumerator FireAnim()
    {
        myAnimator.SetBool("isFiring", true);
        yield return new WaitForSeconds(.3f);
        myAnimator.SetBool("isFiring", false);
    }

}
