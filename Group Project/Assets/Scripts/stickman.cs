using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stickman : MonoBehaviour
{
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
    public AudioSource thud;

    private bool lastControls;
    private int wallJumpNum = 0;
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
    private bool sliding = false;

    private void Awake()
    {
        
    }

    void Start()
    {
        checkControls();
        rb2d = GetComponent<Rigidbody2D>();
        respawnTimer = initialRespawnTimer;
        health = initialHealth;
        anim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        //checks if controls are enabled or not
        resetControls();

        checkSlide();
        //most movement logic for the player
        movePlayer();
        float h = Input.GetAxis(hAxis);
        anim.SetBool("Running", Mathf.Abs(rb2d.velocity.x) > 0 && ableToJump);
        anim.speed = Mathf.Abs(rb2d.velocity.x) / 20;
        //checks if the player is attempting to pick up/drop weapon
        checkPickup();
        //checks if the player is throwing a weapon
        checkThrow();
        //checks if the player is attempting to jump off of the wall
        wallJump();
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
        if(rb2d.velocity.x > GameControl.instance.maxPlayerSpeed)
        {
            rb2d.velocity = new Vector2(GameControl.instance.maxPlayerSpeed, rb2d.velocity.y);
        }
        if (rb2d.velocity.y > GameControl.instance.maxPlayerSpeed)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, GameControl.instance.maxPlayerSpeed);
        }
        playerCanvas.transform.localScale = transform.localScale;
        respawn();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
            Vector2 dist = getDistanceToWall();
            if(dist.x >= 1 && dist.y >= 1)
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            ableToJump = true;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 dist = getDistanceToWall();
            if (dist.x >= 1 && dist.y >= 1)
            {
                ableToJump = true;
                wallJumping = false;
                wallJumpNum = 0;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ableToJump = false;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (rb2d.velocity.y == 0)
            {
                Vector2 dist = getDistanceToWall();
                if (dist.x >= 1 && dist.y >= 1)
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

    public void checkControls()
    {
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
        if (GameControl.instance.USING_SONY_CONTROLLERS)
        {
            // Caleb, your specific axes can be set here, just remember to set them in the Unity editor and don't modify any existing ones
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
    }

    public void movePlayer()
    {
        if (!dead && !controlsDisabled && !GameControl.instance.paused)
        {
            bool v = Input.GetButtonDown(jAxis);
            float h = Input.GetAxis(hAxis);
            Vector2 dist = getDistanceToWall();
            if(rb2d.velocity.y <= 0 && wallJumping)
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
                else
                {
                    if (sliding)
                    {
                        rb2d.gravityScale = 0;
                        rb2d.velocity = new Vector2(0, -5);
                    }
                    else
                    {
                        rb2d.gravityScale = 8;
                    }
                }
                if (sliding)
                {
                    if ((dist.x >= 1 && h < 0) || (dist.y >= 1 && h > 0))
                    {
                        rb2d.velocity = new Vector2(h * playerSpeed, rb2d.velocity.y);
                    }
                }
                else
                {
                    rb2d.velocity = new Vector2(h * playerSpeed, rb2d.velocity.y);
                }
                if (h != 0)
                {
                    transform.localScale = new Vector3(Mathf.Sign(h), transform.localScale.y, transform.localScale.z);
                }
            }
        }
    }

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
                rb2d.velocity = Vector2.zero;
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
                rb2d.velocity = Vector2.zero;
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
                    if(hit)
                    {
                        health -= 30;
                        healthBar.value = health;
                        if(health <= 0)
                        {
                            dead = true;
                        }
                    }
                }
                else if (item.name.Contains("Grapple"))
                {

                }
                else
                {
                    ParticleSystem ps = item.transform.GetChild(0).GetChild(1).gameObject.GetComponent<ParticleSystem>();
                    if (ps.isPlaying)
                    {
                        ps.Stop();
                    }
                }

                item.GetComponent<PickupController>().isEquipped = true;
                item.GetComponent<PickupController>().player = this.gameObject;
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
        dropped.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        if (dropped.name.Contains("Flashlight"))
        {
            dropped.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
            dropped.GetComponent<Rigidbody2D>().gravityScale = 5;
        }
        else if (dropped.name.Contains("Grapple"))
        {

        }
        else
        {
            dropped.transform.GetChild(0).GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();
            dropped.transform.GetChild(0).GetChild(1).gameObject.GetComponent<AudioSource>().Stop();
        }
        dropped.parent = GameControl.instance.pickups.transform;

        dropped.GetComponent<PickupController>().isEquipped = false;
        dropped.GetComponent<PickupController>().player = null;
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

    public float checkWallJump(Rigidbody2D rb2d)
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

    public void checkSlide()
    {
        Vector2 walls = getDistanceToWall();
        if(walls.x < 1 || walls.y < 1)
        {
            sliding = true;
        }
        else
        {
            sliding = false;
        }
    }

    public void wallJump()
    {
        Vector2 walls = getDistanceToWall();
        if (Input.GetButtonDown(jAxis) && !ableToJump)
        {
            float h = Input.GetAxis(hAxis);
            float scale = 2;
            rb2d.gravityScale = 8;
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
    }

    private void gameEnd()
    {
        if (dead)
        {
            GameControl.instance.gameOver = true;
            otherPlayer.rb2d.bodyType = RigidbodyType2D.Kinematic;
            otherPlayer.rb2d.velocity = Vector2.zero;
            rb2d.velocity = Vector2.zero;
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
        if (dead || controlsDisabled || equip.transform.childCount == 0)
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
        if (singleFire && equip.transform.childCount == 1 && equip.transform.GetChild(0).name.Contains("Flashlight"))
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
        if (equip.transform.GetChild(0).name.Contains("Grapple"))
        {
            return;
        }
        GameObject projectile = equip.transform.GetChild(0).gameObject; 
        projectile = equip.transform.GetChild(0).transform.GetChild(0).GetChild(1).gameObject;
        ParticleSystem ps = projectile.GetComponent<ParticleSystem>();
        AudioSource sound = ps.gameObject.GetComponent<AudioSource>();
        if (firing)
        {
            if (!ps.isEmitting)
            {
                ps.Play();
                if(sound != null)
                {
                    sound.Play();
                }
            }
        }
        else
        {
            if (ps.isEmitting)
            {
                ps.Stop();
                if(sound != null)
                {
                    sound.Stop();
                }
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
        rb2d.velocity = Vector2.zero;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
