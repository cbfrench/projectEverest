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

    private Button playButton;
    private Button backButton;
    private Button tutorialButton;

    public void Awake()
    {
        /* Author: Connor French
         * Description: sets play button to active at the opening of the game
         */
        playButton = GameObject.Find("Play").transform.GetComponent<Button>();
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

    public void levelSelect()
    {
        /* Author: Connor French
         * Description: pulls up level select screen when selected from the main menu
         */
        main.gameObject.SetActive(false);
        select.gameObject.SetActive(true);
        tutorialButton = GameObject.Find("Tutorial").transform.GetComponent<Button>();
        tutorialButton.Select();
    }

    public void back()
    {
        /* Author: Connor French
         * Description: goes back to the main menu when selected from either the level select or the controls screens
         */
        main.gameObject.SetActive(true);
        controls.gameObject.SetActive(false);
        select.gameObject.SetActive(false);
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
