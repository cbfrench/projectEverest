using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedlegunController : MonoBehaviour, WeaponScript
{
    /* Author: ?
     * Description:
     * Contributor: Connor French
     */

    public Text label;          // Reference to the text label
    public GameObject needlePrefab;
    public float fireDelay;
    public float velocity;
    public int needlesToFire = 4;
    //public Transform firePoint; needed?
    private bool firing = false;
    private float delayTimer = 0;
    private GameObject player;          // Stores reference to the player
    //private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize 
        player = null;

        // Set label text
        label.text = "Needle Gun";
        label.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(firing){
            StartCoroutine(shootSpray());
            delayTimer = fireDelay;
            firing = false;
        }

        if(delayTimer > 0 ){
            delayTimer -= Time.deltaTime;
        }
    }

    // Called when a player picks up the weapon
    public void initWeaponUnique(GameObject player)
    {
        // Set the player reference
        this.player = player;
        //this.gameObject.transform.localPosition = new Vector3(-0.4f, -0.3f, 0);
        label.gameObject.SetActive(false);
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        // Set the player reference back to null on drop
        this.player = null;
        label.gameObject.SetActive(true);
        firing = false;
        delayTimer = 0;
        StopCoroutine(shootSpray());
        //this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void shoot()
    {
        if(!firing && delayTimer <= 0){
            firing = true;
        }
    }

    private IEnumerator shootSpray(){
        for(int i = 0; i < needlesToFire ; i++){
            GameObject projectile = Instantiate(needlePrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), this.transform.rotation);
            projectile.GetComponent<NeedleBulletController>().player = player;
            projectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(velocity * player.transform.localScale.x, Random.Range(-220.0f, 220.0f)));
            yield return new WaitForSeconds(.14f);
        }
    }

    public void stop()
    {
        
    }
}
