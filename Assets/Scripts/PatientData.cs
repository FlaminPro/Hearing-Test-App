using System;
using System.Collections.Generic;


[Serializable]
public class NoiseExposureHistory
{
    public bool occupational;
    public bool recreational;
    public bool military;
    public string duration = "";
}

[Serializable]
public class MedicalConditions
{
    public bool diabetes;
    public bool cardiovascular;
    public bool hypertension;
    public bool autoimmune;
}

[Serializable]
public class OtotoxicMedications
{
    public bool current;
    public bool past;
    public List<string> types = new List<string>();
}

[Serializable]
public class EarHistory
{
    public string infections = "";
    public bool surgery;
    public bool trauma;
    public bool suddenHearingLoss;
}

[Serializable]
public class Symptoms
{
    public bool tinnitus;
    public string tinnitusFrequency = "";
    public bool dizziness;
    public bool earPain;
    public bool earFullness;
}


[Serializable]
public class PatientHistory
{
    public string name = "";
    public int age;
    public string gender = "";

    public NoiseExposureHistory noiseExposure = new NoiseExposureHistory();
    public MedicalConditions medicalConditions = new MedicalConditions();
    public OtotoxicMedications ototoxicMedications = new OtotoxicMedications();
    public EarHistory earHistory = new EarHistory();
    public Symptoms symptoms = new Symptoms();

    public bool familyHistory;
    public bool hearingAidUse;
}