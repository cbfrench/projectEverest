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

    private bool DEBUG = false;

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
        }
        else
        {
            hAxis = "Horizontal_P2";
            jAxis = "Jump_P2";
            eAxis = "Equip_P2";
            fAxis = "Fire_P2";
            vAxis = "Vertical_P2";
        }
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        respawnTimer = initialRespawnTimer;
    }

    private void Update()
    {
        //checks if controls are enabled or not
        resetControls();
        //most movement logic for the player
        movePlayer();
        //checks if the player is attempting to pick up/drop weapon
        checkPickup();
        //checks if the player is attempting to jump off of the wall
        wallJump();
        //checks if the player is attempting to use their weapon
        fireWeapon();
        //checks to see if enabling the controls has changed
        lastControls = GameControl.instance.controlsDisabled;
        //if the game is over, run game over logic
        if (GameControl.instance.gameOver)
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
        if (collision.gameObject.CompareTag("Wall") && rb2d.velocity.y == 0)
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
            if(collision.GetComponent<Rigidbody2D>().velocity.magnitude > 1)
            {
                //weapon was thrown/dropped from high up
                hit = true;
            }
        }
        if (collision.gameObject.CompareTag("Killbox"))
        {
            dead = true;
            controlsDisabled = true;
            rb2d.velocity = Vector2.zero;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            equip.SetActive(false);
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

    public void movePlayer()
    {
        if (!dead && !controlsDisabled && !GameControl.instance.paused)
        {
            bool v = Input.GetButtonDown(jAxis);
            float h = Input.GetAxis(hAxis);
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
                rb2d.velocity = new Vector2(h * playerSpeed, rb2d.velocity.y);
                if (h != 0)
                {
                    transform.localScale = new Vector3(Mathf.Sign(h), transform.localScale.y, transform.localScale.z);
                }
            }
        }
    }

    public void respawn()
    {
        if (!GameControl.instance.ableToDie)
        {
            if (dead && !GameControl.instance.paused)
            {
                string color = getColor();
                GameControl.instance.statusText.text = color + " respawning in " + string.Format("{0:N0}", Mathf.Ceil(respawnTimer));
                respawnTimer -= Time.deltaTime;
                rb2d.velocity = Vector2.zero;
            }
            if (respawnTimer <= 0)
            {
                int ind = GameControl.instance.getRespawnPlat();
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
                GameControl.instance.statusText.text = "";
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                equip.SetActive(true);
                transform.position = new Vector3(platforms[ind].transform.position.x, platforms[ind].transform.position.y + 3, transform.position.z);
                rb2d.velocity = Vector2.zero;
                respawnTimer = initialRespawnTimer;
                dead = false;
                controlsDisabled = false;
                firing = false;
            }
        }
        else
        {
            GameControl.instance.gameOver = true;
        }
    }

    private void checkPickup()
    {
        if (Input.GetButtonDown(eAxis) || hit)
        {
            Transform dropped = null;
            if(equip.transform.childCount == 1)
            {
                //drop
                dropped = equip.transform.GetChild(0);
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
                    dropped.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    dropped.GetComponent<Rigidbody2D>().gravityScale = 5;
                }
                if (dropped.name.Contains("Flamethrower") || dropped.name.Contains("Squirt Gun"))
                {
                    dropped.transform.GetChild(0).GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();
                }
                //throw
                float h = Input.GetAxis(hAxis);
                float v = Input.GetAxis(vAxis);
                dropped.GetComponent<Rigidbody2D>().velocity = new Vector2(sign(h) * throwSpeed, 10);
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

    public void wallJump()
    {
        Vector2 walls = getDistanceToWall();
        if (Input.GetButtonDown(jAxis) && !ableToJump)
        {
            float h = Input.GetAxis(hAxis);
            float scale = 1;
            if (walls.x < 1)
            {
                if (h > 0)
                {
                    scale = 2;
                }
                if (h < 0)
                {
                    scale = 0.5f;
                }
                wallJumpNum++;
                rb2d.velocity = new Vector2(wallJumpStrength * scale, jumpStrength / wallJumpNum);
                wallJumping = true;
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            if (walls.y < 1)
            {
                if (h < 0)
                {
                    scale = 2;
                }
                if (h > 0)
                {
                    scale = 0.5f;
                }
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
            Time.timeScale = 0;
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
        if (dead)
        {
            return;
        }
        if (Input.GetButtonDown(fAxis))
        {
            firing = true;
            singleFire = true;
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
}
