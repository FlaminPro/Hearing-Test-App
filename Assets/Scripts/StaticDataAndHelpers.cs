using UnityEngine;
using System.Collections.Generic;

public static class StaticDataAndHelpers
{
    // --- NEW ---
    // This will hold the completed questionnaire data
    public static PatientHistory patientHistory;

    // --- NEW ---
    // This will hold the final test results
    public static Dictionary<float, int> audiogramResults;

    // --- OLD ---
    public static Dictionary<int, Dictionary<int, int>> AverageHearingDataMales { get; private set; }
    public static Dictionary<int, Dictionary<int, int>> AverageHearingDataFemales { get; private set; }
    public static Dictionary<float, float> thresholds_dBFS; // This is the CALIBRATION data

    public static void Init()
    {
        // Initialize new, empty objects when the app starts
        patientHistory = new PatientHistory();
        audiogramResults = new Dictionary<float, int>();

        if (AverageHearingDataMales != null || AverageHearingDataFemales != null) return;

        // (rest of your Init() data remains unchanged)
        AverageHearingDataMales = new Dictionary<int, Dictionary<int, int>>() {
            {250,  new Dictionary<int, int> () { {20,5}, {30,5}, {40,6}, {50,8}, {60,12}, {70,18} }},
            {500,  new Dictionary<int, int> () { {20,5}, {30,5}, {40,7}, {50,9}, {60,14}, {70,20} }},
            {1000,  new Dictionary<int, int> () { {20,5}, {30,6}, {40,8}, {50,12}, {60,18}, {70,25} }},
            {2000,  new Dictionary<int, int> () { {20,6}, {30,8}, {40,12}, {50,20}, {60,30}, {70,40} }},
            {4000,  new Dictionary<int, int> () { {20,8}, {30,12}, {40,20}, {50,35}, {60,50}, {70,65} }},
            {8000,  new Dictionary<int, int> () { {20,10}, {30,16}, {40,25}, {50,45}, {60,65}, {70,75} }}
        };
        AverageHearingDataFemales = new Dictionary<int, Dictionary<int, int>>() {
            {250,  new Dictionary<int, int> () { {20,5}, {30,5}, {40,6}, {50,8}, {60,13}, {70,19} }},
            {500,  new Dictionary<int, int> () { {20,5}, {30,5}, {40,7}, {50,9}, {60,15}, {70,22} }},
            {1000,  new Dictionary<int, int> () { {20,5}, {30,6}, {40,8}, {50,10}, {60,17}, {70,25} }},
            {2000,  new Dictionary<int, int> () { {20,5}, {30,7}, {40,10}, {50,14}, {60,22}, {70,32} }},
            {4000,  new Dictionary<int, int> () { {20,6}, {30,10}, {40,15}, {50,25}, {60,40}, {70,55} }},
            {8000,  new Dictionary<int, int> () { {20,8}, {30,14}, {40,20}, {50,38}, {60,55}, {70,70} }}
        };
    }

    public static int GetAgeGroupFromAge(int age)
    {
        // (this function is unchanged)
        int AgeGroup;
        if (age > 0 && age <= 30) AgeGroup = 20;
        else if (age < 40) AgeGroup = 30;
        else if (age < 50) AgeGroup = 40;
        else if (age < 60) AgeGroup = 50;
        else if (age < 70) AgeGroup = 60;
        else if (age >= 70) AgeGroup = 70;
        else AgeGroup = int.MinValue;
        return AgeGroup;
    }
}