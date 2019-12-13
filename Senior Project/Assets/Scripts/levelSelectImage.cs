using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class levelSelectImage : MonoBehaviour
{
    /* Author: Connor French
     * Description: class for changing the preview image on the level select screen
     */
    public Button tutorialButton;
    public Button caveButton;
    public Button mountainButton;
    public Button volcanoButton;
    public Button waterfallButton;
    public Button nuclearButton;
    public Button beachButton;
    public Button cityButton;
    public Button treeButton;
    public Button moonButton;

    public Image levelSelectImg;
    public Sprite tutorialImage;
    public Sprite caveImage;
    public Sprite mountainImage;
    public Sprite volcanoImage;
    public Sprite waterfallImage;
    public Sprite nuclearImage;
    public Sprite beachImage;
    public Sprite cityImage;
    public Sprite treeImage;
    public Sprite moonImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* Author: Connor French
         * Description: if the level select menu is active, sets the image displayed to the currently selected button's associated image
         */
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
            if(selected == waterfallButton.gameObject)
            {
                levelSelectImg.sprite = waterfallImage;
                Debug.Log("Waterfall");
            }
            if (selected == nuclearButton.gameObject)
            {
                levelSelectImg.sprite = nuclearImage;
                Debug.Log("Reactor");
            }
            if (selected == cityButton.gameObject)
            {
                levelSelectImg.sprite = cityImage;
                Debug.Log("City");
            }
            if (selected == beachButton.gameObject)
            {
                levelSelectImg.sprite = beachImage;
                Debug.Log("Beach");
            }
            if (selected == treeButton.gameObject)
            {
                levelSelectImg.sprite = treeImage;
                Debug.Log("Tree");
            }
            if (selected == moonButton.gameObject)
            {
                levelSelectImg.sprite = moonImage;
                Debug.Log("Moon");
            }
        }
    }
}
