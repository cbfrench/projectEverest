using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordController : MonoBehaviour, WeaponScript 
{
    /* Author: Reynaldo Hermawan
     * Description: Class controlling the behavior of the Sword weapon.
     * Contributor: Connor French - Audio
     */

    public Text label;

    public Transform attackPos;
    public LayerMask otherPlayers;

    public float attackRangeX;
    public float attackRangeY;
    public float attackAngle;

    public float attackDelay = 0.3f;
    public float damage = 22f;
    public float knockback = 7000f;

    private GameObject player; 
    private float delayTimer = 0;
    private Animator anim;
    private AudioSource sound;
    private float initialVolume;


    // Use this for initialization
    void Start () {
        anim = gameObject.transform.Find("Sprite").GetComponent<Animator>();
        sound = gameObject.GetComponent<AudioSource>();
        initialVolume = sound.volume;
    }
   
    // Update is called once per frame
    void Update () {
        sound.volume = Admin.soundVolume * initialVolume;
        if(delayTimer > 0 ){
            delayTimer -= Time.deltaTime;
        }

    }

    void OnDrawGizmosSelected(){ 
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, new Vector3(attackRangeX, attackRangeY, 0));
    }

    // Called when a player picks up the weapon
    public void initWeaponUnique(GameObject player)
    {
        // Set the player reference
        this.player = player;
        label.gameObject.SetActive(false);
        //this.gameObject.transform.localPosition = new Vector3(-0.4f, -0.3f, 0);
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        // Set the player reference back to null on drop
        this.player = null;
        label.gameObject.SetActive(true);
        //this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        delayTimer = 0;
        StopCoroutine(swingWeapon());
    }

    // Calls coroutine to swing sword when allowed
    public void shoot()
    {
        if(delayTimer <= 0){
            StartCoroutine(swingWeapon());
        }
    }

    private IEnumerator swingWeapon(){
        /* Author: Reynaldo Hermawan
         * Description: Checks if the sword hit anything and applies damage based on hit.
         */
        anim.SetTrigger("Swing");
        if (!GameControl.instance.paused)
        {
            sound.Play();
        }
        yield return new WaitForSeconds(.17f);
        Collider2D[] peopleHit = Physics2D.OverlapBoxAll(attackPos.position, new Vector2(attackRangeX, attackRangeY), 0, otherPlayers); //mask?
        for(int i = 0; i < peopleHit.Length; i++){
            if(peopleHit[i].gameObject != this.player){
                peopleHit[i].GetComponent<PlayerController>().receiveDamage(damage);
                peopleHit[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(knockback * player.transform.localScale.x, 700));
            }
        }
        delayTimer = attackDelay;
    }

    public void stop()
    {

    }
}
