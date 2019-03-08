using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    public AudioClip explosionSound;
    public float lifeSpan = 1f;
    public float explosionRadius = 2.5f;
    public float explosionForce = 300f;
    public int bounceLimit = 3;
    public GameObject explosion;
    public float damage = 30f;

    private ParticleSystem particles;
    private float lifeTime = 0;
    private int bounces;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for life span
        lifeTime += Time.deltaTime;
        if(lifeTime >= lifeSpan)
        {
            Detonate();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Detonate();
        }
        else if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "Wall")
        {
            bounces++;
            if(bounces >= bounceLimit)
            {
                Detonate();
            }
        }
    }

    public void Detonate()
    {
        // Play the explosion
        particles.Play();
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position);

        // Set velocity to zero
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // Get explosion hits
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), explosionRadius);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb2d = hit.GetComponent<Rigidbody2D>();

            if (rb2d != null)
            {
                rb2d.AddForce(new Vector2(hit.transform.position.x - transform.position.x, hit.transform.position.y - transform.position.y) * explosionForce);
            }

            //Debug.Log(hit.gameObject.tag);
            if (hit.gameObject.tag == "Player")
            {
                // Damage player here
                hit.GetComponent<PlayerController>().receiveDamage(damage);
            }
        }

        // Spawn indicator
        Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);

        // Destory the grenade
        Destroy(gameObject);
    }
}
