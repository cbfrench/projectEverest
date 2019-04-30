using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponScriptHelper
{
    /* Author: Caleb Biggers & Reynaldo Hermawan
     * Description: Script to allow all weapons to have the same methods
     */

    public static void resetWeapon(this WeaponScript weapon, GameObject attachedGO, GameObject player)
    {
        /* Author: Caleb Biggers & Reynaldo Hermawan
         * Description: Method for when a weapon is dropped to reset it 
         */

        // Change rb2d type back to dynamic
        attachedGO.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        // Reset velocity
        attachedGO.transform.eulerAngles = Vector3.zero;
        // Set direction to direction player is facing
        attachedGO.transform.localScale = new Vector3(player.transform.localScale.x, attachedGO.transform.localScale.y, attachedGO.transform.localScale.z);  // Scale may need to be moved to individual scripts
        // Individual weapon script
        weapon.resetWeaponUnique(player);
    }

    public static void initWeapon(this WeaponScript weapon, GameObject attachedGO, GameObject player)
    {
        /* Author: Caleb Biggers & Reynaldo Hermawan
         * Description: Method for when a weapon is picked up by a player
         */

        // Change rb2d to kinematic
        attachedGO.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        // Reset velocity and angular velocity
        attachedGO.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        attachedGO.GetComponent<Rigidbody2D>().angularVelocity = 0;
        // Set angle to holding parents angle
        attachedGO.gameObject.transform.eulerAngles = player.transform.Find("Holding").transform.eulerAngles;
        // Set scale and position to that of the holding
        attachedGO.transform.localScale = new Vector3(1, attachedGO.transform.localScale.y, attachedGO.transform.localScale.z);    // Scale may need to be moved to individual scripts
        attachedGO.transform.position = player.transform.Find("Holding").transform.position;
        // Individual weapon script
        weapon.initWeaponUnique(player);
    }
}