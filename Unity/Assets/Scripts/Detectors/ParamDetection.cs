// Importing Packages
using System;
using System.Numerics;
using NAudio.Wave;
using MathNet.Numerics.IntegralTransforms;

// AudioAnalyzer Class
public class AudioAnalyzer
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
                    ProcessAudio(pcmBuffer, spectrum);
                }
            } while (bytesRead > 0);
        }
    }

    // Audio Processing
    private static void ProcessAudio(byte[] audioData, float[] spectrum)
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

        DetectNotes(spectrum);
    }

    // Frequency and Intensity detection
    private static void DetectNotes(float[] spectrum)
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
        float frequency = maxIndex * 44100f / (2 * spectrum.Length);

        // Debug logs
        Console.WriteLine("Detected Note Frequency: " + frequency + " Hz, Intensity: " + maxIntensity);
    }
}


