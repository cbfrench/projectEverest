using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class musicSliderInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().value = Admin.musicVolume * 25;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
