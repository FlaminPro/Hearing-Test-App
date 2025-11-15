using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum TestEar { Left, Right, Done }

public class AudiometryTest : MonoBehaviour
{
    [Header("Script References")]
    public AudioWaves toneGenerator;
    public UI uiManager;

    [Header("UI Elements")]
    public Button heardButton;
    public Button noResponseButton;
    public TMP_Text statusText;

    // --- UPDATED ---
    // These will store the results for each ear
    private Dictionary<float, int> audiogramResultsLeft = new Dictionary<float, int>();
    private Dictionary<float, int> audiogramResultsRight = new Dictionary<float, int>();
    private TestEar currentEar = TestEar.Left; // Start with the left ear

    private Dictionary<float, float> calibrationData;
    private int currentFreqIndex = 0;
    private int currentTestHL = 20;
    private int ascendingResponses = 0;
    private bool isWaitingForResponse = false;
    private Coroutine responseTimer;
    private float[] frequenciesToTest = { 1000, 2000, 4000, 8000, 500, 250 };

    void OnEnable()
    {
        if (StaticDataAndHelpers.thresholds_dBFS == null)
        {
            statusText.text = "ERROR: Calibration data not found!";
            return;
        }

        calibrationData = StaticDataAndHelpers.thresholds_dBFS;

        heardButton.onClick.AddListener(OnUserHeard);
        noResponseButton.onClick.AddListener(OnUserDidNotHear);

        // Start the test coroutine
        StartCoroutine(RunFullTestBattery());
    }

    void OnDisable()
    {
        if (toneGenerator != null) { toneGenerator.audioSource.Stop(); }
        StopAllCoroutines();
        if (heardButton != null) heardButton.onClick.RemoveListener(OnUserHeard);
        if (noResponseButton != null) noResponseButton.onClick.RemoveListener(OnUserDidNotHear);
    }

    // --- NEW ---
    // This coroutine manages the entire test battery (Left ear, then Right ear)
    IEnumerator RunFullTestBattery()
    {
        // 1. TEST LEFT EAR
        currentEar = TestEar.Left;
        toneGenerator.pan = -1.0f; // Pan hard left
        audiogramResultsLeft.Clear();
        statusText.text = "Testing: LEFT EAR";
        yield return new WaitForSeconds(2.0f); // Give user time to read
        yield return StartCoroutine(RunTestForCurrentEar());

        // 2. TEST RIGHT EAR
        currentEar = TestEar.Right;
        toneGenerator.pan = 1.0f; // Pan hard right
        audiogramResultsRight.Clear();
        statusText.text = "Testing: RIGHT EAR";
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(RunTestForCurrentEar());

        // 3. FINISH
        currentEar = TestEar.Done;
        FinishTest();
    }

    // This is the main test loop, now for whichever ear is 'currentEar'
    IEnumerator RunTestForCurrentEar()
    {
        for (currentFreqIndex = 0; currentFreqIndex < frequenciesToTest.Length; currentFreqIndex++)
        {
            float freq = frequenciesToTest[currentFreqIndex];
            statusText.text = $"Testing {currentEar} Ear: {freq} Hz";

            if (!calibrationData.ContainsKey(freq) || float.IsPositiveInfinity(calibrationData[freq]))
            {
                RecordResult(-1); // -1 means "NR"
                continue;
            }

            currentTestHL = 20;
            ascendingResponses = 0;

            while (true)
            {
                float zero_dB_HL = calibrationData[freq];
                float testLevel_dBFS = zero_dB_HL + currentTestHL;

                if (testLevel_dBFS > 0)
                {
                    testLevel_dBFS = 0;
                    RecordResult(-1);
                    break;
                }

                toneGenerator.frequency = freq;
                toneGenerator.gain = testLevel_dBFS;

                toneGenerator.audioSource.Play();
                yield return new WaitForSeconds(1.0f);
                toneGenerator.audioSource.Stop();

                isWaitingForResponse = true;
                responseTimer = StartCoroutine(ResponseTimer());
                while (isWaitingForResponse) { yield return null; }

                if (ascendingResponses >= 2)
                {
                    RecordResult(currentTestHL);
                    break;
                }
            }
        }
    }

    // --- NEW HELPER ---
    // Records the result to the correct ear's dictionary
    void RecordResult(int threshold)
    {
        float freq = frequenciesToTest[currentFreqIndex];
        if (currentEar == TestEar.Left)
        {
            audiogramResultsLeft[freq] = threshold;
        }
        else
        {
            audiogramResultsRight[freq] = threshold;
        }
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
        currentTestHL -= 10;
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
        ascendingResponses = 0;
        currentTestHL += 5;
    }

    void FinishTest()
    {
        Debug.Log("--- TEST COMPLETE ---");
        statusText.text = "Test Complete!";
        toneGenerator.pan = 0f; // Reset pan to center

        // Save both results dictionaries
        StaticDataAndHelpers.audiogramResultsLeft = this.audiogramResultsLeft;
        StaticDataAndHelpers.audiogramResultsRight = this.audiogramResultsRight;

        if (uiManager != null)
        {
            uiManager.ShowResultsMenu();
        }
    }
}