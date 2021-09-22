using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public bool isSwinging = false; //is the player currently swinging?

    float horizontalMove = 0f;
    [Range(0, .3f)]
    private float movementSmoothing = .05f; //How much to smooth out movement
    private Vector3 charVelocity = Vector3.zero;
    private bool grounded;
    private bool faceRight = true; //is the player facing right
    private GameObject blockPanel; //represents the block selection panel
    private bool isMenuUp = true; //The state of the menu.
    private Vector3 teleportLocation; //If the player is being tp'd this is where they will go.
    private string newLevel; //should we change level when we tp?
    

    private const float groundedRadius = .2f; //Radisu of the overlap circle to determine if grounded

    private SpriteRenderer playerSprite;
    private Animator playerAnimator;
    private LayerMask whatIsGround; //Layermask determining what is ground to the player
    private Transform groundCheck; // a position marking where to check if the player is grounded
    private float runSpeed = 3f;
    private float jumpForce = 13f;
    private GameObject player;
    private Rigidbody2D playerRB;
    private GameObject systemMenu; //the panel we want to enable / disable for a menu
    private bool isSysMenuUp; //is the system menu up


    private void Awake()
    {

        player = this.gameObject;
        playerRB = this.gameObject.GetComponent<Rigidbody2D>();
        playerSprite = this.gameObject.GetComponent<SpriteRenderer>();
        playerAnimator = this.gameObject.GetComponent<Animator>();
        whatIsGround = LayerMask.GetMask("Ground", "Interactive");
        groundCheck = GameObject.Find("GroundCheck").transform;
        

    }

    private void Start()
    {

        blockPanel = GameObject.Find("Block Selection Panel");
        isMenuUp = false;
        blockPanel.SetActive(false);
        systemMenu = GameObject.Find("SystemMenuPanel");
        isSysMenuUp = false;
        systemMenu.SetActive(false);

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }


        DontDestroyOnLoad(player);

    }


    private void Update()
    {

        if (!isSysMenuUp) //don't move if the system menu is up.
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            playerAnimator.SetFloat("SpeedX", Mathf.Abs(horizontalMove));
        }
        
        //Debug.Log(horizontalMove);

        if (horizontalMove >= 0.01f && faceRight == false)
        {
            playerFlip(); //flip the sprite
        } else if (horizontalMove <= -0.01f && faceRight == true)
        {
            playerFlip(); //flip the sprite
        }

        if (Input.GetButtonDown("Menu") && isSysMenuUp == false)
        {
            if (isMenuUp)
            {
                blockPanel.SetActive(false);
                isMenuUp = false;
            }
            else
            {
                blockPanel.SetActive(true);
                isMenuUp = true;
            }
        }

        if(Input.GetButtonDown("SystemMenu"))
        {
            if (isSysMenuUp)
            {
                systemMenu.SetActive(false);
                isSysMenuUp = false;

            } else
            {
                systemMenu.SetActive(true);
                isSysMenuUp = true;
            }
        }

    }

   
    void FixedUpdate()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject && playerRB.velocity.y == 0f)
            {
                grounded = true;
                
            }
        }

        if (Input.GetButton("Jump"))
        {
            //Debug.Log("I pressed the jump button!");
            if(grounded)
            {
                Debug.Log("Jumping!");
                playerRB.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                grounded = false;
            }
        }

        Vector3 targetVelocity = new Vector2(horizontalMove, playerRB.velocity.y);
        playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, targetVelocity, ref charVelocity, movementSmoothing);

        playerAnimator.SetFloat("SpeedY", playerRB.velocity.y);

        
        

        //Debug.Log(playerRB.velocity);

        

    }

    void playerFlip()
    {

        if (faceRight) //if we're facing right we need to face left. The player starts facing right.
        {
            playerSprite.flipX = true;
            faceRight = false;
        } else //if we're facing left we need to face right.
        {
            playerSprite.flipX = false;
            faceRight = true;
        }

    }

    /// <summary>
    /// Begin the process of teleporting the player
    /// </summary>
    /// <param name="tpLocation"> Vector3 coords to physically move player to. </param>
    /// <param name="moveLevel"> String representing which level if any we should change to. Empty string is reserved for no level change. </param>
    public void StartTP(Vector3 tpLocation, string newLevelName)
    {
        
        if(tpLocation != null)
        {

            newLevel = newLevelName;
            teleportLocation = tpLocation;
            playerRB.constraints = RigidbodyConstraints2D.FreezePosition; //Freeze the player untill tp animation / action is done
            playerAnimator.SetBool("UsingDoor", true); // once the animation finishes it will call teleport
            

        } else
        {
            Debug.LogError("PlayerController/StartTP: Error determining location. Is location set?");
        }

    }

    public void Teleport()
    {

        if(newLevel != "") //empty string is reserved for no level change.
        {
            SceneManager.LoadScene(newLevel);
            newLevel = null; //the var must be reset because we're not destorying this object.
            player.transform.position = teleportLocation; //teleport the player to the given location.
            playerAnimator.SetBool("ExitingDoor", true);

        } else
        {
            player.transform.position = teleportLocation; //teleport the player to the given location.
            playerAnimator.SetBool("ExitingDoor", true);

        }

    }

    public void EndTP()
    {

        playerAnimator.SetBool("UsingDoor", false);
        playerAnimator.SetBool("ExitingDoor", false);
        playerRB.constraints = RigidbodyConstraints2D.None; //unfreeze the player
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation; //freeze the rotation again.

    }

    //The code for the resume button
    public void Resume()
    {

        systemMenu.SetActive(false);
        isSysMenuUp = false;

    }


}
