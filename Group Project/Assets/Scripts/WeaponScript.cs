using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponScript
{

    void initWeaponUnique(GameObject player);
    //dunno if there is anything unique yet on pickup

    void resetWeaponUnique(GameObject player);

    void shoot();

    void stop();
}