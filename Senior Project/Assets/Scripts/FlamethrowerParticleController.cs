using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerParticleController : MonoBehaviour
{
    /* Author: Reynaldo Hermawan
     * Description: Class that controls the damage the flamethrower deals to a player
     */

    public GameObject player;
    public float flamethrowerDamage = 33f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject other)
    {
        //Checks if player is a player and is not holding the flamethrower- deals damage to them on particle hit.
        if (other != player && other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().receiveDamage(this.flamethrowerDamage * Time.deltaTime);
        }
    }
}
