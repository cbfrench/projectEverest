using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowArrowController : MonoBehaviour
{

    //private TrailRenderer tr = null;
    // Start is called before the first frame update
    void Start()
    {
        //tr = gameObject.transform.Find("Trail").GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnBecameInvisible(){
        gameObject.SetActive(false);
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
