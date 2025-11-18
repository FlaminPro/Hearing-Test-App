using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionnaireManager : MonoBehaviour
{
    [Header("Step 1: Basic Info")]
    public TMP_InputField nameInput;
    public InputField ageInput;
    public TMP_Dropdown genderDropdown;

    [Header("Step 2: Noise Exposure")]
    public Toggle occupationalNoiseToggle;
    public Toggle recreationalNoiseToggle;
    public Toggle militaryNoiseToggle;
    public TMP_Text DurationOfExposureText;
    public TMP_Dropdown noiseDurationDropdown;

    [Header("Step 3: Medical History")]
    public Toggle diabetesToggle;
    public Toggle cardiovascularToggle;
    public Toggle hypertensionToggle;
    public Toggle autoimmuneToggle;
    public Toggle currentMedsToggle;
    public Toggle pastMedsToggle;

    [Header("Step 4: Ear History")]
    public TMP_Dropdown infectionsDropdown;
    public Toggle earSurgeryToggle;
    public Toggle earTraumaToggle;
    public Toggle suddenLossToggle;

    [Header("Step 5: Symptoms")]
    public Toggle tinnitusToggle;
    public TMP_Text tinnitusFrequencyText;
    public TMP_Dropdown tinnitusFrequencyDropdown;
    public Toggle dizzinessToggle;
    public Toggle earPainToggle;
    public Toggle earFullnessToggle;
    public Toggle familyHistoryToggle;
    public Toggle hearingAidUseToggle;


    [Header("Steps Manager")]
    [SerializeField] GameObject[] Steps;
    [SerializeField] Button PreviousBTN;
    [SerializeField] Button NextBTN;
    [SerializeField] Button ContinueBTN;
    [SerializeField] TMP_Text StatusText;
    private int StepsArrayIndex = 0;

    public static QuestionnaireManager instance;

    private void Awake()
    {
        if (StepsArrayIndex == 0)
        {
            Steps[0].SetActive(true);
            Steps[1].SetActive(false);
            Steps[2].SetActive(false);
            Steps[3].SetActive(false);
            Steps[4].SetActive(false);
        }

        else if (StepsArrayIndex == 4)
        {
            Steps[0].SetActive(false);
            Steps[1].SetActive(false);
            Steps[2].SetActive(false);
            Steps[3].SetActive(false);
            Steps[4].SetActive(true);
        }

        //Singleton
        instance = this;

        //Events
        ContinueBTN.onClick.AddListener(ContinueBTN_OnClick);
    }

    private void Update()
    {
        QuestionaireFormWorking();
    }

    public void ContinueBTN_OnClick()
    {
        if (CheckForAnyEmptyField()) return;

        StatusText.text = string.Empty;
        PatientHistory history = StaticDataAndHelpers.patientHistory;

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

        Debug.Log("Patient History Saved. Age: " + history.age + ", Gender: " + history.gender);
        UI.instance.CalibrateMenu();
    }

    public void PreviousBTNFunc()
    {
        if (StepsArrayIndex > 0)
        {
            Steps[StepsArrayIndex].SetActive(false);
            StepsArrayIndex--;
            Steps[StepsArrayIndex].SetActive(true);
        }
    }

    public void NextBTNFunc()
    {
        if (StepsArrayIndex < 5)
        {
            Steps[StepsArrayIndex].SetActive(false);
            StepsArrayIndex++;
            Steps[StepsArrayIndex].SetActive(true);
        }
    }

    private void QuestionaireFormWorking()
    {
        //Steps Manager
        PreviousBTN.interactable = !(StepsArrayIndex == 0);
        NextBTN.interactable = !(StepsArrayIndex == 4);
        ContinueBTN.gameObject.SetActive(StepsArrayIndex == 4);

        //Step 2
        if (StepsArrayIndex == 1)
        {
            noiseDurationDropdown.gameObject.SetActive((occupationalNoiseToggle.isOn || recreationalNoiseToggle.isOn || militaryNoiseToggle.isOn));
            DurationOfExposureText.gameObject.SetActive((occupationalNoiseToggle.isOn || recreationalNoiseToggle.isOn || militaryNoiseToggle.isOn));
        }

        //Step5

        if (StepsArrayIndex == 4)
        {
            tinnitusFrequencyText.gameObject.SetActive(tinnitusToggle.isOn);
            tinnitusFrequencyDropdown.gameObject.SetActive(tinnitusToggle.isOn);
        }
    }

    public void ResetDefaults()
    {
        Steps[0].SetActive(true);
        Steps[1].SetActive(false);
        Steps[2].SetActive(false);
        Steps[3].SetActive(false);
        Steps[4].SetActive(false);
        StatusText.text = string.Empty;
        StepsArrayIndex = 0;
        //Reset Step 1
        nameInput.text = string.Empty;
        ageInput.text = string.Empty;
        genderDropdown.value = 0;
        genderDropdown.RefreshShownValue();

        //Reset Step 2
        occupationalNoiseToggle.isOn = false;
        recreationalNoiseToggle.isOn = false;
        militaryNoiseToggle.isOn = false;
        noiseDurationDropdown.value = 0;
        noiseDurationDropdown.RefreshShownValue();

        //Reset Step 3
        diabetesToggle.isOn = false;
        cardiovascularToggle.isOn = false;
        hypertensionToggle.isOn = false;
        autoimmuneToggle.isOn = false;
        currentMedsToggle.isOn = false;
        pastMedsToggle.isOn = false;

        //Reset Step 4
        infectionsDropdown.value = 0;
        infectionsDropdown.RefreshShownValue();
        earSurgeryToggle.isOn = false;
        earTraumaToggle.isOn = false;
        suddenLossToggle.isOn = false;

        //Reset Step 5
        tinnitusToggle.isOn = false;
        tinnitusFrequencyDropdown.value = 0;
        tinnitusFrequencyDropdown.RefreshShownValue();
        dizzinessToggle.isOn = false;
        earPainToggle.isOn = false;
        earFullnessToggle.isOn = false;
        familyHistoryToggle.isOn = false;
        hearingAidUseToggle.isOn = false;
    }

    private bool CheckForAnyEmptyField()
    {
        if (nameInput.text == string.Empty)
        {
            StatusText.text = "Name cannot be Empty";
            return true;
        }
        else if(ageInput.text == string.Empty)
        {
            StatusText.text = "Age cannot be Empty";
            return true;
        }
        else if((occupationalNoiseToggle.isOn || recreationalNoiseToggle.isOn || militaryNoiseToggle.isOn) && noiseDurationDropdown.value == 0)
        {
            StatusText.text = "Select a Noise Duration or Disable Any Checkboxes Related to It";
            return true;
        }
        else if(infectionsDropdown.value == 0)
        {
            StatusText.text = "Select infections History, if None Then Select No History Option";
            return true;
        }
        else if(tinnitusToggle.isOn && tinnitusFrequencyDropdown.value == 0)
        {
            StatusText.text = "Select a Tinnitus Frequency or Disable the Tinnitus Checkbox";
            return true;
        }
        else
        {
            return false;
        }
    }
}