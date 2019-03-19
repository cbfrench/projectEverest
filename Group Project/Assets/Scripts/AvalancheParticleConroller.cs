using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvalancheParticleConroller : MonoBehaviour
{
    /* Author: Reynaldo Hermawan
     * Description: Class that controls the damage the avalanche deals to a player
     */

    public GameObject player;
    public float avalancheDamage = 50f;

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
        //Checks if player is a player and deals damage to them on particle hit.
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().receiveDamage(this.avalancheDamage * Time.deltaTime);
        }
    }
}