using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;
    public float scaleDelay;
    public Text statusText;
    public Text p1Text;
    public Text p2Text;
    
    public GameObject fightPlatform;
    public float cameraSpeed;
    public float adjustSpeed;
    public float cameraOffsetX;
    public float cameraOffsetY;
    public GameObject player1;
    public GameObject player2;
    public GameObject cam;
    public bool climbing = false;
    public bool reachedTop = false;
    public bool aboveScreen = false;
    public bool controlsDisabled = true;
    public Text bottomText;
    public Text topText;
    public bool paused = false;
    public GameObject cameraPath;
    public GameObject pickups;
    public bool ableToDie = false;
    public bool gameOver = false;
    public Button quitButton;
    public Button restartButton;
    public float maxPlayerSpeed;
    public Transform killboxes;
    public float flamethrowerDamage = 33f;
    public float avalancheDamage = 50f;
    public bool fight = false;
    public Sprite platformSprite;
    public Sprite wallSprite;
    
    private string previousText;
    private float cameraShaking;
    private int prevPath = 0;
    private int nextPath = 1;
    private GameObject[] waypoints;
    private GameObject lastWaypoint;
    private float climbDelay;
    private Vector3 cameraDir;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //stopParticles();
        changeSprites();
    }

    private void Start()
    {
        statusText.text = "";
        p1Text.text = "";
        p2Text.text = "";
        bottomText.gameObject.SetActive(false);
        topText.gameObject.SetActive(false);
        generateWaypoints();
        lastWaypoint = waypoints[waypoints.Length - 1];
        climbDelay = scaleDelay;
        paused = false;
        Time.timeScale = 1;
    }

    void Update()
    {
        if (gameOver)
        {
            controlsDisabled = true;
        }
        beginGame();
        if (reachedTop)
        {
            if (!gameOver)
            {
                topText.gameObject.SetActive(true);
            }
            if (player1.gameObject.GetComponent<SpriteRenderer>().enabled && player2.gameObject.GetComponent<SpriteRenderer>().enabled)
            {
                ableToDie = true;
            }
        }
        aboveScreen = cameraAdjust();
        wayfind();
        killboxDespawn();
        pauseGame();
    }

    private bool cameraAdjust()
    {
        bool result = false;
        bool addition;
        if(cameraDir.x != 0)
        {
            if(cameraDir.x > 0)
            {
                addition = (player1.transform.position.x > cam.transform.position.x + cameraOffsetX) || (player2.transform.position.x > cam.transform.position.x + cameraOffsetX);
                result = result || addition;
            }
            else
            {
                addition = (player1.transform.position.x + cameraOffsetX < cam.transform.position.x) || (player2.transform.position.x + cameraOffsetX < cam.transform.position.x);
                result = result || addition;
            }
        }
        if(cameraDir.y != 0)
        {
            if (cameraDir.y > 0)
            {
                addition = (player1.transform.position.y > cam.transform.position.y + cameraOffsetY) || (player2.transform.position.y > cam.transform.position.y + cameraOffsetY);
                result = result || addition;
            }
            else
            {
                addition = (player1.transform.position.y + cameraOffsetY < cam.transform.position.y) || (player2.transform.position.y + cameraOffsetY < cam.transform.position.y);
                result = result || addition;
            }
        }
        return result;
    }

    private void pauseGame()
    {
        if (Input.GetButtonDown("Pause") && !gameOver && climbing)
        {
            if (paused)
            {
                Time.timeScale = 1;
                statusText.text = previousText;
            }
            else
            {
                if (climbing)
                {
                    Time.timeScale = 0;
                    previousText = statusText.text;
                    statusText.text = "PAUSED";
                }
            }
            paused = !paused;
        }
    }

    private void beginGame()
    {
        if (!climbing)
        {
            if (climbDelay > 0)
            {
                statusText.text = "Climbing in " + string.Format("{0:N0}", Mathf.Ceil(climbDelay));
                climbDelay -= Time.deltaTime;
                controlsDisabled = true;
            }
            else
            {
                climbing = true;
                statusText.text = "";
                controlsDisabled = false;
                bottomText.gameObject.SetActive(true);
                Input.ResetInputAxes();
            }
        }
    }

    public void DestroyPlats()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platforms");
        foreach (GameObject plat in platforms)
        {
            plat.SetActive(false);
        }
    }

    public int getRespawnPlat()
    {
        float minimum = Mathf.Infinity;
        int ind = -1;
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
        for (int i = 0; i < platforms.Length; i++)
        {
            float dist = Mathf.Sqrt(Mathf.Pow((cam.transform.position.y - platforms[i].transform.position.y), 2) + Mathf.Pow((cam.transform.position.x - platforms[i].transform.position.x), 2));
            if (dist < minimum && dist > 0)
            {
                minimum = dist;
                ind = i;
            }
        }
        return ind;
    }

    public void wayfind()
    {
        if (!reachedTop && climbing)
        {
            Vector3 dir = waypoints[nextPath].transform.position - waypoints[prevPath].transform.position;
            dir = dir.normalized;
            cameraDir = dir;
            if (aboveScreen)
            {
                cam.transform.position = new Vector3(cam.transform.position.x + adjustSpeed * cameraSpeed * Time.deltaTime * dir.x, cam.transform.position.y + adjustSpeed * cameraSpeed * Time.deltaTime * dir.y, cam.transform.position.z);
            }
            else
            {
                cam.transform.position = new Vector3(cam.transform.position.x + cameraSpeed * Time.deltaTime * dir.x, cam.transform.position.y + cameraSpeed * Time.deltaTime * dir.y, cam.transform.position.z);
            }
            bool comparison = false;
            if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if(Mathf.Sign(dir.x) == 1)
                {
                    comparison = cam.transform.position.x >= waypoints[nextPath].transform.position.x;
                }
                else
                {
                    comparison = cam.transform.position.x <= waypoints[nextPath].transform.position.x;
                }
            }
            else
            {
                if (Mathf.Sign(dir.y) == 1)
                {
                    comparison = cam.transform.position.y >= waypoints[nextPath].transform.position.y;
                }
                else
                {
                    comparison = cam.transform.position.y <= waypoints[nextPath].transform.position.y;
                }
            }
            if (comparison)
            {
                if (nextPath == waypoints.Length - 1)
                {
                    reachedTop = true;
                }
                else
                {
                    nextPath++;
                    prevPath++;
                }
            }
        }
    }

    public void generateWaypoints()
    {
        Transform p = cameraPath.transform;
        GameObject[] w = new GameObject[p.childCount];
        for(int i = 0; i < p.childCount; i++)
        {
            w[i] = p.GetChild(i).gameObject;
        }
        waypoints = w;
    }

    public void killboxDespawn()
    {
        GameObject bottom = killboxes.GetChild(0).gameObject;
        GameObject top = killboxes.GetChild(1).gameObject;
        GameObject left = killboxes.GetChild(2).gameObject;
        GameObject right = killboxes.GetChild(3).gameObject;
        if (cameraDir.y > 0)
        {
            top.SetActive(false);
            bottom.SetActive(true);
        }
        else if (cameraDir.y < 0)
        {
            top.SetActive(true);
            bottom.SetActive(false);
        }
        else
        {
            top.SetActive(false);
            bottom.SetActive(true);
        }
        if (cameraDir.x > 0)
        {
            right.SetActive(false);
            left.SetActive(true);
        }
        else if(cameraDir.x < 0)
        {
            right.SetActive(true);
            left.SetActive(false);
        }
        else
        {
            right.SetActive(true);
            left.SetActive(true);
        }
    }

    public void stopParticles()
    {
        GameObject[] particles = GameObject.FindGameObjectsWithTag("Particles");
        for(int i = 0; i < particles.Length; i++)
        {
            ParticleSystem ps = particles[i].GetComponent<ParticleSystem>();
            ps.Stop();
        }
    }

    public void changeSprites()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
        for(int i = 0; i < platforms.Length; i++)
        {
            platforms[i].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = platformSprite;
        }
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        for(int i = 0; i < walls.Length; i++)
        {
            walls[i].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = wallSprite;
        }
    }
}
