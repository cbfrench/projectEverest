using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public float stickSensitivity;
    public float mouseSensitivity;

    private Button currentSelection = null;
    private Slider currentSlider = null;
    private float currentSliderValue = 0;
    private bool dragging = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = 0, v = 0;
        float hcount = 0, vcount = 0;
        float v1 = Input.GetAxis("Vertical_P1") * stickSensitivity;
        float v2 = Input.GetAxis("Vertical_P2") * stickSensitivity;
        float v3 = Input.GetAxis("Vertical_P3") * stickSensitivity;
        float v4 = Input.GetAxis("Vertical_P4") * stickSensitivity;
        float h1 = Input.GetAxis("Horizontal_P1") * stickSensitivity;
        float h2 = Input.GetAxis("Horizontal_P2") * stickSensitivity;
        float h3 = Input.GetAxis("Horizontal_P3") * stickSensitivity;
        float h4 = Input.GetAxis("Horizontal_P4") * stickSensitivity;
        float mh = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mv = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float[] horizontals = { h1, h2, h3, h4, mh };
        float[] verticals = { v1, v2, v3, v4, mv };
        int i;
        for(i = 0; i < horizontals.Length; i++)
        {
            if(horizontals[i] == 0)
            {
                continue;
            }
            hcount++;
            h += horizontals[i];
        }
        for(i = 0; i < verticals.Length; i++)
        {
            if(verticals[i] == 0)
            {
                continue;
            }
            vcount++;
            v += verticals[i];
        }
        if (hcount != 0)
        {
            h = h / hcount;
        }
        else
        {
            h = 0;
        }
        if (vcount != 0)
        {
            v = v / vcount;
        }
        else
        {
            v = 0;
        }
        transform.position += new Vector3(h, v, 0);
        transform.position = new Vector3(constrainWidth(transform.position.x), constrainHeight(transform.position.y), 0);
        if (Input.GetButtonDown("Submit"))
        {
            if(currentSelection != null)
            {
                currentSelection.onClick.Invoke();
            }
        }
        if (Input.GetButtonDown("Cancel"))
        {
            try
            {
                GameObject.Find("Back").transform.GetComponent<Button>().onClick.Invoke();
            }
            catch(System.Exception e)
            {

            }
        }
        if (Input.GetButton("Submit"))
        {
            if (currentSlider != null)
            {
                dragging = true;
                float val = 0;
                if(h > 0)
                {
                    val = 1.0f / 5;
                }
                if(h < 0)
                {
                    val = -1.0f / 5;
                }
                currentSliderValue += val;
                currentSlider.value = currentSliderValue;
                if (currentSlider.gameObject.name.Contains("Music"))
                {
                    Admin.musicVolume = (currentSlider.value - 1) * 1.0f / 25;
                    Music.instance.music.volume = Admin.musicVolume;
                }
                if (currentSlider.gameObject.name.Contains("Sound"))
                {
                    Admin.soundVolume = (currentSlider.value - 1) * 1.0f / 25;
                }
            }
        }
        if (Input.GetButtonUp("Submit"))
        {
            dragging = false;
            currentSlider = null;
        }
    }

    float constrainWidth(float input)
    {
        float width = Screen.width;
        if(input > width)
        {
            return width;
        }
        if(input < 0)
        {
            return 0;
        }
        return input;
    }
    float constrainHeight(float input)
    {
        float height = Screen.height;
        if (input > height)
        {
            return height;
        }
        if (input < 0)
        {
            return 0;
        }
        return input;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        try
        {
            Button b = GameObject.Find(collision.gameObject.name).transform.GetComponent<Button>();
            b.Select();
            currentSelection = b;
        }
        catch (System.Exception e)
        {

        }
        try
        {
            Slider s = collision.GetComponent<Slider>();
            currentSlider = s;
            currentSliderValue = s.value;
        }
        catch (System.Exception e)
        {

        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        try
        {
            Button b = GameObject.Find(collision.gameObject.name).transform.GetComponent<Button>();
            b.Select();
            currentSelection = b;
        }
        catch (System.Exception e)
        {

        }
        try
        {
            Slider s = collision.GetComponent<Slider>();
            currentSlider = s;
            currentSliderValue = s.value;
        }
        catch (System.Exception e)
        {

        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        currentSelection = null;
        if (!dragging)
        {
            currentSlider = null;
        }
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }
}
