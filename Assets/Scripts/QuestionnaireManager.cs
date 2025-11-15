using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestionnaireManager : MonoBehaviour
{
    [Header("Manager Reference")]
    public UI uiManager; // Drag your UI script here

    [Header("Step 1: Basic Info")]
    public TMP_InputField nameInput;
    public InputField ageInput;
    public TMP_Dropdown genderDropdown; // Options: Male, Female, Other...

    [Header("Step 2: Noise Exposure")]
    public Toggle occupationalNoiseToggle;
    public Toggle recreationalNoiseToggle;
    public Toggle militaryNoiseToggle;
    public TMP_Dropdown noiseDurationDropdown; // Options: <1, 1-5, 5-10, 10+

    [Header("Step 3: Medical History")]
    public Toggle diabetesToggle;
    public Toggle cardiovascularToggle;
    public Toggle hypertensionToggle;
    public Toggle autoimmuneToggle;
    public Toggle currentMedsToggle;
    public Toggle pastMedsToggle;
    // For "medTypes", you could use a group of toggles

    [Header("Step 4: Ear History")]
    public TMP_Dropdown infectionsDropdown; // Options: none, childhood, recent, chronic
    public Toggle earSurgeryToggle;
    public Toggle earTraumaToggle;
    public Toggle suddenLossToggle;

    [Header("Step 5: Symptoms")]
    public Toggle tinnitusToggle;
    public TMP_Dropdown tinnitusFrequencyDropdown; // Options: rarely, occasionally...
    public Toggle dizzinessToggle;
    public Toggle earPainToggle;
    public Toggle earFullnessToggle;
    public Toggle familyHistoryToggle;
    public Toggle hearingAidUseToggle;

    // This is called by the "Complete & Continue" button
    public void OnContinueButton()
    {
        // 1. Get the static patientHistory object
        PatientHistory history = StaticDataAndHelpers.patientHistory;

        // 2. Save all data from the UI into the object

        // Step 1
        history.name = nameInput.text;
        int.TryParse(ageInput.text, out history.age);
        history.gender = genderDropdown.options[genderDropdown.value].text;

        // Step 2
        history.noiseExposure.occupational = occupationalNoiseToggle.isOn;
        history.noiseExposure.recreational = recreationalNoiseToggle.isOn;
        history.noiseExposure.military = militaryNoiseToggle.isOn;
        history.noiseExposure.duration = noiseDurationDropdown.options[noiseDurationDropdown.value].text;

        // Step 3
        history.medicalConditions.diabetes = diabetesToggle.isOn;
        history.medicalConditions.cardiovascular = cardiovascularToggle.isOn;
        history.medicalConditions.hypertension = hypertensionToggle.isOn;
        history.medicalConditions.autoimmune = autoimmuneToggle.isOn;
        history.ototoxicMedications.current = currentMedsToggle.isOn;
        history.ototoxicMedications.past = pastMedsToggle.isOn;
        // (Add logic for medTypes if you implemented them)

        // Step 4
        history.earHistory.infections = infectionsDropdown.options[infectionsDropdown.value].text;
        history.earHistory.surgery = earSurgeryToggle.isOn;
        history.earHistory.trauma = earTraumaToggle.isOn;
        history.earHistory.suddenHearingLoss = suddenLossToggle.isOn;

        // Step 5
        history.symptoms.tinnitus = tinnitusToggle.isOn;
        history.symptoms.tinnitusFrequency = tinnitusFrequencyDropdown.options[tinnitusFrequencyDropdown.value].text;
        history.symptoms.dizziness = dizzinessToggle.isOn;
        history.symptoms.earPain = earPainToggle.isOn;
        history.symptoms.earFullness = earFullnessToggle.isOn;
        history.familyHistory = familyHistoryToggle.isOn;
        history.hearingAidUse = hearingAidUseToggle.isOn;

        // 3. Log for debugging
        Debug.Log("Patient History Saved. Age: " + history.age + ", Gender: " + history.gender);

        // 4. Move to the next screen
        if (uiManager != null)
        {
            uiManager.CalibrateMenu();
        }
        else
        {
            Debug.LogError("UIManager not set on QuestionnaireManager!");
        }
    }
}