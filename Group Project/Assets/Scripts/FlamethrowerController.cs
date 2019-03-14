using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlamethrowerController : MonoBehaviour, WeaponScript
{
    /* Author: Caleb Biggers
     * Description: Controller for flamethrower
     * Contributor: Connor French
     */
    public Text label;          // Reference to the text label
    public ParticleSystem particles;   // Refernce to the particle system

    private GameObject player;          // Stores reference to the player
    private AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        /* Author: Caleb Biggers
         * Description:
         * Contributor: Connor French
         */

        // Initialize 
        //player = null;

        // Set label text
        label.text = "Flamethrower";
        label.gameObject.SetActive(true);

        // Get the particle system for the flame
        particles.Stop();
        sound = particles.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Called when a player picks up the weapon
    public void initWeaponUnique(GameObject player)
    {
        // Set the player reference
        particles.GetComponent<FlamethrowerParticleController>().player = player;
        this.player = player;
        label.gameObject.SetActive(false);
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        /* Author: Caleb Biggers
         * Description:
         * Contributor: Connor French
         */
        // Set the player reference back to null on drop
        this.player = null;
        label.gameObject.SetActive(true);
        particles.Stop();
        sound.Stop();
    }

    // Shoot flame
    public void shoot()
    {
        /* Author: Caleb Biggers
         * Description:
         * Contributor: Connor French
         */
        if (!GameControl.instance.paused)
        {
            particles.Play();
            sound.Play();
        }
    }

    // Stop shooting flame
    public void stop()
    {
        /* Author: Caleb Biggers
         * Description:
         * Contributor: Connor French
         */
        particles.Stop();
        sound.Stop();
    }
}
