using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioWaves : MonoBehaviour
{
    [Header("Tone Settings")]
    [Range(125f, 16000f)]
    public double frequency = 1000.0;

    [Range(-80f, 0f)]
    public double gain = -20.0;

    [Tooltip("-1 = Left, 0 = Center, 1 = Right")]
    [Range(-1f, 1f)]
    public float pan = 0f; // <-- NEW

    public AudioSource audioSource { get; private set; }

    private double phase;
    private double increment;
    private double sampling_frequency;
    private double currentAmplitude;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sampling_frequency = AudioSettings.outputSampleRate;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        currentAmplitude = Mathf.Pow(10f, (float)gain / 20f);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * Mathf.PI / sampling_frequency;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;

            float sample = (float)(currentAmplitude * Mathf.Sin((float)phase));

            // --- THIS LOGIC IS UPDATED ---
            // Calculate left/right volumes based on pan
            float leftVol = (pan <= 0) ? 1.0f : 1.0f - pan;
            float rightVol = (pan >= 0) ? 1.0f : 1.0f + pan;

            // Apply the sample to the correct channels
            if (channels > 0)
                data[i] = sample * leftVol; // Left channel
            if (channels > 1)
                data[i + 1] = sample * rightVol; // Right channel

            if (phase > (Mathf.PI * 2))
            {
                phase -= (Mathf.PI * 2);
            }
        }
    }
}