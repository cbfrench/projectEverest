using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour, WeaponScript
{

    public GameObject bulb; // Reference to bulb of flashlight

    // Start is called before the first frame update
    void Start()
    {
        bulb.SetActive(false);
    }

    // Called to shoot weapon
    public void Shoot()
    {
        // Toggle bulb
        bulb.SetActive(!bulb.activeSelf);
    }

    public void Stop()
    {
        // Nothing needed here
    }
}
