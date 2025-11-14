using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject QuestionnaireMenu;
    [SerializeField] GameObject CalibrationMenu;
    [SerializeField] GameObject TestMenu;
    [SerializeField] GameObject ResultsMenu;

    [Header("Script References")]
    [SerializeField] ResultsManager resultsManager;

    private void Awake()
    {
        StaticDataAndHelpers.Init();
        MainMenu.SetActive(true);
        QuestionnaireMenu.SetActive(false);
        CalibrationMenu.SetActive(false);
        TestMenu.SetActive(false);
        ResultsMenu.SetActive(false);
    }

    // 1. Called by "START TEST" on MainMenu
    public void StartQuestionnaire()
    {
        StaticDataAndHelpers.patientHistory = new PatientHistory();
        MainMenu.SetActive(false);
        QuestionnaireMenu.SetActive(true);
    }

    // 2. Called by "Back" on QuestionnaireMenu
    public void QuestionnaireBackBTN()
    {
        MainMenu.SetActive(true);
        QuestionnaireMenu.SetActive(false);
    }

    // 3. Called from QuestionnaireManager.cs
    public void CalibrateMenu()
    {
        QuestionnaireMenu.SetActive(false);
        CalibrationMenu.SetActive(true);
    }

    // 4. Called by "Back" on CalibrationMenu
    public void CalibrateBackBTN()
    {
        QuestionnaireMenu.SetActive(true);
        CalibrationMenu.SetActive(false);
    }

    // 5. Called from HearingTestManager.cs
    public void TestMenuFunc()
    {
        if (StaticDataAndHelpers.thresholds_dBFS == null) { Debug.LogError("No Calibration Data"); }
        else
        {
            CalibrationMenu.SetActive(false);
            TestMenu.SetActive(true);
        }
    }

    // 6. Called by "Back" on TestMenu
    public void TestMenuBackFunc()
    {
        CalibrationMenu.SetActive(true);
        TestMenu.SetActive(false);
    }

    // 7. Called from AudiometryTest.cs
    public void ShowResultsMenu()
    {
        TestMenu.SetActive(false);
        ResultsMenu.SetActive(true);

        if (resultsManager != null)
        {
            resultsManager.DisplayResults();
        }
    }

    // 8. Called by "Menu" on ResultsMenu
    public void ReturnToMainMenu()
    {
        MainMenu.SetActive(true);
        QuestionnaireMenu.SetActive(false);
        CalibrationMenu.SetActive(false);
        TestMenu.SetActive(false);
        ResultsMenu.SetActive(false);
    }

    // 9. <-- NEW FUNCTION -->
    // Called by "Retest" on ResultsMenu
    public void Retest()
    {
        // Go back to the questionnaire and clear old data
        StaticDataAndHelpers.patientHistory = new PatientHistory();

        ResultsMenu.SetActive(false);
        QuestionnaireMenu.SetActive(true);
    }
}