using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    /* Author: Caleb Biggers
    * Description: Controls the beam for the rail gun
    */

    public float damage = 50f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // Damage player here
            collision.GetComponent<PlayerController>().receiveDamage(damage);
        }
    }
}
