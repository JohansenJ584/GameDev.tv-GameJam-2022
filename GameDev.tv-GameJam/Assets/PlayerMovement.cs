using UnityEngine;
 
public class PlayerMovement : MonoBehaviour
{ 
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float JumpCooldown;
    private float WallCooldown;
    private float gripTime;
    private float timeToDoubleJump;
    private float horizontalInput;
    private float gravityScale;
    //private Animator anim;
    [SerializeField] private bool canDoubleJump = false;
    [SerializeField] private bool canWallJump = false;

    
    private bool didDoubleJump = false;
    
    private void Awake()
    {   
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        gravityScale = body.gravityScale;
        //anim = GetComponent<Animator>();
    }
 
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        
        //Directional for animation and for sign mathf
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),  transform.localScale.y,  transform.localScale.z);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1,  transform.localScale.y,  transform.localScale.z);
        }

            if(canWallJump && onWall() && !isGrounded() && gripTime < 1.00f && WallCooldown < 0)
            {
                body.velocity = Vector2.zero;
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
            {
                if(isGrounded())
                {
                    gripTime = 0;
                    timeToDoubleJump = 0;
                }
                body.gravityScale = gravityScale;
            }

        if(JumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        JumpCooldown += Time.deltaTime;
        gripTime += Time.deltaTime;
        WallCooldown -= Time.deltaTime;
        timeToDoubleJump += Time.deltaTime;
        /*
        else
        {
            JumpCooldown += Time.deltaTime;
            WallCooldown -= Time.deltaTime;
        }
        //sets animation parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);
        */
    }
 
    private void Jump()
    {
        if(isGrounded())
        {
            didDoubleJump = false;
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            //anim.SetTrigger("jump");
        }
        else if(canWallJump && onWall() && !isGrounded())
        {
            /*
            if(horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 1, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x) * transform.localScale.x, transform.localScale.y,transform.localScale.z);
            }
            else
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6) / 2;
            }
            */
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, jumpPower);
            WallCooldown = 3.0f;
            JumpCooldown = 0;
        }
        else if(canDoubleJump && !didDoubleJump && timeToDoubleJump > .25f && !onWall() && !isGrounded())
        {
            didDoubleJump = true;
            body.velocity = new Vector2(body.velocity.x, jumpPower * .8f);
        }
    }
 
    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    } 

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null && WallCooldown < 0;
    } 

}
