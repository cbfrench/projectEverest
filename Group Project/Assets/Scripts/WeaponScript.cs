using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponScript
{

    void setUniquePickupDefaults();
    //dunno if there is anything unique yet on pickup

    void setUniqueDroppedDefaults();
/*
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