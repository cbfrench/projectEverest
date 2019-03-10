using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Textbox : MonoBehaviour
{
    /* Author: Connor French
     * Description: class for creating a textbox and populating it with text over time
     */
    public static Textbox instance;
    public Canvas box;
    public Text text;
    public string content;
    public float killTime = 3;

    private GameObject top;
    private float update = 0;
    private float textSpeed = 0.025f;
    private int ind = 0;
    private bool finished = false;
    private GameObject background1;
    private GameObject background2;
    private Text t;
    private Color b1color;
    private Color b2color;
    private Color tColor;
    private bool fadingIn = true;
    private float fadeInTimer;

    private void Awake()
    {
        /*if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        GameControl.instance.textboxDestroyed = false;
        background1 = transform.GetChild(0).gameObject;
        background2 = transform.GetChild(1).gameObject;
        t = transform.GetChild(2).gameObject.GetComponent<Text>();
        b1color = background1.GetComponent<Image>().color;
        b2color = background2.GetComponent<Image>().color;
        tColor = t.color;
        fadeInTimer = killTime;
        Color trans1 = new Color(b1color.r, b1color.g, b1color.b, 0);
        Color trans2 = new Color(b2color.r, b2color.g, b2color.b, 0);
        background1.GetComponent<Image>().color = trans1;
        background2.GetComponent<Image>().color = trans2;
        GameControl.instance.cameraSpeed = 0;
        top = GameObject.FindGameObjectsWithTag("Killbox")[2];
    }

    // Update is called once per frame
    void Update()
    {
        fadeIn();
        updateText();
        kill();
    }

    public void newContent(string input)
    {
        Debug.Log(Time.time);
        text.text = "";
        content = input;
        update = 0;
        ind = 0;
    }

    public void updateText()
    {
        if (fadingIn)
        {
            return;
        }
        if (content != text.text)
        {
            update += Time.deltaTime;
            if (update >= textSpeed)
            {
                text.text = text.text + content[ind];
                update = 0;
                ind++;
            }
        }
        else
        {
            finished = true;
        }
    }

    public void kill()
    {
        if (finished)
        {
            if (killTime <= 0)
            {
                Destroy(gameObject);
                GameControl.instance.textboxDestroyed = true;
                GameControl.instance.cameraSpeed = GameControl.instance.initialCameraSpeed;
                top.GetComponent<Collider2D>().isTrigger = true;
            }
            fadeOut(killTime);
            killTime -= Time.deltaTime;
        }
    }
    public void fadeIn()
    {
        if(fadeInTimer <= 0)
        {
            fadingIn = false;
            return;
        }
        Color transparent = new Color(0, 0, 0, 0);
        background1.GetComponent<Image>().color = Color.Lerp(b1color, transparent, fadeInTimer / 3);
        background2.GetComponent<Image>().color = Color.Lerp(b2color, transparent, fadeInTimer / 3);
        t.GetComponent<Text>().color = Color.Lerp(tColor, transparent, fadeInTimer / 3);
        fadeInTimer -= Time.deltaTime * 6;
        top.GetComponent<Collider2D>().isTrigger = false;
    }
    public void fadeOut(float timer)
    {
        Color transparent = new Color(0, 0, 0, 0);
        background1.GetComponent<Image>().color = Color.Lerp(transparent, b1color, timer / 0.5f);
        background2.GetComponent<Image>().color = Color.Lerp(transparent, b2color, timer / 0.5f);
        t.GetComponent<Text>().color = Color.Lerp(transparent, tColor, timer / 0.5f);
    }
}
