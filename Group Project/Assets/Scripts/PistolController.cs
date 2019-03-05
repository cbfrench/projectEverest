using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PistolController : MonoBehaviour, WeaponScript
{

    public Text label;          // Reference to the text label
    public GameObject bulletPrefab;
    public float fireDelay;
    public float velocity;
    //public Transform firePoint; needed?
    private float delayTimer = 0;
    private GameObject player;          // Stores reference to the player
    //private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize 
        player = null;
        //anim = gameObject.transform.Find("Sprite").GetComponent<Animator>();

        // Set label text
        label.text = "Pistol";
        label.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(delayTimer > 0 ){
            delayTimer -= Time.deltaTime;
        }
    }

    // Called when a player picks up the weapon
    public void initWeaponUnique(GameObject player)
    {
        // Set the player reference
        this.player = player;
        bulletPrefab.GetComponent<PistolBulletController>().player = player;
        //this.gameObject.transform.localPosition = new Vector3(-0.4f, -0.3f, 0);
        label.gameObject.SetActive(false);
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        // Set the player reference back to null on drop
        this.player = null;
        label.gameObject.SetActive(true);
        //this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void shoot()
    {
        if(delayTimer <= 0){
            GameObject bullet = Instantiate(bulletPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), this.transform.rotation);
            bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(velocity * player.transform.localScale.x, 0));
            delayTimer = fireDelay;
        }
    }

    public void stop()
    {
        
    }
}
