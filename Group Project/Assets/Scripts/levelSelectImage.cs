using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class levelSelectImage : MonoBehaviour
{
    public Button tutorialButton;
    public Button caveButton;
    public Button mountainButton;
    public Button volcanoButton;
    public Image levelSelectImg;
    public Sprite tutorialImage;
    public Sprite caveImage;
    public Sprite mountainImage;
    public Sprite volcanoImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected == tutorialButton.gameObject)
            {
                levelSelectImg.sprite = tutorialImage;
                Debug.Log("Tutorial");
            }
            if (selected == caveButton.gameObject)
            {
                levelSelectImg.sprite = caveImage;
                Debug.Log("Cave");
            }
            if (selected == mountainButton.gameObject)
            {
                levelSelectImg.sprite = mountainImage;
                Debug.Log("Mountain");
            }
            if (selected == volcanoButton.gameObject)
            {
                levelSelectImg.sprite = volcanoImage;
                Debug.Log("Volcano");
            }
        }
    }
}
