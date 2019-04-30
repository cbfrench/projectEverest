using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /* Author: Connor French
     * Description: class for controlling each of the players within the scene
     * Contributors: Reynaldo Hermawan, Caleb Biggers
     */

    public Rigidbody2D rb2d;
    public float playerSpeed;
    public float jumpStrength;
    public float wallJumpStrength;
    public bool dead = false;
    public bool ableToJump = true;
    public GameObject equip;
    public float initialRespawnTimer;
    public float respawnTimer;
    public bool controlsDisabled = false;
    public bool wallJumping = false;
    public GameObject shaker;
    public int playerNum;
    private PlayerController otherPlayer;
    public float throwSpeed;
    public bool hit = false;
    public int initialHealth = 100;
    public Canvas playerCanvas;
    public Slider healthBar;
    public GameObject minimapIcon;
    public GameObject trail;
    public GameObject glow;
    public AudioSource thud;
    public ParticleSystem wallSlide;
    public float wallSlideSpeed;
    public int lives = 5;
    public GameObject head;
    public GameObject playerText;

    private bool lastControls;
    private Collider2D item;
    private string hAxis;
    private bool footstool = false;
    private GameObject other;
    private bool singleFire = false;
    private string jAxis;
    private string eAxis;
    private string fAxis;
    private string vAxis;
    private string tAxis;
    private float health;
    private bool damaged = false;
    private float gameEndDelay = 3f;
    private Animator anim;
    private bool sliding = false;
    private int wallJumpNum = 0;
    private float wallDistance = 0.55f;
    private float wallDistanceBelow = 1.015f;
    private bool leftWallJump = false;
    private bool rightWallJump = false;
    private GameObject leftWall;
    private GameObject rightWall;
    private GameObject lastWall = null;

    private GameObject equippedWeapon = null;

    private void Awake()
    {

    }

    void Start()
    {
        /* Author: Connor French
         * Description: sets all values to initial levels. If the player won the last game, spawns the crown on top of their head
         */
        checkControls();
        rb2d = GetComponent<Rigidbody2D>();
        respawnTimer = initialRespawnTimer;
        health = initialHealth;
        anim = gameObject.GetComponent<Animator>();
        if (GameControl.lastWinner != 0 && GameControl.lastWinner == playerNum)
        {
            GameObject crown = Instantiate(GameControl.instance.crown, Vector3.zero, Quaternion.identity);
            crown.transform.parent = head.transform;
            crown.transform.localPosition = Vector3.zero;
        }
    }

    private void Update()
    {
        /* Author: Connor French
         * Description: runs necessary player logic. Checks if the controls are disabled, then moves/animates the player as necessary
         */
        //checks if controls are enabled or not
        resetControls();

        checkSlide();
        //most movement logic for the player
        movePlayer();
        float h = Input.GetAxis(hAxis);
        anim.SetBool("Running", Mathf.Abs(rb2d.velocity.x) > 0 && ableToJump);
        anim.SetFloat("Run Speed", Mathf.Abs(rb2d.velocity.x) / 20);
        anim.SetBool("Jumping", rb2d.velocity.y > 0 && !ableToJump); // Might be a problem for double jumping?
        anim.SetBool("Falling", rb2d.velocity.y < 0 && !sliding && !ableToJump);
        //checks if the player is attempting to pick up/drop weapon
        checkPickup();
        //checks if the player is throwing a weapon
        checkThrow();
        //checks if the player is attempting to jump off of the wall
        wallJump();
        //checks if the player is attempting to use their weapon
        fireWeapon();
        //checks to see if enabling the controls has changed
        lastControls = GameControl.instance.controlsDisabled;
        //if the game is over, run game over logic
        if (health <= 0)
        {
            dead = true;
        }
        if (!dead)
        {
            Text t = playerText.GetComponent<Text>();
            t.text = "Lives: " + lives.ToString();
        }
        if (GameControl.instance.fight || lives <= 1)
        {
            playerDeath();
        }
        gameEnd();
    }

    private void LateUpdate()
    {
        /* Author: Connor French
         * Description: limits velocity, flips player text accordingly, and checks if the player dies/respawns
         */
        if (rb2d.velocity.x > GameControl.instance.maxPlayerSpeed)
        {
            rb2d.velocity = new Vector2(GameControl.instance.maxPlayerSpeed, rb2d.velocity.y);
        }
        if (rb2d.velocity.y > GameControl.instance.maxPlayerSpeed)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, GameControl.instance.maxPlayerSpeed);
        }
        playerCanvas.transform.localScale = transform.localScale;
        if (!GetComponent<Renderer>().isVisible && !controlsDisabled)
        {
            die();
        }
        respawn();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /* Author: Connor French
         * Description: runs collision logic, resets control variables if hitting ground or wall, checks if jumping off of player if player
         */
        if (collision.gameObject.CompareTag("Ground") && rb2d.velocity.y == 0)
        {
            ableToJump = true;
            wallJumping = false;
            wallJumpNum = 0;
            shaker.SetActive(true);
            thud.Play();
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            float d = getWallBeneath();
            if(d <= wallDistanceBelow)
            {
                ableToJump = true;
                wallJumping = false;
                wallJumpNum = 0;
                shaker.SetActive(true);
                thud.Play();
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
        /* Author: Connor French
         * Description: runs collision logic on the chance that the engine missed the initial collision
         */
        if (collision.gameObject.CompareTag("Ground"))
        {
            ableToJump = true;
            wallJumpNum = 0;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            float d = getWallBeneath();
            if (d <= wallDistanceBelow)
            {
                ableToJump = true;
                wallJumping = false;
                wallJumpNum = 0;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        /* Author: Connor French
         * Description: runs on exiting collision, checks if footstooling other player
         */
        if (collision.gameObject.CompareTag("Ground"))
        {
            ableToJump = false;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (rb2d.velocity.y == 0)
            {
                Vector2 dist = getDistanceToWall();
                if (dist.x >= wallDistance && dist.y >= wallDistance)
                {
                    ableToJump = false;
                }
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
                thud.Play();
                ableToJump = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /* Author: Connor French
         * Description: runs on entering trigger, allows weapon to be picked up if necessary, kills player on entering a killbox off-screen, and checks if it's time to activate a tutorial textbox
         */
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
        if (collision.gameObject.CompareTag("Tutorial"))
        {
            GameControl.instance.tutorialCollision = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        /* Author: Connor French
         * Description: runs weapon pickup allowance again in case the engine missed the initial collision
         */
        if(collision.gameObject.CompareTag("Weapon"))
        {
            item = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /* Author: Connor French
         * Description: disallows pickup of any weapon if you are not close enough to it
         */
        if (collision.gameObject.CompareTag("Weapon"))
        {
            item = null;
        }
    }

    public void checkControls()
    {
        /* Author: Connor French
         * Description: runs at the beginning and determines which controllers the player is using
         */
        if (!GameControl.instance.USING_CONTROLLERS)
        {
            hAxis = "Horizontal";
            vAxis = "Vertical";
            jAxis = "Jump";
            fAxis = "Fire1";
            eAxis = "Fire2";
            tAxis = "Fire3";
            return;
        }
        if (GameControl.instance.USING_GAMECUBE_CONTROLLERS)
        {
            string num = playerNum.ToString();
            hAxis = "Horizontal_P" + num;
            jAxis = "Jump_P" + num;
            eAxis = "Equip_P" + num;
            fAxis = "Fire_P" + num;
            vAxis = "Vertical_P" + num;
            tAxis = "Throw_P" + num;
        }
        if (GameControl.instance.USING_SONY_CONTROLLERS)
        {
            if (playerNum == 1)
            {
                hAxis = "Horizontal_P1";
                jAxis = "Jump_P1_Sony";
                eAxis = "Equip_P1_Sony";
                fAxis = "Fire_P1_Sony";
                vAxis = "Vertical_P1";
                tAxis = "Throw_P1_Sony";
            }
            else
            {
                hAxis = "Horizontal_P2";
                jAxis = "Jump_P2_Sony";
                eAxis = "Equip_P2_Sony";
                fAxis = "Fire_P2_Sony";
                vAxis = "Vertical_P2";
                tAxis = "Throw_P2_Sony";
            }
            GameControl.instance.pAxis = "Pause_Sony";
        }
    }

    public void movePlayer()
    {
        /* Author: Connor French
         * Description: responsible for the majority of moving the player, adjusting its velocity and jump variables
         */
        if (!dead && !controlsDisabled && !GameControl.instance.paused)
        {
            bool v = Input.GetButtonDown(jAxis);
            float h = Input.GetAxis(hAxis);
            Vector2 dist = getDistanceToWall();
            if(rb2d.velocity.y <= 5 && wallJumping)
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
                        // Needs to be updated to not need reference to pther player
                        if (otherPlayer.ableToJump)
                        {
                            Rigidbody2D r = otherPlayer.GetComponent<Rigidbody2D>();
                            r.velocity = new Vector2(r.velocity.x, -jumpStrength);
                        }
                        footstool = false;
                    }
                }
                else
                {
                    if (sliding)
                    {
                        rb2d.gravityScale = 0;
                        rb2d.velocity = new Vector2(0, -wallSlideSpeed);
                    }
                    else
                    {
                        rb2d.gravityScale = 8;
                    }
                }
                if (sliding)
                {
                    if ((dist.x >= wallDistance && h < 0) || (dist.y >= wallDistance && h > 0))
                    {
                        rb2d.velocity = new Vector2(h * playerSpeed, rb2d.velocity.y);
                    }
                }
                else
                {
                    rb2d.velocity = new Vector2(h * playerSpeed, rb2d.velocity.y);
                }
                if (h != 0 && !sliding)
                {
                    transform.localScale = new Vector3(Mathf.Sign(h), transform.localScale.y, transform.localScale.z);
                }
            }
        }
    }

    public void respawn()
    {
        /* Author: Connor French
         * Description: respawns the player upon death and increments the life counter. If the player is out of lives, then it triggers an end of game
         */
        Text t = playerText.GetComponent<Text>();
        if (!GameControl.instance.ableToDie && lives > 1 && !GameControl.instance.gameOver)
        {
            GameObject crown = GameObject.FindGameObjectWithTag("Crown");
            if (dead && !GameControl.instance.paused)
            {
                Debug.Log("PLAYER " + playerNum.ToString() + " DIED");
                string color = getColor();
                t.text = "Respawning in " + string.Format("{0:N0}", Mathf.Ceil(respawnTimer));
                respawnTimer -= Time.deltaTime;
                rb2d.velocity = Vector2.zero;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                if(crown != null && crown.transform.parent == head.transform)
                {
                    crown.GetComponent<SpriteRenderer>().enabled = false;
                }
                gameObject.GetComponent<Collider2D>().isTrigger = true;
                healthBar.gameObject.SetActive(false);
                minimapIcon.SetActive(false);
                trail.SetActive(false);
                glow.SetActive(false);
                if(equippedWeapon != null)
                {
                    dropObject(new Vector2(UnityEngine.Random.Range(-10f, 10f), 50));
                }
            }
            if (respawnTimer <= 0)
            {
                if (GameControl.instance.tutorial)
                {
                    if (!GameControl.instance.inTutorial)
                    {
                        if (lives != 1 || GameControl.instance.reachedTop)
                        {
                            lives--;
                        }
                    }
                }
                else
                {
                    lives--;
                }
                int ind = GameControl.instance.getRespawnPlat();
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
                t.text = "";
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                gameObject.GetComponent<Collider2D>().isTrigger = false;
                equip.SetActive(true);
                transform.position = new Vector3(platforms[ind].transform.position.x, platforms[ind].transform.position.y + 3, transform.position.z);
                trail.SetActive(true);
                if (crown != null && crown.transform.parent == head.transform)
                {
                    crown.GetComponent<SpriteRenderer>().enabled = true;
                }
                rb2d.velocity = Vector2.zero;
                respawnTimer = initialRespawnTimer;
                dead = false;
                controlsDisabled = false;
                healthBar.gameObject.SetActive(true);
                health = initialHealth;
                healthBar.value = health;
                minimapIcon.SetActive(true);
                glow.SetActive(true);
            }
        }
        else
        {
            /*GameControl.instance.speedyMusic = true;
            GameControl.instance.fight = true;*/
        }
    }

    private void checkPickup()
    {
        /* Author: Connor French & Reynaldo Hermawan
         * Description: checks to see if you can pick something up and if there is something within range. If there is, then it is picked up and made a child of the player
         * Contributors: Caleb Biggers
         */
        if (controlsDisabled)
        {
            return;
        }
       
        // Check if player is trying to pick up object or got an object thrown at them
        if (Input.GetButtonDown(eAxis) || hit)
        {
            GameObject tempEquipedWeaponRef = equippedWeapon;
            if(equippedWeapon != null)
            {
                dropObject(new Vector2(-5 * transform.localScale.x, 20));
                //throw
                float h = Input.GetAxis(hAxis);
                float v = Input.GetAxis(vAxis);
            }
            if (item != null && item.gameObject != tempEquipedWeaponRef)
            {
                //pickup
                item.gameObject.transform.parent = equip.transform;

                // Find the weapon script attached to the weapon (All weapons must extend WeaponScript)
                equippedWeapon = item.gameObject;
                equippedWeapon.GetComponent<WeaponScript>().initWeapon(equippedWeapon, this.gameObject);
            }
            hit = false;
        }
    }

    private void fireWeapon()
    {
        /* Authors: Reynaldo Hermawan & Caleb Biggers
         * Description: checks to see if the player has a weapon equipped and if the player is interacting with the fire button
         * Contributor: Connor French
         */
        if (dead || controlsDisabled || equippedWeapon == null)
        {
            return;
        }

        if (Input.GetButtonDown(fAxis)){
            equippedWeapon.GetComponent<WeaponScript>().shoot();
        }
        else if (Input.GetButtonUp(fAxis))
        {
            equippedWeapon.GetComponent<WeaponScript>().stop();
        }
    }

    private void dropObject(Vector2 launchWeapon)
    {
        /* Author: Reynaldo Hermawan
         * Description: drops an object if the player hits the pickup button while already holding a weapon
         * Contributor: Connor French
         */
        //drop
        if (equippedWeapon == null) {
            return;
        }
        if(equip.transform.childCount == 0 || equippedWeapon != equip.transform.GetChild(0).gameObject){
            equippedWeapon = null;
            return;
        }
        // Reset the weapon
        equippedWeapon.GetComponent<WeaponScript>().resetWeapon(equippedWeapon, this.gameObject);
        // Set the parent to the pickup container
        equippedWeapon.transform.parent = GameControl.instance.pickups.transform;
        
        // Launch the weapon
        equippedWeapon.GetComponent<Rigidbody2D>().velocity = launchWeapon;

        // Set the equipped weapon back to null
        equippedWeapon = null;
        return;
    }

    private void checkThrow()
    {
        /* Author: Connor French
         * Description: checks if the player is holding a weapon and hitting the throw button. If yes, then it drops the object with a velocity.
         * Contributor: Reynaldo Hermawan 
         */
        if (controlsDisabled)
        {
            return;
        }
        if (Input.GetButtonDown(tAxis))
        {
            if (equip.transform.childCount == 1)
            {
                //throw
                dropObject(new Vector2(throwSpeed * transform.localScale.x, 10));
            }
        }
    }

    public float sign(float value)
    {
        /* Author: Connor French
         * Description: helper function that returns the sign of the input or 0 if the input is 0
         */
        if(value == 0)
        {
            return 0;
        }
        return Mathf.Sign(value);
    }

    public float checkWallJump(Rigidbody2D rb2d)
    {
        /* Author: Connor French
         * Description: checks which wall the player is closest to the left and right for use in wall-jump calculations
         */
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

    public Vector2 getDistanceToWall()
    {
        /* Author: Connor French
         * Description: gets the distance to each wall to the left and right and returns it as a vector
         */
        Vector2 result = new Vector2(Mathf.Infinity, Mathf.Infinity);
        float h = Input.GetAxis(hAxis);
        int ignore = LayerMask.GetMask("Wall");
        RaycastHit2D left = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.left), Mathf.Infinity, ignore);
        RaycastHit2D right = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.right), Mathf.Infinity, ignore);
        if (left.collider != null)
        {
            result.x = left.distance;
            leftWall = left.transform.gameObject;
        }
        if(right.collider != null)
        {
            result.y = right.distance;
            rightWall = right.transform.gameObject;
        }
        return result;
    }

    public float getWallBeneath()
    {
        /* Author: Connor French
         * Description: checks to see if the player is above a wall and returns its distance to be used to determine whether or not the player is standing on a wall
         */
        int ignore = LayerMask.GetMask("Wall", "Platform");
        RaycastHit2D down = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), Mathf.Infinity, ignore);
        if(down.collider != null)
        {
            return down.distance;
        }
        return Mathf.Infinity;
    }

    public void raycastToPlayer()
    {
        /* Author: Connor French
         * Description: checks to see if the player is above another player to determine if the player can jump off of their head
         */
        Vector2 result = new Vector2(Mathf.Infinity, Mathf.Infinity);
        int ignore = LayerMask.GetMask("Player");
        RaycastHit2D down = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), Mathf.Infinity, ignore);
        if(down.collider != null)
        {
            footstool = true;
            otherPlayer = down.transform.GetComponent<PlayerController>();
        }
        else
        {
            footstool = false;
        }
    }

    private void resetControls()
    {
        /* Author: Connor French
         * Description: checks to see if the GameController has disabled controls and sets the players controls accordingly
         */
        if (GameControl.instance.controlsDisabled)
        {
            controlsDisabled = true;
        }
        else if (lastControls)
        {
            controlsDisabled = false;
        }
    }

    public void checkSlide()
    {
        /* Author: Connor French
         * Description: checks to see if the player is close enough to the wall to slide and sets the animation check accordingly
         */
        Vector2 walls = getDistanceToWall();
        float d = getWallBeneath();
        if((walls.x < wallDistance || walls.y < wallDistance) && d >= wallDistanceBelow)
        {
            sliding = true;
            if (walls.x < walls.y)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            wallSlide.Play();
        }
        else
        {
            sliding = false;
            wallSlide.Stop();
        }
        anim.SetBool("Wall Sliding", sliding);
    }

    public void wallJump()
    {
        /* Author: Connor French
         * Description: runs the majority of the wall-jump functionality, moving the player according to which wall they are jumping off of
         */
        Vector2 walls = getDistanceToWall();
        if (Input.GetButtonDown(jAxis) && !ableToJump)
        {
            float h = Input.GetAxis(hAxis);
            float scale = 2;
            rb2d.gravityScale = 8;
            if (walls.x < wallDistance)
            {
                if (leftWall == lastWall)
                {
                    wallJumpNum++;
                }
                else
                {
                    wallJumpNum = 1;
                }
                lastWall = leftWall;
                rb2d.velocity = new Vector2(wallJumpStrength * scale, jumpStrength / wallJumpNum);
                wallJumping = true;
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            if (walls.y < wallDistance)
            {
                if (rightWall == lastWall)
                {
                    wallJumpNum++;
                }
                else
                {
                    wallJumpNum = 1;
                }
                lastWall = rightWall;
                rb2d.velocity = new Vector2(-wallJumpStrength * scale, jumpStrength / wallJumpNum);
                wallJumping = true;
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void playerDeath()
    {
        if(lives <= 1 && dead)
        {
            lives = 0;
            playerText.GetComponent<Text>().text = "You Died!";
        }
    }

    private void gameEnd()
    {
        /* Author: Connor French
         * Description: checks to see if the game is over and determines who the winner is and displays that information to the screen
         */
        if (GameControl.instance.gameOver)
        {
            if (dead)
            {
                rb2d.bodyType = RigidbodyType2D.Kinematic;
                rb2d.velocity = Vector2.zero;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<Collider2D>().isTrigger = true;
                healthBar.gameObject.SetActive(false);
                GameControl.instance.topText.gameObject.SetActive(false);
                if (equippedWeapon != null)
                {
                    dropObject(new Vector2(UnityEngine.Random.Range(-10f, 10f), 50));
                }
            }
            else
            {
                playerText.GetComponent<Text>().text = "WINNER";
                GameControl.instance.endOfGame(gameObject);
            }
        }
        /*
        if (dead && lives <= 1)
        {
            GameControl.instance.gameOver = true;
            GameControl.instance.speedyMusic = false;
            otherPlayer.rb2d.bodyType = RigidbodyType2D.Kinematic;
            otherPlayer.rb2d.velocity = Vector2.zero;
            rb2d.velocity = Vector2.zero;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<Collider2D>().isTrigger = true;
            healthBar.gameObject.SetActive(false);
            GameControl.instance.topText.gameObject.SetActive(false);
            Text t = playerText.GetComponent<Text>();
            t.text = "Lives: 0";
            if (equippedWeapon != null)
            {
                dropObject(new Vector2(UnityEngine.Random.Range(-10f, 10f), 50));
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
                if (playerNum == 1)
                {
                    GameControl.lastWinner = 2;
                }
                else
                {
                    GameControl.lastWinner = 1;
                }
                if (!ended)
                {
                    GameControl.instance.restartButton.Select();
                    ended = true;
                }
            }
        }*/
    }

    private string getWinner()
    {
        /* Author: Connor French
         * Description: helper function to return the other player's color
         */
        string color = "Red";
        if(playerNum == 1)
        {
            color = "Blue";
        }
        return color;
    }

    private string getColor()
    {
        /* Author: Connor French
         * Description: helper function to return the player's color
         */
        string color = "Blue";
        if (playerNum == 1)
        {
            color = "Red";
        }
        return color;
    }

    private void die()
    {
        /* Author: Connor French
         * Description: helper function to reset some values upon player death
         */
        dead = true;
        controlsDisabled = true;
        rb2d.velocity = Vector2.zero;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }


    public void receiveDamage(float dmgTaken){
        /* Author: Reynaldo Hermawan
         * Description: allows players to take damage from any weapon source
         */
        health -= dmgTaken;
        healthBar.value = health;
        if (health <= 0)
        {
            dead = true;
        }
    }
}
