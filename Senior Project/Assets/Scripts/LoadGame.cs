using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    /* Author: Connor French
     * Description: class for running scene loading functions from the main menu as well as the pause menu
     */
    public Canvas main;
    public Canvas controls;
    public Canvas select;
    public Canvas players;
    public Canvas options;
    public GameObject cursor;

    private Button playButton;
    private Button backButton;
    private Button tutorialButton;
    private Button twoPlayer;
    private GameObject[] crowns;

    public void Awake()
    {
        /* Author: Connor French
         * Description: sets play button to active at the opening of the game
         */
        playButton = GameObject.Find("Play").transform.GetComponent<Button>();
    }

    public void Start()
    {
        /* Author: Connor French
         * Description: gets all crowns in menu scene
         */
        crowns = GameObject.FindGameObjectsWithTag("Crown");
        Cursor.visible = false;
    }

    public void loadLevel()
    {
    }

    public void loadTutorial()
    {
        /* Author: Connor French
         * Description: loads tutorial
         */
        Admin.sceneToLoad = "Tutorial_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void loadCave()
    {
        /* Author: Connor French
         * Description: loads cave level
         */
        Admin.sceneToLoad = "Prototype_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void loadMountain()
    {
        /* Author: Connor French
         * Description: loads mountain level
         */
        Admin.sceneToLoad = "Curve_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void loadVolcano()
    {
        /* Author: Connor French
         * Description: loads volcano level
         */
        Admin.sceneToLoad = "Hardcore_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void loadWaterfall()
    {
        /* Author: Connor French
         * Description: loads waterfall level
         */
        Admin.sceneToLoad = "Waterfall_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void loadReactor()
    {
        /* Author: Connor French
         * Description: loads reactor level
         */
        Admin.sceneToLoad = "Nuclear_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void loadBeachside()
    {
        /* Author: Connor French
         * Description: loads beach level
         */
        Admin.sceneToLoad = "Beach_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void loadSkyline()
    {
        /* Author: Connor French
         * Description: loads city level
         */
        Admin.sceneToLoad = "City_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void loadGreatTree()
    {
        /* Author: Connor French
         * Description: loads tree level
         */
        Admin.sceneToLoad = "Tree_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void loadMoon()
    {
        /* Author: Connor French
         * Description: loads moon level
         */
        Admin.sceneToLoad = "Moon_Level";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void instructions()
    {
        /* Author: Connor French
         * Description: pulls up controls screen when selected from the main menu
         */
        controls.gameObject.SetActive(true);
        cursor.transform.SetParent(controls.transform, false);     
        main.gameObject.SetActive(false); 
    }

    public void optionSelect()
    {
        options.gameObject.SetActive(true);
        cursor.transform.SetParent(options.transform, false);
        main.gameObject.SetActive(false);
    }

    public void playerSelect()
    {
        players.gameObject.SetActive(true);
        cursor.transform.SetParent(players.transform, false);
        for (int i = 0; i < crowns.Length; i++)
        {
            crowns[i].GetComponent<SpriteRenderer>().enabled = false;
        }
        main.gameObject.SetActive(false);
    }

    public void selectNumberOfPlayers(int num)
    {
        Admin.numberOfPlayers = num;
        levelSelect();
    }

    public void levelSelect()
    {
        /* Author: Connor French
         * Description: pulls up level select screen when selected from the main menu
         */
        select.gameObject.SetActive(true);
        cursor.transform.SetParent(select.transform, false);
        players.gameObject.SetActive(false);
    }

    public void back()
    {
        /* Author: Connor French
         * Description: goes back to the main menu when selected from either the level select or the controls screens
         */
        for (int i = 0; i < crowns.Length; i++)
        {
            crowns[i].GetComponent<SpriteRenderer>().enabled = true;
        }
        main.gameObject.SetActive(true);
        cursor.transform.SetParent(main.transform, false);
        controls.gameObject.SetActive(false);
        select.gameObject.SetActive(false);
        players.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        playButton.Select();
    }

    public void exitGame()
    {
        /* Author: Connor French
         * Description: quits game
         */
        Application.Quit();
    }
}
