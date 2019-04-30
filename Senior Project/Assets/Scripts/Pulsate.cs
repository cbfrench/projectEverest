using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pulsate : MonoBehaviour
{
    /* Author: Connor French
     * Description: class for making text labels gradually change color so they don't appear so static
     */
    public Text t;
    public float speed;

    private Quaternion fixedRotation;

    private void Awake()
    {
        /* Author: Connor French
         * Description: sets intial rotation to prevent label from rotating
         */
        fixedRotation = transform.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        /* Author: Connor French
         * Description: sets the variable t to the Text object in question
         */
        t = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        /* Author: Connor French
         * Description: changes the color of the text to make it pulsate
         */
        t.color = new Color32(255, 255, 255, (byte)Mathf.Floor(Mathf.PingPong(Time.time * speed, 255)));
    }

    private void LateUpdate()
    {
        /* Author: Connor French
         * Description: ensures that initial rotation remains the same
         */
        transform.rotation = fixedRotation;
    }
}
