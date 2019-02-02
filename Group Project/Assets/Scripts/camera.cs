using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    private void LateUpdate()
    {
        if (GameControl.instance.climbing)
        {
            if (GameControl.instance.reachedTop)
            {
                GameControl.instance.DestroyPlats();
            }
        }
    }
}
