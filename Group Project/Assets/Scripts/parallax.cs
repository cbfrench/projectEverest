using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    public GameObject cam;
    public float speed;

    private float offset;
    private float startingY;
    private float startingZ;

    private void Start()
    {
        offset = transform.position.y - cam.transform.position.y;
        startingY = transform.position.y;
        startingZ = 0;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, cam.transform.position.y * speed, startingZ);
        if (transform.position.y < startingY)
        {
            transform.position = new Vector3(transform.position.x, startingY, startingZ);
        }
    }
}
