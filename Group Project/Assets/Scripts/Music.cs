using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    /* Author: Connor French
     * Description: class for creating a music player that stays constant throughout scenes
     */
    public static Music instance;
    public AudioSource music;
    public float musicInc = 1;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}
