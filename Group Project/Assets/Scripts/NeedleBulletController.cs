using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleBulletController : MonoBehaviour
{
    public GameObject player;
    public float bulletDamage = 15f;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != player && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().receiveDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
