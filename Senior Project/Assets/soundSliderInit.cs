using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class soundSliderInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().value = Admin.soundVolume * 25;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
