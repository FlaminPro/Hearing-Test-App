using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    [Header("Menu Panels")]
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject QuestionnaireMenu;
    [SerializeField] GameObject CalibrationMenu;
    [SerializeField] GameObject TestMenu;
    [SerializeField] GameObject ResultsMenu;

    [Header("Script References")]
    [SerializeField] ResultsManager resultsManager;

    [Header("Buttons")]
    [SerializeField] Button StartBTN;

    private void Awake()
    {
        //Initialize Static Data
        StaticDataAndHelpers.Init();
        
        MainMenu.SetActive(true);
        QuestionnaireMenu.SetActive(false);
        CalibrationMenu.SetActive(false);
        TestMenu.SetActive(false);
        ResultsMenu.SetActive(false);
        
        //Singleton
        instance = this;

        //Events
        StartBTN.onClick.AddListener(StartBTN_OnClick);
    }

    #region MainMenu
    private void StartBTN_OnClick()
    {
        StaticDataAndHelpers.patientHistory = new PatientHistory();
        MainMenu.SetActive(false);
        QuestionnaireMenu.SetActive(true);
    }
    #endregion


    #region QuestionnaireMenu
    public void QuestionnaireBackBTN_OnClick()
    {
        QuestionnaireManager.instance.ResetDefaults();
        MainMenu.SetActive(true);
        QuestionnaireMenu.SetActive(false);
    }

    public void CalibrateMenu()
    {
        QuestionnaireMenu.SetActive(false);
        CalibrationMenu.SetActive(true);
    }

    #endregion

    #region CalibrationMenu

    public void CalibrateBackBTN()
    {
        QuestionnaireMenu.SetActive(true);
        CalibrationMenu.SetActive(false);
    }

    public void TestMenuFunc()
    {
        if (StaticDataAndHelpers.thresholds_dBFS == null) { Debug.LogError("No Calibration Data"); }
        else
        {
            CalibrationMenu.SetActive(false);
            TestMenu.SetActive(true);
        }
    }

    #endregion

    #region TestMenu
    public void TestMenuBackFunc()
    {
        CalibrationMenu.SetActive(true);
        TestMenu.SetActive(false);
    }

    public void ShowResultsMenu()
    {
        TestMenu.SetActive(false);
        ResultsMenu.SetActive(true);

        if (resultsManager != null)
        {
            resultsManager.DisplayResults();
        }
    }
    #endregion

    #region ResultsMenu
    public void ReturnToMainMenu()
    {
        MainMenu.SetActive(true);
        QuestionnaireMenu.SetActive(false);
        CalibrationMenu.SetActive(false);
        TestMenu.SetActive(false);
        ResultsMenu.SetActive(false);
    }

    public void Retest()
    {
        StaticDataAndHelpers.patientHistory = new PatientHistory();
        QuestionnaireManager.instance.ResetDefaults();
        ResultsMenu.SetActive(false);
        QuestionnaireMenu.SetActive(true);
    }
    #endregion
}