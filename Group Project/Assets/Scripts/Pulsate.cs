using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pulsate : MonoBehaviour
{
    public Text t;
    public float speed;

    private Quaternion fixedRotation;

    private void Awake()
    {
        fixedRotation = transform.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        t = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        t.color = new Color32(255, 255, 255, (byte)Mathf.Floor(Mathf.PingPong(Time.time * speed, 255)));
    }

    private void LateUpdate()
    {
        transform.rotation = fixedRotation;
    }
}
