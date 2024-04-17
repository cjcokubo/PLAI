using System;
using System.Numerics;
using NAudio.Wave;
using MathNet.Numerics.IntegralTransforms;

public class ParamDetection
{
    public static void Main(string[] args)
    {
        // Buffer size = amount of audio data read at each iteration of the loop
        int bufferSize = 4096;
        float[] spectrum = new float[bufferSize / 2];

        // Testing wav file; Comment out if mic input used
        string audioFilePath = "musical_notes.wav";
        float sampleRate;

        using (var audioFile = new AudioFileReader(audioFilePath))
        {
            sampleRate = audioFile.WaveFormat.SampleRate;
        }

        // Processing audio file (into PCM)
        // Comment out if audio already preprocessed
        using (var audioFile = new AudioFileReader(audioFilePath))
        {
            var buffer = new float[bufferSize];
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
        float[] floatBuffer = new float[audioData.Length / 2];
        for (int i = 0; i < floatBuffer.Length; i++)
        {
            floatBuffer[i] = BitConverter.ToInt16(audioData, i * 2) / 32768f;
        }

        // Apply a Hanning window
        for (int i = 0; i < floatBuffer.Length; i++)
        {
            floatBuffer[i] *= (float)(0.5 * (1 - Math.Cos(2 * Math.PI * i / (floatBuffer.Length - 1))));
        }

        Complex[] fftBuffer = new Complex[floatBuffer.Length];
        for (int i = 0; i < floatBuffer.Length; i++)
        {
            fftBuffer[i] = new Complex(floatBuffer[i], 0);
        }
        Fourier.Forward(fftBuffer);

        // Calculate the maximum index for 2490 Hz
        int maxIndexLimit = (int)((2490 * fftBuffer.Length) / sampleRate);

        for (int i = 0; i <= maxIndexLimit; i++)
        {
            spectrum[i] = (float)Math.Sqrt(fftBuffer[i].Real * fftBuffer[i].Real + fftBuffer[i].Imaginary * fftBuffer[i].Imaginary);
        }

        DetectNotes(spectrum, sampleRate, maxIndexLimit);
    }

    // Detectors
    private static void DetectNotes(float[] spectrum, float sampleRate, int maxIndexLimit)
    {
        float maxIntensity = 0f;
        int maxIndex = 0;
        float threshold = 0.1f;

        // Only consider frequencies up to 2490 Hz
        for (int i = 0; i <= maxIndexLimit; i++)
        {
            if (spectrum[i] > maxIntensity && spectrum[i] > threshold)
            {
                maxIntensity = spectrum[i];
                maxIndex = i;
            }
        }

        float frequency = maxIndex * sampleRate / spectrum.Length;

        string[] notes = {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
        int semitoneCount = (int)(Math.Round(12 * (Math.Log(frequency / 440) / Math.Log(2))) + 69);
        int noteIndex = semitoneCount % 12;
        string noteName = notes[noteIndex];

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

        Console.WriteLine("Frequency: " + frequency + " Hz, Note: " + noteName + ", Intensity: " + maxIntensity);
    }
}
