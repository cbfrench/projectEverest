using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowArrowController : MonoBehaviour
{
    /* Author: Reynaldo Hermawan
     * Description: class for controlling behavior of the arrows fired from a bow
     * Contributors: Connor French
     */
    public GameObject player;
    public Vector2 minmax;
    //private TrailRenderer tr = null;
    // Start is called before the first frame update
    void Start()
    {
        //tr = gameObject.transform.Find("Trail").GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = GetComponent<Rigidbody2D>().velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); 
    }

    void OnBecameInvisible(){
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != player && collision.gameObject.tag == "Player")
        {
            float velocity = gameObject.GetComponent<Rigidbody2D>().velocity.x;
            float damage = Mathf.Pow(((Mathf.Abs(velocity) - minmax.x) / minmax.y), 2) * 100f;
            collision.gameObject.GetComponent<PlayerController>().receiveDamage(damage);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
