using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour, Projectile {
   public float speed;
   public float fireRate;

   private TrailRenderer tr = null;
	
   // Is called first before Start() but also happens after active
   void OnEnable(){
      if (tr != null){
         tr.Clear();
      } 
   }

   void Start(){
      tr = gameObject.transform.Find("Trail").GetComponent<TrailRenderer>();
   }

	// Update is called once per frame
	void Update () {
		if(speed != 0){
         transform.position += transform.right * (speed * Time.deltaTime);
      } else{
         Debug.Log("No projectile speed.");
      }
	}

   void OnTriggerEnter2D(Collider2D other){
      gameObject.SetActive(false);
   }

   void OnBecameInvisible(){
      gameObject.SetActive(false);
   }

   public float getFireRate(){
      return fireRate;
   }
}
