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
    public float avalancheDamage = 50f;
    public bool fight = false;
    public Sprite platformSprite;
    public Sprite wallSprite;
    public AudioClip music;
    public bool tutorial = false;
    public Textbox textbox;
    public bool textboxDestroyed = false;
    public float initialCameraSpeed;
    public bool inTutorial = false;
    public bool tutorialCollision = false;

    private string previousText;
    private float cameraShaking;
    private int prevPath = 0;
    private int nextPath = 1;
    private GameObject[] waypoints;
    private GameObject lastWaypoint;
    private float climbDelay;
    private Vector3 cameraDir;
    private int tutorialCount = 0;
    private string[] tutorialText = {"Welcome to the Tutorial Level! Here is where you will learn everything you need to know to become a champion!", "If you want to survive, you're going to have to climb out of here. Press A to jump!" , "You're also going to have to fight, so pick up weapons with B and use them with L or R!", "If you don't like your weapon, you can drop it by pressing B again or throw it by pressing X. Try throwing it at an opponent!", "The camera is about to start moving. Be careful not to fall behind!", "If you fall out of the view of the camera, you will lose a life!", "You only have a limited number of lives, and if they all run out, you lose!", "You may run into an area that has no platforms. You can jump off of the walls by holding towards the wall and pressing the A button!", "When you reach the final platform, all other platforms will drop away and you will duel each other to drain each other's lives!"};

    public bool USING_CONTROLLERS = false;
    public bool USING_GAMECUBE_CONTROLLERS = false;
    public bool USING_SONY_CONTROLLERS = false;

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
        stopParticles();
        changeSprites();
        string[] controllers = Input.GetJoystickNames();
        if(controllers.Length == 0)
        {
            USING_CONTROLLERS = false;
            USING_GAMECUBE_CONTROLLERS = false;
            USING_SONY_CONTROLLERS = false;
            return;
        }
        for(int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i].Contains("vJoy"))
            {
                USING_GAMECUBE_CONTROLLERS = true;
            }
            else
            {
                USING_SONY_CONTROLLERS = true;
            }
        }
        USING_CONTROLLERS = USING_GAMECUBE_CONTROLLERS || USING_SONY_CONTROLLERS;
        Debug.Log("Using Controllers: " + USING_CONTROLLERS + ", Gamecube?: " + USING_GAMECUBE_CONTROLLERS + ", Sony?: " + USING_SONY_CONTROLLERS);
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
        Music.instance.music.clip = music;
        Music.instance.music.Play();
        initialCameraSpeed = cameraSpeed;
        if(SceneManager.GetActiveScene().name == "Tutorial_Level")
        {
            tutorial = true;
            setText(tutorialText[tutorialCount]);
        }
    }

    void Update()
    {
        if (tutorial)
        {
            runTutorial();
        }
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
                //ableToDie = true;
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
                Music.instance.music.volume = 1;
            }
            else
            {
                if (climbing)
                {
                    Time.timeScale = 0;
                    previousText = statusText.text;
                    statusText.text = "PAUSED";
                    Music.instance.music.volume = 0.25f;
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
        if (!reachedTop && climbing && !gameOver)
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

    public void setText(string s)
    {
        if (!tutorial)
        {
            return;
        }
        GameObject[] t = GameObject.FindGameObjectsWithTag("Textbox");
        if(t.Length != 0)
        {
            for(int i = 0; i < t.Length; i++)
            {
                Destroy(t[i]);
            }
        }
        Textbox text = Instantiate(textbox, new Vector3(cam.transform.position.x, cam.transform.position.y - 11, -5), Quaternion.identity) as Textbox;
        text.content = s;
        text.transform.parent = cam.transform;
    }

    public void runTutorial()
    {
        GameObject[] exists = GameObject.FindGameObjectsWithTag("Textbox");
        inTutorial = exists.Length != 0;
        bool triggerText = false;
        if (!inTutorial)
        {
            if(cam.transform.position.y >= 184.5 && tutorialCount == 7)
            {
                tutorialCount = 8;
                triggerText = true;
            }
            if(cam.transform.position.y >= 115 && tutorialCount == 6)
            {
                tutorialCount = 7;
                triggerText = true;
            }
            if (textboxDestroyed && tutorialCount == 5)
            {
                tutorialCount = 6;
                triggerText = true;
            }
            if (tutorialCount == 4 && ((player1.GetComponent<PlayerController>().dead && player1.GetComponent<PlayerController>().healthBar.value > 0) || (player2.GetComponent<PlayerController>().dead && player2.GetComponent<PlayerController>().healthBar.value > 0)))
            {
                tutorialCount = 5;
                triggerText = true;
            }
            if (textboxDestroyed && tutorialCount == 3)
            {
                tutorialCount = 4;
                triggerText = true;
            }
            if (textboxDestroyed && tutorialCount == 2)
            {
                tutorialCount = 3;
                triggerText = true;
            }
            if (textboxDestroyed && tutorialCount == 1)
            {
                tutorialCount = 2;
                triggerText = true;
            }
            if (textboxDestroyed && tutorialCount == 0)
            {
                tutorialCount = 1;
                triggerText = true;
            }
        }

        if (tutorialCount < tutorialText.Length && triggerText)
        {
            setText(tutorialText[tutorialCount]);
            triggerText = false;
        }
    }
}
