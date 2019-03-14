using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleBulletController : MonoBehaviour
{
    /* Author: ?
     * Description:
     * Contributor: Connor French
     */
    public GameObject player;
    public float bulletDamage = 15f;
    public float homeSpeed;

    private GameObject otherPlayer;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i] != player)
            {
                otherPlayer = players[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float prevX = transform.position.x;
        transform.position = Vector3.MoveTowards(transform.position, otherPlayer.transform.position, homeSpeed * Time.deltaTime);
        transform.position = new Vector3(prevX, transform.position.y, transform.position.z);
    }
    
    void OnBecameInvisible(){
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != player && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().receiveDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
