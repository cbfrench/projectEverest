using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    /* Author: Connor French
     * Description: functionality for the pause menu
     */
    public Button quit;
    public AudioClip menuMusic;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* Author: Connor French
         * Description: checks if the player tries to pause the game when it is acceptable to pause the game
         */
        if (!GameControl.instance.gameOver && GameControl.instance.climbing)
        {
            if (GameControl.instance.paused)
            {
                quit.gameObject.SetActive(true);
                quit.Select();
            }
            else
            {
                quit.gameObject.SetActive(false);
            }
        }
    }

    public void returnToMenu()
    {
        /* Author: Connor French
         * Description: returns to the main menu from the pause menu
         */
        GameControl.instance.speedyMusic = false;
        Music.instance.music.clip = menuMusic;
        Music.instance.music.volume = 1;
        Music.instance.music.pitch = 1;
        Music.instance.music.Play();
        Admin.sceneToLoad = "Main_Menu";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void playAgain()
    {
        /* Author: Connor French
         * Description: reloads current scene from beginning
         */
        Admin.sceneToLoad = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("LoadingScreen");
    }
}
