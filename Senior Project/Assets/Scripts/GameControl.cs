using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameControl : MonoBehaviour
{
    /* Author: Connor French
     * Description: class for controlling the flow of gameplay as well as storing important variables
     * Contributor: Reynaldo Hermawan
     */
    public static GameControl instance;
    public float scaleDelay;
    public Text statusText;
    /*public Text p1Text;
    public Text p2Text;
    */
    public GameObject fightPlatform;
    public float cameraSpeed;
    public float adjustSpeed;
    public float cameraOffsetX;
    public float cameraOffsetY;
    public GameObject player;
    public GameObject playerText;
    /*public GameObject player1;
    public GameObject player2;*/
    public GameObject cam;
    public GameObject shaker;
    public bool climbing = false;
    public bool reachedTop = false;
    public bool aboveScreen = false;
    public bool controlsDisabled = true;
    public Canvas screenText;
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
    public static int lastWinner = 0;
    public GameObject crown;
    public string pAxis = "Pause";
    public GameObject topKillBox;
    public bool speedyMusic = false;
    public bool lastLife = false;

    private GameObject[] players = new GameObject[4];
    private int numberOfPlayers;
    private string[] controllers;
    private string previousText;
    private float cameraShaking;
    private int prevPath = 0;
    private int nextPath = 1;
    private GameObject[] waypoints;
    private GameObject lastWaypoint;
    private float climbDelay;
    private Vector3 cameraDir;
    private int tutorialCount = 0;
    private float lastMusicTime;
    private string[] tutorialText = {"Welcome to the Tutorial Level! Here is where you will learn everything you need to know to become a champion!", "If you want to survive, you're going to have to climb out of here. Press A to jump!" , "You're also going to have to fight, so pick up weapons with B and use them with L or R!", "If you don't like your weapon, you can drop it by pressing B again or throw it by pressing X. Try throwing it at an opponent!", "The camera is about to start moving. Be careful not to fall behind!", "If you fall out of the view of the camera, you will lose a life!", "You only have a limited number of lives, and if they all run out, you lose!", "You may run into an area that has no platforms. You can jump off of the walls by holding towards the wall and pressing the A button!", "When you reach the final platform, all other platforms will drop away and you will duel each other to drain each other's lives!"};
    private Color32[] playerColors = { new Color32(255, 0, 0, 255), new Color32(0, 245, 255, 255), new Color(0, 255, 0, 255), new Color(255, 255, 0, 255) };
    private string[] playerColorWords = { "Red", "Blue", "Green", "Yellow" };
    private float gameEndDelay = 3f;
    private bool once = false;

    public bool USING_CONTROLLERS = false;
    public bool USING_GAMECUBE_CONTROLLERS = false;
    public bool USING_SONY_CONTROLLERS = false;

    public GameObject spawnLoc;
    public GameObject weaponList;

    void Awake()
    {
        /* Author: Connor French
         * Description: ensures that only one GameControl exists and determines which control setup is being used based on connected devices
         */
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
        controllers = Input.GetJoystickNames();
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
        /* Author: Connor French
         * Description: sets all values to initial required values. Gets waypoints for the camera movement, plays the music, and determines if this level has a tutorial in it
         */
        createPlayers();
        statusText.text = "";
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
            if (USING_SONY_CONTROLLERS)
            {
                tutorialText[1] = "If you want to survive, you're going to have to climb out of here. Press X to jump!";
                tutorialText[2] = "You're also going to have to fight, so pick up weapons with Square and use them with L2 or R2!";
                tutorialText[3] = "If you don't like your weapon, you can drop it by pressing Square again or throw it by pressing Circle. Try throwing it at an opponent!";
                tutorialText[7] = "You may run into an area that has no platforms. You can jump off of the walls by holding towards the wall and pressing the Square button!";
            }
            tutorial = true;
            setText(tutorialText[tutorialCount]);
        }
        //weaponList = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g=>g.tag=="Weapon").ToList();
    }

    void Update()
    {
        /* Author: Connor French
         * Description: runs tutorial if in tutorial level, runs beginning game logic, checks where the players are on the screen, moves the camera and despawns appropriate killboxes, and checks if the player has paused the game
         */
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
            /*
            if (player1.gameObject.GetComponent<SpriteRenderer>().enabled && player2.gameObject.GetComponent<SpriteRenderer>().enabled)
            {
                //ableToDie = true;
            }*/
        }
        aboveScreen = cameraAdjust();
        wayfind();
        killboxDespawn();
        pauseGame();
        if (speedyMusic)
        {
            Music.instance.music.pitch = 1.25f;
        }
        else
        {
            Music.instance.music.pitch = 1;
        }
        checkGameOver();
    }

    private void createPlayers()
    {
        int i;
        numberOfPlayers = PlayerNumberSelect.numberOfPlayers;
        if(numberOfPlayers == 0)
        {
            numberOfPlayers = Input.GetJoystickNames().Length;
        }
        for (i = 0; i < numberOfPlayers; i++)
        {
            GameObject generated = Instantiate(player, new Vector3(i * 2, 0, 0), Quaternion.identity);
            generated.GetComponent<PlayerController>().playerNum = i + 1;
            generated.GetComponent<SpriteRenderer>().color = playerColors[i];
            generated.transform.Find("Glow").GetComponent<Light>().color = playerColors[i];
            generated.transform.Find("CameraShake").GetComponent<CameraShake>().camTransform = shaker.transform;
            GameObject pText = Instantiate(playerText, new Vector3(-300 + 200 * i, 0, 0), Quaternion.identity);
            pText.transform.SetParent(screenText.transform, false);
            pText.GetComponent<Text>().color = playerColors[i];
            pText.GetComponent<Text>().text = "";
            generated.GetComponent<PlayerController>().playerText = pText;
            Renderer minimapColor = generated.transform.Find("Minimap Icon").GetComponent<Renderer>();
            minimapColor.material.shader = Shader.Find("_Color");
            minimapColor.material.SetColor("_Color", playerColors[i]);
            minimapColor.material.shader = Shader.Find("Unlit/Color");
            minimapColor.material.SetColor("_UnlitColor", playerColors[i]);
            players[i] = generated;
        }
    }

    private bool cameraAdjust()
    {
        /* Author: Connor French
         * Description: checks whether or not the camera needs to be adjusted in the direction of the camera's movement
         */
        bool result = false;
        bool addition = false;
        int i;
        if(cameraDir.x != 0)
        {
            if(cameraDir.x > 0)
            {
                for(i = 0; i < numberOfPlayers; i++)
                {
                    addition |= (players[i].transform.position.x > cam.transform.position.x + cameraOffsetX);
                }
                result = result || addition;
            }
            else
            {
                for (i = 0; i < numberOfPlayers; i++)
                {
                    addition |= (players[i].transform.position.x + cameraOffsetX < cam.transform.position.x);
                }
                result = result || addition;
            }
        }
        if(cameraDir.y != 0)
        {
            if (cameraDir.y > 0)
            {
                for (i = 0; i < numberOfPlayers; i++)
                {
                    addition |= (players[i].transform.position.y > cam.transform.position.y + cameraOffsetY);
                }
                result = result || addition;
            }
            else
            {
                for (i = 0; i < numberOfPlayers; i++)
                {
                    addition |= (players[i].transform.position.y + cameraOffsetY < cam.transform.position.y);
                }
                result = result || addition;
            }
        }
        return result;
    }

    private void pauseGame()
    {
        /* Author: Connor French
         * Description: pauses the game if the pause button is pressed
         */
        if (Input.GetButtonDown(pAxis) && !gameOver && climbing)
        {
            if (paused)
            {
                Time.timeScale = 1;
                statusText.text = previousText;
                Music.instance.music.volume = 1;
                Music.instance.music.pitch = lastMusicTime;
            }
            else
            {
                if (climbing)
                {
                    Time.timeScale = 0;
                    previousText = statusText.text;
                    statusText.text = "PAUSED";
                    Music.instance.music.volume = 0.25f;
                    lastMusicTime = Music.instance.music.pitch;
                    Music.instance.music.pitch = 1;
                }
            }
            paused = !paused;
        }
    }

    private void beginGame()
    {
        /* Author: Connor French
         * Description: counts the clock down at the beginning of the game and begins the game once it reaches 0
         */
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
        /* Author: Connor French & Reynaldo Hermawan
         * Description: Destroys all extraneous platforms when the game reaches the final fight also starts weapon spawning
         */
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platforms");
        foreach (GameObject plat in platforms)
        {
            plat.SetActive(false);
        }
        spawnLoc.GetComponent<SpawnBox>().startRandomWeaponSpawn();
    }

    public int getRespawnPlat()
    {
        /* Author: Connor French
         * Description: checks which platform is the closest to the middle of the screen without being below it in order to respawn a dead player safely
         */
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
        /* Author: Connor French
         * Description: moves the camera along the path dictated by the GameObjects held within CameraPath in the scene
         */
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
        /* Author: Connor French
         * Description: creates a list of the waypoints that the camera must follow
         */
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
        /* Author: Connor French
         * Description: removes the killboxes in the direction of movement so that moving too far ahead won't kill the player
         */
        GameObject bottom = killboxes.GetChild(0).gameObject;
        GameObject top = killboxes.GetChild(1).gameObject;
        GameObject left = killboxes.GetChild(2).gameObject;
        GameObject right = killboxes.GetChild(3).gameObject;
        if (tutorial)
        {
            return;
        }
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
        /* Author: Connor French
         * Description: stops all tagged Particle Systems from emitting
         */
        GameObject[] particles = GameObject.FindGameObjectsWithTag("Particles");
        for(int i = 0; i < particles.Length; i++)
        {
            ParticleSystem ps = particles[i].GetComponent<ParticleSystem>();
            ps.Stop();
        }
    }

    public void changeSprites()
    {
        /* Author: Connor French
         * Description: changes all wall and floor sprites in a scene to the ones given in the GameController to prevent need for similar prefabs
         */
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

    public bool anyDead()
    {
        int i;
        for(i = 0; i < numberOfPlayers; i++)
        {
            if (players[i].GetComponent<PlayerController>().dead)
            {
                return true;
            }
        }
        return false;
    }

    public void setText(string s)
    {
        /* Author: Connor French
         * Description: creates a textbox and sets its text to the given string
         */
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
        /* Author: Connor French
         * Description: runs the tutorial level logic for displaying helpful textboxes
         */
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
            if (tutorialCount == 4 && anyDead())
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

    public GameObject returnRandomWeapon(){
        /* Author: Reynaldo Hermawan
         * Description: Returns a random weapon to spawn into the game.
         */
        return weaponList.transform.GetChild(Random.Range(0,weaponList.transform.childCount)).gameObject;
    }
    private void checkGameOver()
    {
        if (!gameOver)
        {
            int i;
            int alive = 0;
            //lastLife = false;
            for (i = 0; i < numberOfPlayers; i++)
            {
                PlayerController current = players[i].GetComponent<PlayerController>();
                if (!current.dead || current.lives >= 1)
                {
                    alive++;
                    if (current.lives == 1)
                    {
                        lastLife = true;
                    }
                }
            }
            if (alive == 2)
            {
                //only two players remain
                if (lastLife)
                {
                    speedyMusic = true;
                }
            }
            if (alive <= 1)
            {
                //game is over
                gameOver = true;
                speedyMusic = false;
                controlsDisabled = false;
            }
        }
    }
    public void endOfGame(GameObject player)
    {
        if (gameEndDelay > 0)
        {
            once = true;
            statusText.text = "Game!";
            gameEndDelay -= Time.deltaTime;
            Debug.Log(gameEndDelay + " " + once.ToString());
        }
        else
        {
            if (once)
            {
                int num = player.GetComponent<PlayerController>().playerNum - 1;
                string color = playerColorWords[num];
                statusText.text = color + " wins!";
                controlsDisabled = true;
                quitButton.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
                lastWinner = num + 1;
                restartButton.Select();
                once = false;
            }
        }
    }
}
