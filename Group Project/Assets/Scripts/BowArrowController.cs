﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowArrowController : MonoBehaviour
{
    /* Author: Reynaldo Hermawan
     * Description: Class controlling the behavior of the arrows fired from the 
     * bow weapon.
     */
    public GameObject player;
    public float maxVelocity;
    //private TrailRenderer tr = null;
    public AudioSource audioSource;
    public AudioClip fireArrow;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        fireArrow = Resources.Load<AudioClip>("Bow");
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource.PlayOneShot(fireArrow);
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
            var damage = Mathf.Lerp(0f, 100f, Mathf.InverseLerp (0f, this.maxVelocity, gameObject.GetComponent<Rigidbody2D>().velocity.x));
            collision.gameObject.GetComponent<PlayerController>().receiveDamage(damage);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
