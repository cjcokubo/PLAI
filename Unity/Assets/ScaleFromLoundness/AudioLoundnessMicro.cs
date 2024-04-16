using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoundnessMicro : MonoBehaviour
{
    public int samplewindow = 64;
    private AudioClip microphoneClip;
    
    // Start is called before the first frame update
    void Start()
    {
        MicrophoneToAudioClip();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MicrophoneToAudioClip()
    {
        string microphoneName = Microphone.devices[0];
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
    }

    public float GetLoundnessFromMicrophone()
    {
        return GetLoundnessFromAudio(Microphone.GetPosition(Microphone.devices[0]), microphoneClip);
    }

    public float GetLoundnessFromAudio(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - samplewindow;

        if (startPosition < 0)
        {
            return 0;
        }

        float[] waveData = new float[samplewindow];
        clip.GetData(waveData, startPosition);

        // Loudness
        float totalLoudness = 0;

        for (int i = 0; i < samplewindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]); // Sound range -1 to 1, 0 = no sound
        }

        return totalLoudness / samplewindow;
    }
}
