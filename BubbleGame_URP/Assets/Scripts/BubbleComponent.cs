using System.Collections;
using System.Collections.Generic;
using PolySpatial.Samples;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class BubbleComponent : MonoBehaviour
{

    float speed = 1.2f;
    public GameObject decalPrefab;

    Vector3 Velocity;
    Vector3 oldPos;
    public Color myColor;

    // Start is called before the first frame update
    void Start()
    {
        oldPos = transform.position;
        myColor = Random.ColorHSV();
        Color copyColor = myColor;
        copyColor.a = 0.2f;
        GetComponent<MeshRenderer>().material.color = copyColor;
        ConfigureFromXR();
    }

    void ConfigureFromXR()
    {
        float lengthValue = 0.5f;
        float volumeValue = 0.5f;
        float pitchValue  = 0.5f;


        GameObject slider = GameObject.Find("Volume Slider");
        if(slider)
        {
            volumeValue = slider.GetComponent<SpatialUISlider>().GetPercent();
        }
        slider = GameObject.Find("Pitch Slider");
        if (slider)
        {
            pitchValue = slider.GetComponent<SpatialUISlider>().GetPercent();
        }
        slider = GameObject.Find("Length Slider");
        if (slider)
        {
            lengthValue = slider.GetComponent<SpatialUISlider>().GetPercent();
        }
        //use values! 0.5f should be no change!!!========


        transform.localScale *= (lengthValue + 0.5f);
        speed *= (volumeValue + 0.5f);

    }


    // Update is called once per frame
    void Update()
    {
        Vector3 fwd = transform.forward;
        //fwd += Vector3.up * speed * 0.1f;
        transform.position += (fwd * speed * Time.deltaTime);

        Velocity = transform.position - oldPos;
        oldPos = transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        print("hit!");
        //Instantiate(decalPrefab, collision., Quaternion.LookRotation(-collision.contacts[0].normal));
        //decalPrefab.GetComponent<MyPolyMesh>().myColor = myColor;
        //speed = 0f;
        //Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("HIT!");
        print(collision.gameObject.name);

        if (collision.gameObject.TryGetComponent(out ARPlane plane))
        {
            if(plane.classification == UnityEngine.XR.ARSubsystems.PlaneClassification.Wall ||
               plane.classification == UnityEngine.XR.ARSubsystems.PlaneClassification.Floor ||
               plane.classification == UnityEngine.XR.ARSubsystems.PlaneClassification.Ceiling
                )
            {
                //hit wall floor or ceiling!
                SpawnDecalOnHit(collision);
            }
            else
            {
                //hit unidentified object (default: ignore)
            }
        }

        else if(!GameObject.FindObjectOfType<ARPlane>())
        {
            //testing mode- NOT IN AVP!
            SpawnDecalOnHit(collision);
        }
           

    }


    void SpawnDecalOnHit(Collision collision)
    {
        var decal = Instantiate(decalPrefab, collision.contacts[0].point, Quaternion.LookRotation(-collision.contacts[0].normal));
        if (decal.GetComponent<MyPolyMesh>())
        {
            decal.GetComponent<MyPolyMesh>().myColor = myColor;
        }
        else
        {
            //not my custom decal- something else!
            Color copyColor = myColor;
            copyColor.a = 1f;
            decal.GetComponentInChildren<MeshRenderer>().material.color = copyColor;
            decal.transform.localScale = transform.localScale;
            decal.transform.position += collision.contacts[0].normal * 0.001f * Random.Range(0.995f,1.005f);
        }
        speed = 0f;
        Destroy(gameObject);
    }
}
