using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    public Canvas main;
    public Canvas controls;
    public Canvas select;

    private Button playButton;
    private Button backButton;
    private Button firstLevelButton;

    public void Awake()
    {
        playButton = GameObject.Find("Play").transform.GetComponent<Button>();
    }

    public void loadLevel()
    {
    }

    public void loadCave()
    {
        SceneManager.LoadScene("Prototype_Level");
    }

    public void loadMountain()
    {
        SceneManager.LoadScene("Curve_Level");
    }

    public void instructions()
    {
        main.gameObject.SetActive(false);
        controls.gameObject.SetActive(true);
        backButton = GameObject.Find("Back").transform.GetComponent<Button>();
        backButton.Select();
    }

    public void levelSelect()
    {
        main.gameObject.SetActive(false);
        select.gameObject.SetActive(true);
        firstLevelButton = GameObject.Find("Cave").transform.GetComponent<Button>();
        firstLevelButton.Select();
    }

    public void back()
    {
        main.gameObject.SetActive(true);
        controls.gameObject.SetActive(false);
        select.gameObject.SetActive(false);
        playButton.Select();
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
