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
    }

    public void loadLevel()
    {
    }

    public void loadTutorial()
    {
        /* Author: Connor French
         * Description: loads tutorial
         */
        SceneManager.LoadScene("Tutorial_Level");
    }

    public void loadCave()
    {
        /* Author: Connor French
         * Description: loads cave level
         */
        SceneManager.LoadScene("Prototype_Level");
    }

    public void loadMountain()
    {
        /* Author: Connor French
         * Description: loads mountain level
         */
        SceneManager.LoadScene("Curve_Level");
    }

    public void loadVolcano()
    {
        /* Author: Connor French
         * Description: loads volcano level
         */
        SceneManager.LoadScene("Hardcore_Level");
    }

    public void instructions()
    {
        /* Author: Connor French
         * Description: pulls up controls screen when selected from the main menu
         */
        main.gameObject.SetActive(false);
        controls.gameObject.SetActive(true);
        backButton = GameObject.Find("Back").transform.GetComponent<Button>();
        backButton.Select();
    }

    public void playerSelect()
    {
        for (int i = 0; i < crowns.Length; i++)
        {
            crowns[i].GetComponent<SpriteRenderer>().enabled = false;
        }
        main.gameObject.SetActive(false);
        players.gameObject.SetActive(true);
        twoPlayer = GameObject.Find("2-Player").transform.GetComponent<Button>();
        twoPlayer.Select();
    }

    public void selectNumberOfPlayers(int num)
    {
        PlayerNumberSelect.numberOfPlayers = num;
        levelSelect();
    }

    public void levelSelect()
    {
        /* Author: Connor French
         * Description: pulls up level select screen when selected from the main menu
         */
        players.gameObject.SetActive(false);
        select.gameObject.SetActive(true);
        tutorialButton = GameObject.Find("Tutorial").transform.GetComponent<Button>();
        tutorialButton.Select();
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
        controls.gameObject.SetActive(false);
        select.gameObject.SetActive(false);
        players.gameObject.SetActive(false);
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
