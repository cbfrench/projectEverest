using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownFall : MonoBehaviour
{
    /* Author: Connor French
     * Description: creates a cool effect on the title screen
     */
    public float speed;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, -1 * Time.deltaTime * speed, 0));
        if(transform.position.y <= -1.5)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }
    }
}
