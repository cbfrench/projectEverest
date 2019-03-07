using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    public float damage = 50f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "player")
        {
            // Damage player here
            collision.GetComponent<PlayerController>().receiveDamage(damage);
        }
    }
}
