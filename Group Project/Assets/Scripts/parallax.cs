using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    /* Author: Connor French
     * Description: class for moving something at a relative speed to the camera. Used for moving the background slowly to give it a parallax depth effect
     */
    public GameObject cam;
    public float speed;

    private float offset;
    private float startingX;
    private float startingY;
    private float startingZ;

    private void Start()
    {
        /* Author: Connor French
         * Description: sets initial position for the object
         */
        offset = transform.position.y - cam.transform.position.y;
        startingX = transform.position.x;
        startingY = transform.position.y;
        startingZ = 0;
    }

    private void LateUpdate()
    {
        /* Author: Connor French
         * Description: moves the object at a slightly different than the camera, giving the illusion of depth
         */
        float xScale = 1.5f;
        if(speed == 1)
        {
            xScale = 1;
        }
        transform.position = new Vector3(cam.transform.position.x * speed * xScale + startingX, cam.transform.position.y * speed + startingY, startingZ);
    }
}
