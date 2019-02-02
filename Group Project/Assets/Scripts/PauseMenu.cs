using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button quit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        SceneManager.LoadScene("Main_Menu");
    }

    public void playAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
