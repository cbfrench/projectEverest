using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform camTransform;
    public float initialShakeDuration;
    public float shakeDuration;
    public float shakeAmount;
    public float decreaseFactor;

    Vector2 originalPos;

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
        shakeDuration = initialShakeDuration;
    }

    void Update()
    {
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
