using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Stickman_MovementController))]
public class stickman : MonoBehaviour
{
    //public Rigidbody2D rb2d;
    //public float playerSpeed;
    //public float jumpStrength;
    //public float wallJumpStrength;
    public bool dead = false;
    public bool ableToJump = true;
    public GameObject equip;
    public float initialRespawnTimer;
    public float respawnTimer;
    public bool controlsDisabled = false;
    //public bool wallJumping = false;
    public GameObject shaker;
    public int playerNum;
    public stickman otherPlayer;
    public bool firing = false;
    public float throwSpeed;
    public bool hit = false;
    public int initialHealth = 100;
    public Canvas playerCanvas;
    public Slider healthBar;
    public GameObject minimapIcon;
    public GameObject trail;
    public GameObject glow;

    private bool lastControls;
    //private int wallJumpNum = 0;
    private Collider2D item;
    private string hAxis;
    private bool footstool = false;
    private GameObject other;
    private bool singleFire = false;
    private bool ended = false;
    private string jAxis;
    private string eAxis;
    private string fAxis;
    private string vAxis;
    private string tAxis;
    private float health;
    private bool damaged = false;
    private GameObject particleParent;
    private GameObject weaponFired;
    private GameObject environmentalDamage;
    private float gameEndDelay = 3f;
    private Animator anim;

    /*Sarah_Dev: Player Movement*/
    Stickman_MovementController movementController;
    Vector3 velocity;

    public float playerSpeed = 20;

    public float jumpHeight = 9; //how many units is a jump?
    public float timeToJumpApex = .4f; //how long until the top of a jump?
    float gravity;
    float jumpVelocity;

    float velocityXSmoothing;
    public float accelerationTimeAirborne = .2f; //time to change direction in air
    public float accelerationTimeGrounded = .1f; //time to change direction on ground

    public float wallSlideSpeedMax = 3; //Give as a postive!! (speed means no neg)
    public Vector2 wallJumpClimb = new Vector2(7.5f, 40);
    public Vector2 wallJumpOff = new Vector2(8.5f, 30);
    public Vector2 wallLeap = new Vector2(24, 36);
    public float wallStickTime = .25f; //Makes wallLeap's easier to perform
    float timeToWallUnstick;
    bool canWallJumpClimb;

    bool touchingGround;
    /*Sarah_Dev*/

    private bool DEBUG = true;

    private void Awake()
    {
        if (DEBUG)
        {
            Debug.Log("There are no controllers detected, switching to keyboard mode.");
            if (playerNum == 1)
            {
                hAxis = "Horizontal";
                vAxis = "Vertical";
                jAxis = "Jump";
                fAxis = "Fire1";
                eAxis = "Fire2";
                tAxis = "Fire3";
            }
            return;
        }
        if (playerNum == 1)
        {
            hAxis = "Horizontal_P1";
            jAxis = "Jump_P1";
            eAxis = "Equip_P1";
            fAxis = "Fire_P1";
            vAxis = "Vertical_P1";
            tAxis = "Throw_P1";
        }
        else
        {
            hAxis = "Horizontal_P2";
            jAxis = "Jump_P2";
            eAxis = "Equip_P2";
            fAxis = "Fire_P2";
            vAxis = "Vertical_P2";
            tAxis = "Throw_P2";
        }
    }

    void Start()
    {
        //rb2d = GetComponent<Rigidbody2D>();
        respawnTimer = initialRespawnTimer;
        health = initialHealth;
        anim = gameObject.GetComponent<Animator>();

        movementController = GetComponent<Stickman_MovementController>();
        movePlayer_start();
    }

    private void Update()
    {
        //checks if controls are enabled or not
        resetControls();
        //most movement logic for the player
        //movePlayer();
        movePlayer_update();
        //checks if the player is attempting to pick up/drop weapon
        checkPickup();
        //checks if the player is throwing a weapon
        checkThrow();
        //checks if the player is attempting to jump off of the wall
        //wallJump();
        //checks if the player is attempting to use their weapon
        fireWeapon();
        //checks to see if player is taking damage
        checkDamage();
        //checks to see if enabling the controls has changed
        lastControls = GameControl.instance.controlsDisabled;
        //if the game is over, run game over logic
        if (GameControl.instance.fight)
        {
            gameEnd();
        }
    }

    private void LateUpdate()
    {
        /*if(rb2d.velocity.x > GameControl.instance.maxPlayerSpeed)
        {
            rb2d.velocity = new Vector2(GameControl.instance.maxPlayerSpeed, rb2d.velocity.y);
        }
        if (rb2d.velocity.y > GameControl.instance.maxPlayerSpeed)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, GameControl.instance.maxPlayerSpeed);
        }*/
        playerCanvas.transform.localScale = transform.localScale;
        float h = Input.GetAxis(hAxis);
        anim.SetBool("Running", Mathf.Abs(h) > 0 && ableToJump);
        respawn();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && velocity.y == 0)
        {
            ableToJump = true;
            touchingGround = true;
            //wallJumping = false;
            //wallJumpNum = 0;
            shaker.SetActive(true);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 dist = getDistanceToWall();
            if(dist.x >= 1 && dist.y >= 1)
            {
                ableToJump = true;
                //wallJumping = false;
                //wallJumpNum = 0;
                shaker.SetActive(true);
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            raycastToPlayer();
            if (footstool)
            {
                other = collision.gameObject;
                ableToJump = true;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ableToJump = true;
            touchingGround = true;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 dist = getDistanceToWall();
            if (dist.x >= 1 && dist.y >= 1)
            {
                ableToJump = true;
                //wallJumping = false;
                //wallJumpNum = 0;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ableToJump = false;
            touchingGround = false;
        }
        if (collision.gameObject.CompareTag("Wall") && velocity.y == 0)
        {
            Vector2 dist = getDistanceToWall();
            if (dist.x >= 1 && dist.y >= 1)
            {
                ableToJump = false;
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            raycastToPlayer();
            if (footstool)
            {
                other = collision.gameObject;
                footstool = false;
            }
            else
            {
                ableToJump = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            item = collision;
            if(collision.GetComponent<Rigidbody2D>().velocity.magnitude > 20)
            {
                //weapon was thrown/dropped from high up
                hit = true;
            }
        }
        if (collision.gameObject.CompareTag("Killbox"))
        {
            die();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Weapon"))
        {
            item = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            item = null;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        damaged = true;
        if (other.gameObject.name == "Avalanche")
        {
            environmentalDamage = other.gameObject;
            particleParent = environmentalDamage;
            weaponFired = null;
        }
        else
        {
            try
            {
                particleParent = other.transform.parent.transform.parent.transform.parent.transform.parent.gameObject;
                weaponFired = other.transform.parent.transform.parent.gameObject;
            }
            catch(NullReferenceException e)
            {

            }
        }
    }

    /*
    public void movePlayer()
    {
        if (!dead && !controlsDisabled && !GameControl.instance.paused)
        {
            bool v = Input.GetButtonDown(jAxis);
            float h = Input.GetAxis(hAxis);

            if (rb2d.velocity.y <= 0 && wallJumping)
            {
                wallJumping = false;
            }
            if (!wallJumping)
            {
                if (v && ableToJump)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, jumpStrength);
                    ableToJump = false;
                    if (footstool)
                    {
                        if (otherPlayer.ableToJump)
                        {
                            Rigidbody2D r = otherPlayer.GetComponent<Rigidbody2D>();
                            r.velocity = new Vector2(r.velocity.x, -jumpStrength);
                        }
                        footstool = false;
                    }
                }
                rb2d.velocity = new Vector2(h * playerSpeed, rb2d.velocity.y);
                if (h != 0)
                {
                    transform.localScale = new Vector3(Mathf.Sign(h), transform.localScale.y, transform.localScale.z);
                }
            }
        }
    }*/

    public void respawn()
    {
        Text t = GameControl.instance.p2Text;
        if (!GameControl.instance.ableToDie)
        {
            if (dead && !GameControl.instance.paused)
            {
                string color = getColor();
                if (playerNum == 1)
                {
                    t = GameControl.instance.p1Text;
                }
                t.text = color + " respawning in " + string.Format("{0:N0}", Mathf.Ceil(respawnTimer));
                respawnTimer -= Time.deltaTime;
                velocity = Vector3.zero;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<Collider2D>().isTrigger = true;
                healthBar.gameObject.SetActive(false);
                Transform dropped = dropObject();
                minimapIcon.SetActive(false);
                glow.SetActive(false);
                if (dropped != null)
                {
                    dropped.GetComponent<Rigidbody2D>().velocity = new Vector2(UnityEngine.Random.Range(-10f, 10f), 50);
                }
            }
            if (respawnTimer <= 0)
            {
                int ind = GameControl.instance.getRespawnPlat();
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
                t.text = "";
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                gameObject.GetComponent<Collider2D>().isTrigger = false;
                equip.SetActive(true);
                trail.SetActive(false);
                transform.position = new Vector3(platforms[ind].transform.position.x, platforms[ind].transform.position.y + 3, transform.position.z);
                trail.SetActive(true);
                velocity = Vector3.zero;
                respawnTimer = initialRespawnTimer;
                dead = false;
                controlsDisabled = false;
                firing = false;
                healthBar.gameObject.SetActive(true);
                health = initialHealth;
                healthBar.value = health;
                minimapIcon.SetActive(true);
                glow.SetActive(true);
            }
        }
        else
        {
            GameControl.instance.fight = true;
        }
    }

    private void checkPickup()
    {
        if (controlsDisabled)
        {
            return;
        }
        if (Input.GetButtonDown(eAxis) || hit)
        {
            Transform dropped = null;
            if(equip.transform.childCount == 1)
            {
                dropped = dropObject();
                //throw
                float h = Input.GetAxis(hAxis);
                float v = Input.GetAxis(vAxis);
                dropped.GetComponent<Rigidbody2D>().velocity = new Vector2(-5 * transform.localScale.x, 20);
            }
            if (item != null && item.transform != dropped)
            {
                //pickup
                item.gameObject.transform.parent = equip.transform;
                item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                item.GetComponent<Rigidbody2D>().angularVelocity = 0;
                item.gameObject.transform.eulerAngles = equip.transform.eulerAngles;
                item.gameObject.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                item.gameObject.transform.position = equip.transform.position;
                item.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                if (item.name.Contains("Flashlight"))
                {
                    singleFire = true;
                }
            }
            hit = false;
        }
    }

    private Transform dropObject()
    {
        //drop
        if (equip.transform.childCount == 0) {
            return null;
        }
        Transform dropped = equip.transform.GetChild(0);
        dropped.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        dropped.gameObject.transform.eulerAngles = Vector3.zero;
        float s = -1;
        if (transform.localScale.x == 1)
        {
            s = 1;
        }
        dropped.gameObject.transform.localScale = new Vector3(s, dropped.gameObject.transform.localScale.y, dropped.gameObject.transform.localScale.z);
        dropped.parent = GameControl.instance.pickups.transform;
        dropped.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        if (dropped.name.Contains("Flashlight"))
        {
            dropped.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
            dropped.GetComponent<Rigidbody2D>().gravityScale = 5;
        }
        else
        {
            dropped.transform.GetChild(0).GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();
        }
        return dropped;
    }

    private void checkThrow()
    {
        if (controlsDisabled)
        {
            return;
        }
        if (Input.GetButtonDown(tAxis))
        {
            Transform dropped = null;
            if (equip.transform.childCount == 1)
            {
                dropped = dropObject();
                //throw
                dropped.GetComponent<Rigidbody2D>().velocity = new Vector2(throwSpeed * transform.localScale.x, 10);
            }
        }
    }

    public float sign(float value)
    {
        if(value == 0)
        {
            return 0;
        }
        return Mathf.Sign(value);
    }

    /*public float checkWallJump(Rigidbody2D rb2d)
    {
        float h = Input.GetAxis(hAxis);
        int ignore = LayerMask.GetMask("Wall");
        RaycastHit2D left = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.left), Mathf.Infinity, ignore);
        RaycastHit2D right = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.right), Mathf.Infinity, ignore);
        if (left.collider != null && right.collider != null)
        {
            if (left.distance < right.distance)
            {
                return -1;
            }
        }
        return 1;
    }
    */

    public Vector2 getDistanceToWall()
    {
        Vector2 result = new Vector2(Mathf.Infinity, Mathf.Infinity);
        float h = Input.GetAxis(hAxis);
        int ignore = LayerMask.GetMask("Wall");
        RaycastHit2D left = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.left), Mathf.Infinity, ignore);
        RaycastHit2D right = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.right), Mathf.Infinity, ignore);
        if (left.collider != null)
        {
            result.x = left.distance;
        }
        if(right.collider != null)
        {
            result.y = right.distance;
        }
        return result;
    }

    public void raycastToPlayer()
    {
        Vector2 result = new Vector2(Mathf.Infinity, Mathf.Infinity);
        int ignore = LayerMask.GetMask("Player");
        RaycastHit2D down = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), Mathf.Infinity, ignore);
        if(down.collider != null)
        {
            footstool = true;
        }
        else
        {
            footstool = false;
        }
    }

    private void resetControls()
    {
        if (GameControl.instance.controlsDisabled)
        {
            controlsDisabled = true;
        }
        else if (lastControls)
        {
            controlsDisabled = false;
        }
    }

    /*public void wallJump()
    {
        Vector2 walls = getDistanceToWall();
        if (Input.GetButtonDown(jAxis) && !ableToJump)
        {
            float h = Input.GetAxis(hAxis);
            float scale = 2;
            if (walls.x < 1)
            {
                wallJumpNum++;
                rb2d.velocity = new Vector2(wallJumpStrength * scale, jumpStrength / wallJumpNum);
                wallJumping = true;
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            if (walls.y < 1)
            {
                wallJumpNum++;
                rb2d.velocity = new Vector2(-wallJumpStrength * scale, jumpStrength / wallJumpNum);
                wallJumping = true;
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        }
    }*/

    private void gameEnd()
    {
        if (dead)
        {
            GameControl.instance.gameOver = true;
            //otherPlayer.rb2d.bodyType = RigidbodyType2D.Kinematic;
            //otherPlayer.rb2d.velocity = Vector2.zero;
            velocity = Vector3.zero;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<Collider2D>().isTrigger = true;
            healthBar.gameObject.SetActive(false);
            Transform dropped = dropObject();
            GameControl.instance.topText.gameObject.SetActive(false);
            if (dropped != null)
            {
                dropped.GetComponent<Rigidbody2D>().velocity = new Vector2(UnityEngine.Random.Range(-10f, 10f), 50);
            }
            if (gameEndDelay > 0)
            {
                GameControl.instance.statusText.text = "Game!";
                gameEndDelay -= Time.deltaTime;
            }
            else
            {
                string color = getWinner();
                GameControl.instance.statusText.text = color + " wins!";
                GameControl.instance.controlsDisabled = true;
                GameControl.instance.quitButton.gameObject.SetActive(true);
                GameControl.instance.restartButton.gameObject.SetActive(true);
                if (!ended)
                {
                    GameControl.instance.restartButton.Select();
                    ended = true;
                }
            }
        }
    }

    private string getWinner()
    {
        string color = "Red";
        if(playerNum == 1)
        {
            color = "Blue";
        }
        return color;
    }

    private string getColor()
    {
        string color = "Blue";
        if (playerNum == 1)
        {
            color = "Red";
        }
        return color;
    }

    private void fireWeapon()
    {
        if (dead || controlsDisabled)
        {
            return;
        }
        if (Input.GetButtonDown(fAxis))
        {
            firing = true;
            if (equip.transform.GetChild(0).name.Contains("Flashlight"))
            {
                singleFire = true;
            }
        }
        if (Input.GetButtonUp(fAxis))
        {
            firing = false;
        }
        if (singleFire)
        {
            fireOnce();
        }
        else
        {
            fireMulti();
        }
    }

    private void fireOnce()
    {
        if(equip.transform.childCount != 1)
        {
            return;
        }
        if (firing && singleFire)
        {
            GameObject projectile = equip.transform.GetChild(0).gameObject;
            if (equip.transform.GetChild(0).name.Contains("Flashlight"))
            {
                projectile = equip.transform.GetChild(0).transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
                projectile.SetActive(!projectile.activeSelf);
            }
            singleFire = false;
        }
    }
    private void fireMulti()
    {
        if (equip.transform.childCount == 0)
        {
            return;
        }
        if (equip.transform.GetChild(0).name.Contains("Flashlight"))
        {
            return;
        }
        GameObject projectile = equip.transform.GetChild(0).gameObject; 
        projectile = equip.transform.GetChild(0).transform.GetChild(0).GetChild(1).gameObject;
        ParticleSystem ps = projectile.GetComponent<ParticleSystem>();
        if (firing)
        {
            if (!ps.isEmitting)
            {
                ps.Play();
            }
        }
        else
        {
            if (ps.isEmitting)
            {
                ps.Stop();
            }
        }
    }

    private void checkDamage()
    {
        if(particleParent == null || particleParent.name == gameObject.name)
        {
            return;
        }
        if (particleParent != environmentalDamage)
        {
            if (damaged)
            {
                health -= GameControl.instance.flamethrowerDamage * Time.deltaTime;
                healthBar.value = health;
                damaged = false;
                if (health <= 0)
                {
                    dead = true;
                }
            }
        }
        else
        {
            if (damaged)
            {
                health -= GameControl.instance.avalancheDamage * Time.deltaTime;
                healthBar.value = health;
                damaged = false;
                if (health <= 0)
                {
                    dead = true;
                }
            }
        }
    }

    private void die()
    {
        dead = true;
        controlsDisabled = true;
        velocity = Vector3.zero;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    /*Sarah_Dev: Wall Collision + Sliding + Jumping*/
    public void movePlayer_start()
    {
        //~KINEMATICS TIME~
        // /\d = vi * t + (a*t^2)/2
        //-> (-1 * jumpHeight) = 0 + (gravity * timeToJumpApex^2)/2
        gravity = -(2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        //vf = vi + a*t -> jumpSpeed = 0 + (-1 * gravity) * timeToJumpApex
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        canWallJumpClimb = false;
    }

    public void movePlayer_update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        int wallDirX = (movementController.collisions.left) ? -1 : 1; //-1 = left, 1 = right

        bool wallSliding = false;

        //NOTE - wallsliding will now set velocity.x = 0 to allow for wallStickTime
        //SO - YOU MUST SET VELOCITY X BEFORE WALLSLIDING CALCULATIONS
        movePlayer_updateVelocityX(input);

        //wallsliding is true IF: colliding with a wall to our left or right, 
        //not grounded, and moving downwards
        /*if ((movementController.collisions.left || movementController.collisions.right)
            && (!movementController.collisions.below) && (velocity.y < 0))
        {
            wallSliding = true;
            movePlayer_updateWallSliding(input, wallDirX);
        }*/

        if ((movementController.collisions.left || movementController.collisions.right) 
            && !movementController.collisions.below && velocity.y < 0 && !touchingGround)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }

        }


        //prevent gravity from accumulating if we're not falling
        if (movementController.collisions.above || movementController.collisions.below)
        {
            velocity.y = 0;
        }

        if ((!canWallJumpClimb) && movementController.collisions.below)
        {
            canWallJumpClimb = true;
        }

        if (Input.GetButtonDown(jAxis))
        {
            print("attempting to jump");

            if (wallSliding) //can wall jump
            {
                print("Walljumping!");
                movePlayer_updateWallJump(input, wallDirX);
            }

            if (movementController.collisions.below) //can regular jump
            {
                velocity.y = jumpVelocity;
            }
        }

        //multiply by Time.deltaTime to account for uneven updating
        velocity.y += gravity * Time.deltaTime; //apply gravity
        movementController.Move(velocity * Time.deltaTime); //move the player
    }

    void movePlayer_updateVelocityX(Vector2 input)
    {
        //our final x velocity
        float targetVelocityX = input.x * playerSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            (movementController.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

    }

    void movePlayer_updateWallSliding(Vector2 input, int wallDirX)
    {
        if (velocity.y < -wallSlideSpeedMax)
        {
            velocity.y = -wallSlideSpeedMax;
        }

        if (timeToWallUnstick > 0)
        {
            velocity.x = 0;
            velocityXSmoothing = 0;

            if (input.x != wallDirX && input.x != 0)
            {
                timeToWallUnstick -= Time.deltaTime;
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
        else
        {
            timeToWallUnstick = wallStickTime;
        }
    }

    void movePlayer_updateWallJump(Vector2 input, int wallDirX)
    {

        if (wallDirX == input.x) //walljump climb
        {
            if (canWallJumpClimb)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;

                canWallJumpClimb = false;
            }
        }
        else if (input.x == 0) //walljump
        {
            velocity.x = -wallDirX * wallJumpOff.x;
            velocity.y = wallJumpOff.y;
        }
        else //wall leap
        {
            velocity.x = -wallDirX * wallLeap.x;
            velocity.y = wallLeap.y;
        }

    }
    /*Sarah_Dev*/

}
