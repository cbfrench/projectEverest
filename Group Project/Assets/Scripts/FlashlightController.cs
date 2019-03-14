using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour, WeaponScript
{
    /* Author: Caleb Biggers
     * Description: Controller for flashlight
     */

    public GameObject bulb; // Reference to bulb of flashlight
    public Text label;

    // Start is called before the first frame update
    void Start()
    {
        // Turn off the bulb
        bulb.SetActive(false);

        // Turn on the label
        label.text = "Flashlight";
        label.gameObject.SetActive(true);
    }

    public void initWeaponUnique(GameObject player)
    {
        // Turn off the label
        label.gameObject.SetActive(false);
    }

    public void resetWeaponUnique(GameObject player)
    {
        /* Author:
         * Description:
         * Contributor: Connor French
         */
        // Turn on the label
        label.gameObject.SetActive(true);
        bulb.SetActive(false);
    }

    // Called to shoot weapon
    public void shoot()
    {
        // Toggle bulb
        bulb.SetActive(!bulb.activeSelf);
    }

    public void stop()
    {
        // Nothing needed here
    }
}
