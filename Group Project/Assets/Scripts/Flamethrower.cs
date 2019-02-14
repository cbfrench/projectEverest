using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flamethrower : MonoBehaviour, WeaponScript
{

    public GameObject player;   // Stores reference to the player
    public Text label;          // Reference to the text label

    private ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {

        // Initialize 
        player = null;

        // Set label text
        label.text = "Flamethrower";
        label.gameObject.SetActive(true);

        // Get the particle system for the flame
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
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
        particles.Stop();
    }

    // Shoot flame
    public void shoot()
    {
        particles.Play();
    }

    // Stop shooting flame
    public void stop()
    {
        particles.Stop();
    }
}
