using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeLauncherController : MonoBehaviour, WeaponScript
{

    public float chargeTime = .5f;
    public float launchForce = 2000f;

    public Text label;          // Reference to the text label
    public GameObject grenadePrefab;
    public Transform grenadeSpawn;
    private GameObject player;          // Stores reference to the player
    private bool charging;
    private float charge;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (charging)
        {
            charge += Time.deltaTime;
            if(charge >= chargeTime)
            {
                charge = chargeTime;
            }
        }
    }

    // Called when a player picks up the weapon
    public void initWeaponUnique(GameObject player)
    {
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

    // Press to charge
    public void shoot()
    {
        charging = true;
    }

    // Release to fire
    public void stop()
    {
        // Reset charging
        charging = false;

        // Spawn grenade
        GameObject grenade = Instantiate(grenadePrefab, new Vector3(grenadeSpawn.position.x, grenadeSpawn.position.y, grenadeSpawn.position.z), transform.rotation);

        float force = launchForce * (charge / chargeTime);

        grenade.GetComponent<Rigidbody2D>().AddForce(new Vector2(force, 0));

        charge = 0;
    }
}
