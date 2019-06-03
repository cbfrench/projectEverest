using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowController : MonoBehaviour, WeaponScript
{
    /* Author: Reynaldo Hermawan
     * Description: Class controlling the behavior of the Bow weapon.
     * Contributor: Connor French & Caleb Biggers
     */

    public Text label;          // Reference to the text label
    public float maxDrawtime;
    public GameObject arrowPrefab;
    public float maxVelocity;
    public Slider slider;
    //public Transform firePoint; needed?

    private GameObject player;
    private float drawTime;
    private bool bowDraw;
    private Animator anim;
    private AudioSource sound;
    private float initialVolume;

    // Start is called before the first frame update
    void Start()
    {
        /* Author: Reynaldo Hermawan
         * Description: Sets initial variables
         */
        // Initialize 
        anim = gameObject.transform.Find("Sprite").GetComponent<Animator>();

        // Set label text
        label.text = "Bow";
        label.gameObject.SetActive(true);

        arrowPrefab.GetComponent<BowArrowController>().maxVelocity = maxVelocity;
        sound = gameObject.GetComponent<AudioSource>();
        initialVolume = sound.volume;
    }

    // Update is called once per frame
    void Update()
    {
        /* Author: Reynaldo Hermawan
         * Description: Sets the animation for drawing the bow as well as a timer to count how long the bow is being drawn
         * Contributor: Caleb Biggers
         */
        sound.volume = Admin.soundVolume * initialVolume;
        slider.value = (drawTime / maxDrawtime);

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
        /* Author: Reynaldo Hermawan
         * Description: Resets the weapon to a neutral picked-up state
         */
        drawTime = 0f;
        bowDraw = false;
        // Set the player reference
        this.player = player;
        this.gameObject.transform.localPosition = new Vector3(-0.4f, -0.3f, 0);
        label.gameObject.SetActive(false);
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        /* Author: Reynaldo Hermawan
         * Description: Resets the bow weapon appropriately upon being dropped
         * Contributor: Connor French
         */
        // Set the player reference back to null on drop
        label.gameObject.SetActive(true);
        this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        anim.SetBool("Drawing", false);
        bowDraw = false;
    }

    public void shoot()
    {
        //Checks button held
        bowDraw = true;
    }

    public void stop()
    {
        /* Author: Reynaldo Hermawan
         * Description: Scales velocity of arrow to how long fire button is held and fires it
         */
        //Scaling value to new ranges
        var arrowVelocity = Mathf.Lerp(10f, maxVelocity, Mathf.InverseLerp (0f, maxDrawtime, drawTime));
        bowDraw = false;
        drawTime = 0;
        anim.ResetTrigger("FullDraw");
        if (!GameControl.instance.paused)
        {
            sound.Play();
        }

        GameObject arrow = Instantiate(arrowPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), this.transform.rotation);
        arrow.GetComponent<BowArrowController>().player = player;
        arrow.GetComponent<Rigidbody2D>().velocity = new Vector3(arrowVelocity * player.transform.localScale.x, 2f, 0);
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