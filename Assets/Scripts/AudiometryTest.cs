using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AudiometryTest : MonoBehaviour
{
    [Header("Script References")]
    [Tooltip("Drag your 'AudioWaves' (ToneGenerator) script here")]
    public AudioWaves toneGenerator;

    [Header("Manager References")]
    public UI uiManager; // <-- NEW: Drag your UI script here

    [Header("UI Elements")]
    public Button heardButton;
    public Button noResponseButton;
    public TMP_Text statusText;

    // This is where the test results will be stored
    private Dictionary<float, int> audiogramResults = new Dictionary<float, int>();

    // This is the calibration data we will load
    private Dictionary<float, float> calibrationData;

    // --- Internal Test Logic Variables ---
    private int currentFreqIndex = 0;
    private int currentTestHL = 20;
    private int ascendingResponses = 0;
    private bool isWaitingForResponse = false;
    private Coroutine responseTimer;

    // The frequencies to test (MUST match calibration)
    private float[] frequenciesToTest = { 1000, 2000, 4000, 8000, 500, 250 };

    void OnEnable()
    {
        // 1. GET THE CALIBRATION DATA!
        if (StaticDataAndHelpers.thresholds_dBFS == null)
        {
            statusText.text = "ERROR: Calibration data not found!";
            return;
        }

        calibrationData = StaticDataAndHelpers.thresholds_dBFS;

        // 2. Setup the test
        heardButton.onClick.AddListener(OnUserHeard);
        noResponseButton.onClick.AddListener(OnUserDidNotHear);
        audiogramResults.Clear();

        // 3. Start the test
        StartCoroutine(RunTest());
    }

    void OnDisable()
    {
        if (toneGenerator != null)
        {
            toneGenerator.audioSource.Stop();
        }
        StopAllCoroutines();

        if (heardButton != null) heardButton.onClick.RemoveListener(OnUserHeard);
        if (noResponseButton != null) noResponseButton.onClick.RemoveListener(OnUserDidNotHear);
    }

    // This is the main test loop
    IEnumerator RunTest()
    {
        for (currentFreqIndex = 0; currentFreqIndex < frequenciesToTest.Length; currentFreqIndex++)
        {
            float freq = frequenciesToTest[currentFreqIndex];
            statusText.text = $"Testing: {freq} Hz";

            if (!calibrationData.ContainsKey(freq) || float.IsPositiveInfinity(calibrationData[freq]))
            {
                Debug.Log($"Skipping {freq} Hz (No Response in calibration).");
                audiogramResults[freq] = -1; // -1 means "NR"
                continue;
            }

            // Reset logic for this frequency
            currentTestHL = 20;
            ascendingResponses = 0;

            // Run the "10-down, 5-up" logic
            while (true)
            {
                float zero_dB_HL = calibrationData[freq];
                float testLevel_dBFS = zero_dB_HL + currentTestHL;

                // Safety Check
                if (testLevel_dBFS > 0)
                {
                    testLevel_dBFS = 0;
                    Debug.LogWarning($"Reached max volume for {freq} Hz.");
                    audiogramResults[freq] = -1; // Mark as No Response
                    break;
                }

                // 1. SET TONE
                toneGenerator.frequency = freq;
                toneGenerator.gain = testLevel_dBFS;

                // 2. PLAY TONE
                toneGenerator.audioSource.Play();
                yield return new WaitForSeconds(1.0f);
                toneGenerator.audioSource.Stop();

                // 3. WAIT FOR RESPONSE
                isWaitingForResponse = true;
                responseTimer = StartCoroutine(ResponseTimer());
                while (isWaitingForResponse)
                {
                    yield return null;
                }

                // 4. CHECK FOR THRESHOLD
                if (ascendingResponses >= 2)
                {
                    Debug.Log($"Threshold for {freq} Hz found: {currentTestHL} dB HL");
                    audiogramResults[freq] = currentTestHL;
                    break; // Found it! Move to next frequency
                }
            }
        }

        FinishTest();
    }

    IEnumerator ResponseTimer()
    {
        yield return new WaitForSeconds(2.0f);
        if (isWaitingForResponse)
        {
            isWaitingForResponse = false;
            OnUserMissed();
        }
    }

    void OnUserHeard()
    {
        if (!isWaitingForResponse) return;
        isWaitingForResponse = false;
        StopCoroutine(responseTimer);

        ascendingResponses++;
        currentTestHL -= 10; // 10-DOWN
    }

    public void OnUserDidNotHear()
    {
        if (!isWaitingForResponse) return;
        isWaitingForResponse = false;
        StopCoroutine(responseTimer);

        OnUserMissed();
    }

    void OnUserMissed()
    {
        ascendingResponses = 0; // Reset
        currentTestHL += 5; // 5-UP
    }

    void FinishTest()
    {
        Debug.Log("--- TEST COMPLETE ---");
        statusText.text = "Test Complete!";

        // --- THIS IS THE CRITICAL UPDATE ---

        // 1. Save the final results to our static class
        // We are saving a *new copy* just to be safe.
        StaticDataAndHelpers.audiogramResults = new Dictionary<float, int>(this.audiogramResults);

        // 2. Tell the UI manager to move to the results screen
        if (uiManager != null)
        {
            uiManager.ShowResultsMenu();
        }
        else
        {
            Debug.LogError("UIManager is not assigned on AudiometryTest!");
        }

        // We no longer need this debug log, but you can keep it
        foreach (var result in audiogramResults)
        {
            string threshold = (result.Value == -1) ? "No Response" : result.Value + " dB HL";
            Debug.Log($"Result: {result.Key} Hz = {threshold}");
        }
    }
}