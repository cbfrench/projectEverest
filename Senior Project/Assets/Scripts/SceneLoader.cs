using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Text t;
    private bool loadScene = true;
    private string scene;
    private float speed = 200f;

    private void Update()
    {
        t.color = new Color32(255, 255, 255, (byte)(255 - Mathf.Floor(Mathf.PingPong(Time.time * speed, 255))));
        if (loadScene)
        {
            loadScene = false;
            scene = Admin.sceneToLoad;
            StartCoroutine(loadNewScene());
        }
    }
    IEnumerator loadNewScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
