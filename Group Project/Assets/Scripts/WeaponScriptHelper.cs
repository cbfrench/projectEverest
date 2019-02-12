using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponScriptHelper
{
    public static void setDroppedDefaults(this WeaponScript weapon){
/*      Transform dropped = equip.transform.GetChild(0);
        dropped.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        dropped.gameObject.transform.eulerAngles = Vector3.zero;
        float s = -1;
        if (transform.localScale.x == 1)
        {
            s = 1;
        }
        //maybe make helper class???
        dropped.gameObject.transform.localScale = new Vector3(s, dropped.gameObject.transform.localScale.y, dropped.gameObject.transform.localScale.z);
        dropped.parent = GameControl.instance.pickups.transform;
        dropped.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);*/
        weapon.setUniqueDroppedDefaults();
    }

    public static void setPickupDefaults(this WeaponScript weapon){
        /*
        item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        item.GetComponent<Rigidbody2D>().angularVelocity = 0;
        item.gameObject.transform.eulerAngles = equip.transform.eulerAngles;
        item.gameObject.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        item.gameObject.transform.position = equip.transform.position;
        item.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);*/
        weapon.setUniquePickupDefaults();
    }
}
