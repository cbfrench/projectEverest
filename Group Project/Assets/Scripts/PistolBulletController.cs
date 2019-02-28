using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBulletController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnBecameInvisible(){
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //do damage
        }
        else if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
