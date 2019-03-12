using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    /* Author: Connor French
     * Description: script that is attached to a GameObject that, upon activation, shakes the camera a variable amount
     */

    public Transform camTransform;
    public float initialShakeDuration;
    public float shakeDuration;
    public float shakeAmount;
    public float decreaseFactor;

    Vector2 originalPos;

    void OnEnable()
    {
        /* Author: Connor French
         * Description: stores initial position of the gameobject when it is enabled
         */
        originalPos = camTransform.localPosition;
        shakeDuration = initialShakeDuration;
    }

    void Update()
    {
        /* Author: Connor French
         * Description: shakes the gameobject for a specified time, then resets its position to the initial position and becomes inactive
         */
        if (shakeDuration > 0 && !GameControl.instance.paused)
        {
            camTransform.localPosition = originalPos + Random.insideUnitCircle * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            gameObject.SetActive(false);
            camTransform.localPosition = originalPos;
        }
    }
}
