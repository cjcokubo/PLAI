using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFromMicrophone : MonoBehaviour
{
    public AudioSource source;
    public Vector3 minScale;
    public Vector3 maxScale;
    public AudioLoundnessMicro detector;

    public float loundnessSensibility = 100;
    public float threshold = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float loundness = detector.GetLoundnessFromMicrophone() * loundnessSensibility;

        if (loundness < threshold){
            loundness = 0;
        }


        transform.localScale = Vector3.Lerp(minScale, maxScale, loundness);
        //When loundness is 0, the scale will be minScale, and when loundness is 1, the scale will be maxScale.
    }
}
