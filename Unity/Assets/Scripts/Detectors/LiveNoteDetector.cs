using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiveNoteDetector : MonoBehaviour
{

    public TextMeshProUGUI text;
    public MicrophoneListener listener;
    public AudioPitchEstimator estimator;

    public float estimationRate = 30;

    private int clearCounter = 0;
    private int clearLimit = 30;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(UpdatePitch), 0, 1.0f / estimationRate);
    }

    void UpdatePitch()
    {
        var frequency = estimator.Estimate(listener.GetAudioSource());

        if (float.IsNaN(frequency))
        {
            clearCounter += 1;
            if (clearCounter >= clearLimit)
            {
                text.text = "";
                clearCounter = 0;
            }
            return;
        }
        else
        {
            text.text = string.Format("{0}\n{1:0.0} Hz", Frequency2Note(frequency), frequency);
        }
        
    }

    string Frequency2Note(float frequency)
    {
        var noteNumber = Mathf.RoundToInt(12 * Mathf.Log(frequency / 440) / Mathf.Log(2) + 69);
        string[] names = {
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
        };
        return names[noteNumber % 12];
    }


}
