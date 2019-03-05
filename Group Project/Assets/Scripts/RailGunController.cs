using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailGunController : MonoBehaviour, WeaponScript
{

    public float chargeTime = 3f;
    public float fireTime = 1f;
    public float fireRate = 5f;
    public GameObject beam;

    public Text label;          // Reference to the text label
    private GameObject player;          // Stores reference to the player
    private bool charging;
    private bool firing;
    private bool fired;
    private float charge;
    private float fireDelta;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize 
        player = null;

        // Set label text
        label.text = "Rail Gun";
        label.gameObject.SetActive(true);

        // Set charging
        charging = false;
        firing = false;
        fired = false;

        fireDelta = 0;

        // disable beam
        beam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (fired)
        {
            fireDelta += Time.deltaTime;
            if(fireDelta >= fireRate)
            {
                fired = false;
                fireDelta = 0;
            }
        }

        if (charging)
        {
            charge += Time.deltaTime;
            if (charge >= chargeTime)
            {
                charging = false;
                fire();
            }
        }
        else if (firing)
        {
            charge += Time.deltaTime;
            if (charge >= fireTime)
            {
                beam.SetActive(false);
                firing = false;
                charge = 0;
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
        if (!fired)
        {
            fired = true;
            charging = true;
        }
    }

    // Release to fire
    public void stop()
    {
        // Reset charging
        charging = false;
        charge = 0;
    }

    public void fire()
    {
        // Reset charge
        charge = 0;

        // turn on beam
        beam.SetActive(true);

        // Set firing
        firing = true;
    }
}
