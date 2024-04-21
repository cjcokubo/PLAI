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

    private float lastFrequency = 0f;
    private float lastNoteEndTime = 0f;
    private float currentNoteStartTime = 0f;

    // Thresholds for articulation detection
    private float staccatoDurationThreshold = 200; // in milliseconds
    private float legatoGapThreshold = 50; // in milliseconds

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
        }
        else
        {
            if (frequency != lastFrequency)
            {
                float currentTime = Time.time * 1000;
                if (lastFrequency != 0) 
                {
                    float noteDuration = currentTime - currentNoteStartTime;
                    float noteGap = currentNoteStartTime - lastNoteEndTime;
                    string articulation = GetArticulation(noteDuration, noteGap);

                    text.text = $"{Frequency2Note(frequency)}\n{frequency:0.0} Hz\n{GetVibe(frequency)}\n{noteDuration}ms\n{articulation}";
                }
                currentNoteStartTime = currentTime;
                lastFrequency = frequency;
            }
            lastNoteEndTime = Time.time * 1000;
        }
    }

    string Frequency2Note(float frequency)
    {
        var noteNumber = Mathf.RoundToInt(12 * Mathf.Log(frequency / 440) / Mathf.Log(2) + 69);
        string[] names = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        return names[noteNumber % 12];
    }

    string GetVibe(float frequency)
    {
        if (frequency > 2000) return "KIKI";
        if (frequency >= 200 && frequency <= 2000) return "NORMAL";
        return "BOUBA";
    }

    string GetArticulation(float noteDuration, float noteGap)
    {
        if (noteDuration <= staccatoDurationThreshold) return "Staccato";
        if (noteGap <= legatoGapThreshold && lastFrequency != 0) return "Legato";
        return "Normal";
    }
}






// ORIGINAL AS OF 04/20/2024; Remove when above changes as extensively tested

//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class LiveNoteDetector : MonoBehaviour
//{

//    public TextMeshProUGUI text;
//    public MicrophoneListener listener;
//    public AudioPitchEstimator estimator;

//    public float estimationRate = 30;

//    private int clearCounter = 0;
//    private int clearLimit = 30;


//    // Start is called before the first frame update
//    void Start()
//    {
//        InvokeRepeating(nameof(UpdatePitch), 0, 1.0f / estimationRate);
//    }

//    void UpdatePitch()
//    {
//        var frequency = estimator.Estimate(listener.GetAudioSource());

//        if (float.IsNaN(frequency))
//        {
//            clearCounter += 1;
//            if (clearCounter >= clearLimit)
//            {
//                text.text = "";
//                clearCounter = 0;
//            }
//            return;
//        }
//        else
//        {
//            text.text = string.Format("{0}\n{1:0.0} Hz\n{2}", Frequency2Note(frequency), frequency, GetVibe(frequency));
//        }

//    }

//    string Frequency2Note(float frequency)
//    {
//        var noteNumber = Mathf.RoundToInt(12 * Mathf.Log(frequency / 440) / Mathf.Log(2) + 69);
//        string[] names = {
//            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
//        };
//        return names[noteNumber % 12];
//    }


//    string GetVibe(float frequency)
//    {
//        string vibe = "N/A";
//        switch (frequency)
//        {
//            case > 2000:
//                vibe = "KIKI";
//                break;
//            case >= 200 and <= 2000:
//                vibe = "NORMAL";
//                break;
//            default:
//                vibe = "BOUBA";
//                break;
//        }
//        return vibe;
//    }

//    float GetIntensity(float frequency)
//    {
//        // function for getting volume of input
//        // tackled in ScaleFromLoudness
//    }

//    string GetArticulation(float frequency)
//    {
//        // function for identifying staccato vs. legato
//    }

//}


