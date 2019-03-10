using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerParticleController : MonoBehaviour
{
    public GameObject player;
    public float flamethrowerDamage = 16.667f;

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
        if (other != player && other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().receiveDamage(this.flamethrowerDamage * Time.deltaTime);
        }
    }
}
