using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour {

   public List<GameObject> pooledProjectiles = new List<GameObject>();
   public GameObject projectilePrefab;
   public int poolAmt;
   public GameObject firePoint;

   private float timeToFire = 0;

   void Start(){
      for (int i = 0; i < poolAmt; i++) {
        GameObject obj = (GameObject)Instantiate(projectilePrefab);
        obj.SetActive(false); 
        pooledProjectiles.Add(obj);
      }
   }

   void Update () {
      if(Input.GetMouseButton(0) && Time.time >= timeToFire){
         timeToFire = Time.time + 1 / projectilePrefab.GetComponent<Projectile>().getFireRate();
         FireProjectile();
      }
   }

   void FireProjectile(){
      GameObject projectile = this.GetInactiveProjectile();

      if (projectile != null) {
         projectile.transform.position = firePoint.transform.position;
         projectile.transform.localRotation = firePoint.transform.parent.transform.localRotation;
         projectile.SetActive(true);
      }
   }

   public GameObject GetInactiveProjectile() {
      for (int i = 0; i < pooledProjectiles.Count; i++) {
         if (!pooledProjectiles[i].activeInHierarchy) {
            return pooledProjectiles[i];
         }
      }  
      return null;
   }
}
