using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{

    public bool isEquipped = false;         // Stores if pickup is equipped by a player
    public PlayerController player = null;  // Stores the player holding the pickup ** Might be better way of doing this

    // Method to set the isEquipped value
    public void SetEquipped(bool value)
    {
        isEquipped = value;
    }

    // Method to set the player
    public void SetPlayer(PlayerController thePlayer)
    {
        player = thePlayer;
    }
}
