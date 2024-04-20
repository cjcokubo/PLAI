using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;




public class NoteDisplay : MonoBehaviour
{
    ParamDetection detector = new ParamDetection();
    
    public TextMeshProUGUI text;

    void Start()
    {
        text.text = "Hello World";

        List<string> pitches = detector.GetPitches();

        foreach (var pitch in pitches) 
        {
            text.text += "\n" + pitch;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
