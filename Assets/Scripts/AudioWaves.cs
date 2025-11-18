using UnityEngine;

// We add LineRenderer to the requirements
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
    public float pan = 0f;

    public AudioSource audioSource { get; private set; }

    private double phase;
    private double increment;
    private double sampling_frequency;
    private double currentAmplitude;

    [Header("Visualization Settings")]
    public int resolution = 200;
    public float waveWidth = 10f;
    public float waveHeight = 2f;
    public float waveSpeed = 2f;
    [SerializeField] LineRenderer lineRenderer;

    private bool isVisualizing = false;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sampling_frequency = AudioSettings.outputSampleRate;
        audioSource.playOnAwake = false;

        lineRenderer.positionCount = resolution;
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.gameObject.SetActive(false);
    }

    void Update()
    {
        currentAmplitude = Mathf.Pow(10f, (float)gain / 20f);

        bool isPlaying = audioSource.isPlaying && gain > -80.0f;

        if (isPlaying)
        {
            if (!isVisualizing)
            {
                isVisualizing = true;
                lineRenderer.gameObject.SetActive(true);
            }
            DrawSineWave();
        }
        else
        {
            if (isVisualizing)
            {
                isVisualizing = false;
                lineRenderer.gameObject.SetActive(false);
                lineRenderer.positionCount = 0;
            }
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * Mathf.PI / sampling_frequency;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;

            float sample = (float)(currentAmplitude * Mathf.Sin((float)phase));

            float leftVol = (pan <= 0) ? 1.0f : 1.0f - pan;
            float rightVol = (pan >= 0) ? 1.0f : 1.0f + pan;

            if (channels > 0)
                data[i] = sample * leftVol;
            if (channels > 1)
                data[i + 1] = sample * rightVol;

            if (phase > (Mathf.PI * 2))
            {
                phase -= (Mathf.PI * 2);
            }
        }
    }
    void DrawSineWave()
    {
        if (lineRenderer.positionCount != resolution)
        {
            lineRenderer.positionCount = resolution;
        }

        float timePhase = Time.time * (float)frequency * waveSpeed * 0.01f;

        for (int i = 0; i < resolution; i++)
        {
            float x = (float)i / (resolution - 1);

            float cycles = 3f;

            float y = Mathf.Sin((x * cycles * 2 * Mathf.PI) + timePhase);

            float xPos = x * waveWidth - (waveWidth / 2f);
            float yPos = y * waveHeight;

            lineRenderer.SetPosition(i, new Vector3(xPos, yPos, 0));
        }
    }
}