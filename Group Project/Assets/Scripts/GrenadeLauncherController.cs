using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeLauncherController : MonoBehaviour, WeaponScript
{
    public Text label;          // Reference to the text label
    public GameObject grenadePrefab;
    public Transform grenadeSpawn;
    private GameObject player;          // Stores reference to the player

    // Start is called before the first frame update
    void Start()
    {

        // Initialize 
        player = null;

        // Set label text
        label.text = "Grenade Launcher";
        label.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

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

    // Shoot flame
    public void shoot()
    {
        Instantiate(grenadePrefab, new Vector3(grenadeSpawn.position.x, grenadeSpawn.position.y, grenadeSpawn.position.z), transform.rotation);
    }

    // Stop shooting flame
    public void stop()
    {

    }
}
