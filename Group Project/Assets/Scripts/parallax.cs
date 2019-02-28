using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    public GameObject cam;
    public float speed;

    private float offset;
    private float startingX;
    private float startingY;
    private float startingZ;

    private void Start()
    {
        offset = transform.position.y - cam.transform.position.y;
        startingX = transform.position.x;
        startingY = transform.position.y;
        startingZ = 0;
    }

    private void LateUpdate()
    {
        float xScale = 1.5f;
        if(speed == 1)
        {
            xScale = 1;
        }
        transform.position = new Vector3(cam.transform.position.x * speed * xScale + startingX, cam.transform.position.y * speed + startingY, startingZ);
    }
}
