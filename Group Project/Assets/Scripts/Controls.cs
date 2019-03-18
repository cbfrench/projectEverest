using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    /* Author: Connor French
     * Description: changes controls text based on controller used
     */
    private Text t;
    private bool sony = false;
    // Start is called before the first frame update
    void Start()
    {
        t = gameObject.GetComponent<Text>();
        string[] controllers = Input.GetJoystickNames();
        if (controllers.Length == 0)
        {
            return;
        }
        for (int i = 0; i < controllers.Length; i++)
        {
            if (!controllers[i].Contains("vJoy"))
            {
                sony = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sony)
        {
            t.text = "Move: Analog Stick\nJump: X\nEquip: Square\nFire: L2 / R2\nThrow: Circle";
        }
    }
}
