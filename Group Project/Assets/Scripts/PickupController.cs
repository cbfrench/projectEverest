using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{

    public bool isEquipped = false;     // Stores if pickup is equipped by a player
    public GameObject player = null;    // Stores the player holding the pickup ** Might be better way of doing this
}
