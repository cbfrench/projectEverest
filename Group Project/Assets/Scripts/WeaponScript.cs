using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponScript
{

    void setPickupDefaults();
/*
        item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        item.GetComponent<Rigidbody2D>().angularVelocity = 0;
        item.gameObject.transform.eulerAngles = equip.transform.eulerAngles;
        item.gameObject.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        item.gameObject.transform.position = equip.transform.position;
        item.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);*/

    void setUniqueDroppedDefaults();
/*
        Transform dropped = equip.transform.GetChild(0);
        dropped.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        dropped.gameObject.transform.eulerAngles = Vector3.zero;
        float s = -1;
        if (transform.localScale.x == 1)
        {
            s = 1;
        }*/
    void deathLaunchWeapon(); //maybe make helper class???
/*      dropped.gameObject.transform.localScale = new Vector3(s, dropped.gameObject.transform.localScale.y, dropped.gameObject.transform.localScale.z);
        dropped.parent = GameControl.instance.pickups.transform;
        dropped.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        if (dropped.name.Contains("Flashlight"))
        {
            dropped.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
            dropped.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            dropped.GetComponent<Rigidbody2D>().gravityScale = 5;
        }
        if (dropped.name.Contains("Flamethrower") || dropped.name.Contains("Squirt Gun"))
        {
            dropped.transform.GetChild(0).GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();
        }*/


    void shootProjectile();
}

/*
    private void fireOnce()
    {
        if(equip.transform.childCount != 1)
        {
            return;
        }
        if (firing && singleFire)
        {
            GameObject projectile = equip.transform.GetChild(0).gameObject;
            if (equip.transform.GetChild(0).name.Contains("Flashlight"))
            {
                projectile = equip.transform.GetChild(0).transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
                projectile.SetActive(!projectile.activeSelf);
            }
            singleFire = false;
        }
    }
    private void fireMulti()
    {
        if (equip.transform.childCount == 0)
        {
            return;
        }
        if (equip.transform.GetChild(0).name.Contains("Flashlight"))
        {
            return;
        }
        GameObject projectile = equip.transform.GetChild(0).gameObject; 
        projectile = equip.transform.GetChild(0).transform.GetChild(0).GetChild(1).gameObject;
        ParticleSystem ps = projectile.GetComponent<ParticleSystem>();
        if (firing)
        {
            if (!ps.isEmitting)
            {
                ps.Play();
            }
        }
        else
        {
            if (ps.isEmitting)
            {
                ps.Stop();
            }
        }
    }

    private void checkDamage()
    {
        if(particleParent == null || particleParent.name == gameObject.name)
        {
            return;
        }
        if (particleParent != environmentalDamage)
        {
            if (damaged)
            {
                health -= GameControl.instance.flamethrowerDamage * Time.deltaTime;
                healthBar.value = health;
                damaged = false;
                if (health <= 0)
                {
                    dead = true;
                }
            }
        }
        else
        {
            if (damaged)
            {
                health -= GameControl.instance.avalancheDamage * Time.deltaTime;
                healthBar.value = health;
                damaged = false;
                if (health <= 0)
                {
                    dead = true;
                }
            }
        }
    }
*/