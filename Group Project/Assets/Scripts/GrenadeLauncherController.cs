using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeLauncherController : MonoBehaviour, WeaponScript
{

    /* Author: Caleb Biggers
    * Description: Controller for grenade launcher weapon
    * Contributor: Connor French
    */

    public float chargeTime = .5f;
    public float launchForce = 2000f;
    public Text label;          // Reference to the text label
    public GameObject grenadePrefab;
    public Transform grenadeSpawn;
    public Slider slider;
    public float fireRate = 1f;

    private GameObject player;          // Stores reference to the player
    private bool charging;
    private float charge;
    private float fireDelta;
    private bool fired;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize 
        player = null;

        // Set label text
        label.text = "Grenade Launcher";
        label.gameObject.SetActive(true);

        // Set charging
        charging = false;

        // Set slider
        slider.value = 0;

        fired = false;
        fireDelta = 0;
    }

    // Update is called once per frame
    void Update()
    {

        // Update the slider
        slider.value = charge / chargeTime;

        if (charging)
        {
            charge += Time.deltaTime;
            if(charge >= chargeTime)
            {
                charge = chargeTime;
            }
        }

        if (fired)
        {
            fireDelta += Time.deltaTime;
        }
    }

    // Called when a player picks up the weapon
    public void initWeaponUnique(GameObject player)
    {
        /* Author: Caleb Biggers
         * Description: 
         * Contributor: Connor French
         */
        // Set the player reference
        this.player = player;
        label.gameObject.SetActive(false);
        charging = false;
        fired = false;
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        // Set the player reference back to null on drop
        this.player = null;
        label.gameObject.SetActive(true);
    }

    // Press to charge
    public void shoot()
    {
        if(fireDelta >= fireRate || fireDelta == 0)
        {
            fireDelta = 0;
            charging = true;
            fired = false;
            fireDelta = 0;
        }
    }

    // Release to fire
    public void stop()
    {
        if (charging)
        {
            // Reset charging
            charging = false;
            fired = true;

            // Spawn grenade
            GameObject grenade = Instantiate(grenadePrefab, new Vector3(grenadeSpawn.position.x, grenadeSpawn.position.y, grenadeSpawn.position.z), Quaternion.Euler(0, 0, 0));
            grenade.GetComponent<GrenadeController>().player = player;

            float force = launchForce * (charge / chargeTime);
            if (force <= 50)
            {
                force = 50;
            }

            grenade.GetComponent<Rigidbody2D>().AddForce(new Vector2(player.transform.localScale.x * force, 0));

            charge = 0;
        }
    }
}
