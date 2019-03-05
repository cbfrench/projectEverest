using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAim : MonoBehaviour {

   private Vector3 mouse_pos;
   public Transform target; //Assign to the object you want to rotate
   private Vector3 object_pos;
   private float angle;

	// Update is called once per frame
	void Update () {
      Vector3 mousePos = Input.mousePosition;
      mousePos.z = 12f;

      Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
      mousePos.x = mousePos.x - objectPos.x;
      mousePos.y = mousePos.y - objectPos.y;

      float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}
}
