using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvalancheParticleConroller : MonoBehaviour
{
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
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().receiveDamage(this.avalancheDamage * Time.deltaTime);
        }
    }
}