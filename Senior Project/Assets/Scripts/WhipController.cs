using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhipController : MonoBehaviour, WeaponScript 
{
    /* Author: Reynaldo Hermawan
     * Description: Class controlling the behavior of the Whip weapon.
     */

    public Text label;

    public Transform attackWhipPos;
    public Transform attackTipPos;
    public LayerMask otherPlayers;

    public float attackWhipRangeX;
    public float attackWhipRangeY;
    public float attackTipRangeX;
    public float attackTipRangeY;
    public float attackAngle;

    public float attackDelay = 0.8f;
    public float damageWhip = 15f;
    public float damageTip = 45f;
    public float knockbackWhip = 4000f;
    public float knockbackTip = 10000f;

    private GameObject player; 
    private float delayTimer = 0;
    private Animator anim;
    private AudioSource sound;


    // Use this for initialization
    void Start () {
        anim = gameObject.transform.Find("Sprite").GetComponent<Animator>();
        sound = gameObject.GetComponent<AudioSource>();
    }
   
    // Update is called once per frame
    void Update () {
        if(delayTimer > 0 ){
            delayTimer -= Time.deltaTime;
        }

    }

    // Called when a player picks up the weapon
    public void initWeaponUnique(GameObject player)
    {
        // Set the player reference
        this.player = player;
        label.gameObject.SetActive(false);
        anim.SetTrigger("Pickup");
        this.gameObject.transform.localPosition = new Vector3(0, -0.2f, 0);
    }

    // Called when a player drops the weapon
    public void resetWeaponUnique(GameObject player)
    {
        // Set the player reference back to null on drop
        this.player = null;
        label.gameObject.SetActive(true);
        anim.SetTrigger("Drop");
        StopCoroutine(swingWhip());
        this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        delayTimer = 0;
    }

    // Calls coroutine to swing whip when allowed
    public void shoot()
    {
        if(delayTimer <= 0){
            StartCoroutine(swingWhip());
            delayTimer = attackDelay;
        }
    }

    private IEnumerator swingWhip(){
        /* Author: Reynaldo Hermawan
         * Description: Checks if the whip hit anything and aapplies damage based on hit. Whip has 2 hitboxes with different damage values.
         */
        anim.SetTrigger("Swing");
        if (!GameControl.instance.paused)
        {
            sound.Play();
        }
        yield return new WaitForSeconds(.28f);
        Collider2D[] peopleTipHit = Physics2D.OverlapBoxAll(attackTipPos.position, new Vector2(attackTipRangeX, attackTipRangeY), 0, otherPlayers);
        if(peopleTipHit.Length > 0){
            for(int i = 0; i < peopleTipHit.Length; i++){
                if(peopleTipHit[i].gameObject != this.player){
                    peopleTipHit[i].GetComponent<PlayerController>().receiveDamage(damageTip);
                    peopleTipHit[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(knockbackTip * player.transform.localScale.x, 800));
                }
            }
        } else {
            Collider2D[] peopleWhipHit = Physics2D.OverlapBoxAll(attackWhipPos.position, new Vector2(attackWhipRangeX, attackWhipRangeY), 0, otherPlayers);
            for(int i = 0; i < peopleWhipHit.Length; i++){
                if(peopleWhipHit[i].gameObject != this.player){
                    peopleWhipHit[i].GetComponent<PlayerController>().receiveDamage(damageWhip);
                    peopleWhipHit[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(knockbackWhip * player.transform.localScale.x, 800));
                }
            }
        }
    }

    void OnDrawGizmosSelected(){ 
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackWhipPos.position, new Vector3(attackWhipRangeX, attackWhipRangeY, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackTipPos.position, new Vector3(attackTipRangeX, attackTipRangeY, 0));
    }

    public void stop()
    {

    }
}

