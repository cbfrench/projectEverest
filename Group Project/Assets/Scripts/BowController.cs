﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowController : MonoBehaviour, WeaponScript
{

    public Text label;          // Reference to the text label
    public float maxDrawtime;
    public GameObject arrowPrefab;
    public float maxVelocity;
    //public Transform firePoint; needed?

    private float drawTime;
    private bool bowDraw;
    private GameObject player;          // Stores reference to the player
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize 
        player = null;
        anim = gameObject.transform.Find("Sprite").GetComponent<Animator>();

        // Set label text
        label.text = "Bow";
        label.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (bowDraw)
        {
            if(drawTime < maxDrawtime){
                drawTime += Time.deltaTime;
            }else{
                drawTime = maxDrawtime;
            }
        }
        anim.SetBool("Drawing", bowDraw);
    }

    // Called when a player picks up the weapon
    public void initWeaponUnique(GameObject player)
    {
        drawTime = 0f;
        bowDraw = false;
        // Set the player reference
        this.player = player;
        label.gameObject.SetActive(false);
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        // Set the player reference back to null on drop
        this.player = null;
        label.gameObject.SetActive(true);
    }

    public void shoot()
    {
        bowDraw = true;
        //player.playerSpeed = playerSpeed / 2; maybe?
    }

    public void stop()
    {
        //Scaling value to new range
        var arrowVelocity = Mathf.Lerp(10f, maxVelocity, Mathf.InverseLerp (0f, maxDrawtime, drawTime));
        Debug.Log(arrowVelocity);
        Debug.Log(drawTime);
        bowDraw = false;
        drawTime = 0;

        GameObject arrow = Instantiate(arrowPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), this.transform.rotation);

        arrow.GetComponent<Rigidbody2D>().AddForce(new Vector2(arrowVelocity, 50));
    }
}


/*
   public List<GameObject> pooledProjectiles = new List<GameObject>();
   public GameObject projectilePrefab;
   public int poolAmt;

   void Start(){
      for (int i = 0; i < poolAmt; i++) {
        GameObject obj = (GameObject)Instantiate(projectilePrefab);
        obj.SetActive(false); 
        pooledProjectiles.Add(obj);
      }
   

    Vector3 dir = rigidbody.velocity;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
*/