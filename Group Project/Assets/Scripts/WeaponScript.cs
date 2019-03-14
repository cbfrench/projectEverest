using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponScript
{
    /* Author: Caleb Biggers & Reynaldo Hermawan
     * Description: Interface for the Weapon methods
     */

    void initWeaponUnique(GameObject player);

    void resetWeaponUnique(GameObject player);

    void shoot();

    void stop();
}