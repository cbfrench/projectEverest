using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    /* Author: Connor French
     * Description: class for removing all platforms once the top of the stage is reached, should probably be removed
     */
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
