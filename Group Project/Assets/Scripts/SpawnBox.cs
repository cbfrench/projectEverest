using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    public float timeBetweenSpawn;
    public float finalFightWidth;
    public int raycastDownLength = 40;

    private bool started = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startRandomWeaponSpawn(){
        if(!started){
            started = true;
            StartCoroutine("spawnWeaponCont");
        }
    }

    private IEnumerator spawnWeaponCont() {
        //if(m_CurrentSpawnCount <  SpawnCount)
        //{
        //for loop?
        Vector3 position = this.gameObject.transform.position + Random.insideUnitSphere * finalFightWidth;
        position.y = this.gameObject.transform.position.y;
        position.z = 0;
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, raycastDownLength);
        if(hit != null && hit.collider != null && (hit.collider.tag == "Ground" || hit.collider.tag == "Wall"))
        {
            GameObject weapon = Instantiate(GameControl.instance.returnRandomWeapon(), position, Quaternion.identity);
            weapon.SetActive(true);
            yield return new WaitForSeconds(timeBetweenSpawn);
            StartCoroutine("spawnWeaponCont");
        }else{
            StartCoroutine("spawnWeaponCont");
        }        
        //}
    }
}
