using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBulletBehavior : MonoBehaviour, Projectile {
   public float speed;
   public float fireRate;

   public Vector3 direction;
   public Vector3 nextDirection;

   private RaycastHit2D hit;
   private TrailRenderer tr = null;
   
   // Is called first before Start() but also happens after active
   void OnEnable(){
      if (tr != null){
         tr.Clear();
      } 
      direction = gameObject.transform.right;
      nextDirection = ReflectionCast(gameObject.transform.position, direction);
   }

   void Start(){
      tr = gameObject.transform.Find("Trail").GetComponent<TrailRenderer>();
      direction = gameObject.transform.right;
      nextDirection = ReflectionCast(gameObject.transform.position, direction);
   }

   // Update is called once per frame
   void Update () { 
      if(speed != 0){
         transform.position += direction * (speed * Time.deltaTime);
         //nextDirection = ReflectionCast(gameObject.transform.position, direction);
      } else{
         Debug.Log("No projectile speed.");
      }
   }

   void OnTriggerEnter2D(Collider2D other){
      if (nextDirection != Vector3.zero){
         Quaternion newRotation = new Quaternion();
         newRotation.SetFromToRotation(hit.normal, nextDirection.normalized);
         gameObject.transform.rotation = newRotation;
         gameObject.transform.rotation *= Quaternion.Euler(0, 0, 90);
         direction = nextDirection;
         nextDirection = ReflectionCast(gameObject.transform.position, direction);
      } else {
         gameObject.SetActive(false);
      }
   }

   void OnBecameInvisible(){
      gameObject.SetActive(false);
   }

   private Vector3 ReflectionCast(Vector3 position, Vector3 direction)
   {
      LayerMask mask = LayerMask.GetMask("Ground");
      //Debug.DrawRay (transform.position, direction * 50, Color.white);
      hit = Physics2D.Raycast(gameObject.transform.position, direction, Mathf.Infinity, mask);
      if (hit != null && hit.collider != null) {
         Vector3 inDirection = Vector3.Reflect(direction, hit.normal);
         /*
         Debug.DrawRay (hit.point, inDirection * 50, Color.red);
         Debug.DrawRay (transform.position,  hit.normal * 50, Color.green);
         Debug.Log(hit.distance.ToString("R"));
         */
         return inDirection;  
      }else{
         return Vector3.zero;
      }
   }

   public float getFireRate(){
      return fireRate;
   }
}
