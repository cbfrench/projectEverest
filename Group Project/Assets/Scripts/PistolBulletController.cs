using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBulletController : MonoBehaviour
{
    /* Author: Reynaldo Hermawan
     * Description: Class controlling the behavior of the bullets fired from the Pistol weapon.
     */
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
        /* Author: Reynaldo Hermawan
         * Description: Damages player on contact based on speed, destroys arrow on contact with anything
         */
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
