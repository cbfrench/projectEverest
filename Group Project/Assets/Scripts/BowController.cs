using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowController : MonoBehaviour, WeaponScript
{
    /* Author: Reynaldo Hermawan
     * Description: class used to control the behavior of the Bow weapon
     * Contributors: Connor French
     */

    public Text label;          // Reference to the text label
    public float maxDrawtime;
    public GameObject arrowPrefab;
    public float minVelocity;
    public float maxVelocity;
    //public Transform firePoint; needed?

    private GameObject player;
    private float drawTime;
    private bool bowDraw;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize 
        anim = gameObject.transform.Find("Sprite").GetComponent<Animator>();

        // Set label text
        label.text = "Bow";
        label.gameObject.SetActive(true);

        //values determined through testing
        Vector2 vels = new Vector2(0, 111.1f);
        arrowPrefab.GetComponent<BowArrowController>().minmax = vels;
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
                anim.SetTrigger("FullDraw");
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
        arrowPrefab.GetComponent<BowArrowController>().player = player;
        this.gameObject.transform.localPosition = new Vector3(-0.4f, -0.3f, 0);
        label.gameObject.SetActive(false);
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        // Set the player reference back to null on drop
        label.gameObject.SetActive(true);
        this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        anim.SetBool("Drawing", false);
    }

    public void shoot()
    {
        bowDraw = true;
        //player.playerSpeed = playerSpeed / 2; maybe?
    }

    public void stop()
    {
        //Scaling value to new range
        var arrowVelocity = Mathf.Lerp(minVelocity, maxVelocity, Mathf.InverseLerp (0f, maxDrawtime, drawTime));
        bowDraw = false;
        drawTime = 0;
        anim.ResetTrigger("FullDraw");

        GameObject arrow = Instantiate(arrowPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), this.transform.rotation);

        arrow.GetComponent<Rigidbody2D>().AddForce(new Vector2(arrowVelocity * player.transform.localScale.x, 50));
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

    GameObject projectile = this.GetInactiveProjectile();

    if (projectile != null) {
        projectile.transform.position = firePoint.transform.position;
        projectile.transform.localRotation = firePoint.transform.parent.transform.localRotation;
        projectile.SetActive(true);
    }

    public GameObject GetInactiveProjectile() {
        for (int i = 0; i < pooledProjectiles.Count; i++) {
            if (!pooledProjectiles[i].activeInHierarchy) {
                return pooledProjectiles[i];
            }
        }  
        return null;
    }
*/