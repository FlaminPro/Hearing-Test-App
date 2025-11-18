using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HearingTestManager : MonoBehaviour
{
    [Header("Script References")]
    [Tooltip("Drag your 'AudioWaves' script component here")]
    public AudioWaves toneGenerator;

    [Header("UI Elements")]
    [Tooltip("The main slider for 'barely hearable' level")]
    public Slider calibrationSlider;

    [Tooltip("Button to confirm the level and move to the next frequency")]
    public Button confirmButton;

    [Tooltip("Button for 'I still can't hear anything'")]
    public Button noResponseButton;

    [Tooltip("Text to show the user (e.g., 'Calibrating 1000 Hz')")]
    public TMP_Text statusText;

    [Header("Test Frequencies")]
    public float[] frequenciesToTest = { 1000, 2000, 4000, 8000, 500, 250 };

    private Dictionary<float, float> calibrationThresholds = new Dictionary<float, float>();
    private int currentFrequencyIndex = 0;

    void Awake()
    {
        calibrationSlider.minValue = -80f;
        calibrationSlider.maxValue = 0f;

        calibrationSlider.interactable = true;

        calibrationSlider.onValueChanged.AddListener(OnSliderChanged);
        confirmButton.onClick.AddListener(OnConfirmPressed);
        noResponseButton.onClick.AddListener(OnNoResponsePressed);
    }

    void OnEnable()
    {
        Debug.Log("Calibration Panel Enabled. Starting calibration.");

        currentFrequencyIndex = 0;
        calibrationThresholds.Clear();

        LoadFrequency(currentFrequencyIndex);
    }

    void OnDisable()
    {
        Debug.Log("Calibration Panel Disabled. Stopping tone.");

        if (toneGenerator != null && toneGenerator.audioSource != null)
        {
            toneGenerator.audioSource.Stop();
            toneGenerator.gain = -80;
        }
    }

    void OnSliderChanged(float newDbValue)
    {
        // The slider's value is the decibel (dBFS) value.
        toneGenerator.gain = (double)newDbValue;
    }

    void LoadFrequency(int index)
    {
        float freq = frequenciesToTest[index];

        if (statusText != null)
        {
            statusText.text = $"Calibrating: {freq} Hz";
        }

        toneGenerator.frequency = freq;

        calibrationSlider.value = -60f;
        toneGenerator.gain = -60f;


        toneGenerator.audioSource.Play();
    }

    void OnConfirmPressed()
    {
        float freq = frequenciesToTest[currentFrequencyIndex];
        float threshold_dBFS = calibrationSlider.value;
        calibrationThresholds[freq] = threshold_dBFS;

        Debug.Log($"Threshold for {freq} Hz saved: {threshold_dBFS} dBFS");

        currentFrequencyIndex++;
        if (currentFrequencyIndex < frequenciesToTest.Length)
        {
            LoadFrequency(currentFrequencyIndex);
        }
        else
        {
            FinishCalibration();
        }
    }

    void OnNoResponsePressed()
    {
        float freq = frequenciesToTest[currentFrequencyIndex];
        calibrationThresholds[freq] = float.PositiveInfinity;

        Debug.Log($"No Response (NR) recorded for {freq} Hz");

        currentFrequencyIndex++;
        if (currentFrequencyIndex < frequenciesToTest.Length)
        {
            LoadFrequency(currentFrequencyIndex);
        }
        else
        {
            FinishCalibration();
        }
    }

    void FinishCalibration()
    {
        Debug.Log("--- CALIBRATION COMPLETE ---");
        toneGenerator.audioSource.Stop();

        if (statusText != null)
        {
            statusText.text = "Calibration Complete!";
        }

        StaticDataAndHelpers.thresholds_dBFS = this.calibrationThresholds;

        // Print results to console
        foreach (var entry in calibrationThresholds)
        {
            Debug.Log($"Result: {entry.Key} Hz at {entry.Value} dBFS");
        }

        UI.instance.TestMenuFunc();
    }
}