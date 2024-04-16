// Importing Packages
using System;
using System.Numerics;
using NAudio.Wave;
using MathNet.Numerics.IntegralTransforms;

// ParamDetection Class
public class ParamDetection
{
    public static void Main(string[] args)
    {
        // Buffer size = amount of audio data read at each iteration of the loop
        int bufferSize = 4096;

        // spectrum array is half the bufferSize since we only store first half of the spectrum
        // Fourier transform of a real-valued signal produces a symmetric result
        float[] spectrum = new float[bufferSize / 2]; // Adjusted size

        // Testing wav file; Comment out if mic input used
        string audioFilePath = "Clown in Town.wav"; 

        // Retrieving sample rate from the audio file
        float sampleRate;
        using (var audioFile = new AudioFileReader(audioFilePath))
        {
            sampleRate = audioFile.WaveFormat.SampleRate;
        }

        // Processing audio file
        // Comment out if audio already preprocessed
        using (var audioFile = new AudioFileReader(audioFilePath))
        {
            var buffer = new float[bufferSize];

            var waveFormat = new WaveFormat(audioFile.WaveFormat.SampleRate, 1);

            int bytesRead;
            do
            {
                bytesRead = audioFile.Read(buffer, 0, bufferSize);

                if (bytesRead > 0)
                {
                    byte[] pcmBuffer = new byte[bytesRead * 2];
                    Buffer.BlockCopy(buffer, 0, pcmBuffer, 0, pcmBuffer.Length);
                    ProcessAudio(pcmBuffer, spectrum, sampleRate);
                }
            } while (bytesRead > 0);
        }
    }

    // Audio Processing
    private static void ProcessAudio(byte[] audioData, float[] spectrum, float sampleRate)
    {
        float[] floatBuffer = new float[audioData.Length / 4]; 
        for (int i = 0; i < floatBuffer.Length; i++)
        {
            floatBuffer[i] = BitConverter.ToInt16(audioData, i * 2) / 32768f;
        }

        // Using Fast Fourier Transform
        Complex[] fftBuffer = new Complex[spectrum.Length]; 
        for (int i = 0; i < floatBuffer.Length / 2; i++) 
        {
            fftBuffer[i] = new Complex(floatBuffer[i], 0);
        }
        Fourier.Forward(fftBuffer);

        for (int i = 0; i < spectrum.Length; i++)
        {
            spectrum[i] = (float)Math.Sqrt(fftBuffer[i].Real * fftBuffer[i].Real + fftBuffer[i].Imaginary * fftBuffer[i].Imaginary);
        }

        DetectNotes(spectrum, sampleRate);
    }

    // Frequency and Intensity detection
    private static void DetectNotes(float[] spectrum, float sampleRate)
    {
        float maxIntensity = 0f;
        int maxIndex = 0;
        float threshold = 0.1f;

        for (int i = 0; i < spectrum.Length; i++)
        {
            if (spectrum[i] > maxIntensity && spectrum[i] > threshold)
            {
                maxIntensity = spectrum[i];
                maxIndex = i;
            }
        }

        // Adjusted to match audio sample rate and buffer size
        float frequency = maxIndex * sampleRate / (2 * spectrum.Length);

        // Calculate duration in milliseconds
        float length = 1000f / frequency;

        // Determine musical note
        // Formula: return noteStrings[(Math.round(12 * (Math.log(frequency / 440) / Math.log(2))) + 69) % 12];
        string[] notes = {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
        // int semitoneCount = (int)Math.Round(12 * Math.Log(frequency / 440.0) / Math.Log(2));
        int semitoneCount = (int)(Math.Round(12 * (Math.Log(frequency / 440) / Math.Log(2))) + 69) % 12;
        int noteIndex = (semitoneCount + 9) % 12;
        string noteName = notes[noteIndex];
        // string noteName = "aaaa";


        // Determine vibe
        // Ranges determined after consultation with Jackson Waters
        string vibe;
        if (frequency > 2000)
        {
            vibe = "KIKI";
        }
        else if (frequency >= 200 && frequency <= 2000)
        {
            vibe = "NORMAL";
        }
        else
        {
            vibe = "BOUBA";
        }

        // Logs
        Console.WriteLine("Frequency: " + frequency + "Hz, Note: " + noteName + ", Intensity: " + maxIntensity + ", Length: " + length + " ms, Vibe: " + vibe);
    }

}



