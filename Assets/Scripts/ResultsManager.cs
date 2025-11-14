using System; // <-- Required for DateTime
using System.Collections.Generic;
using System.IO; // <-- Required for file saving
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This class will hold a risk factor's details
public class RiskFactor
{
    public string factor;
    public string severity; // "high", "moderate", "low"
    public string description;
}

public class ResultsManager : MonoBehaviour
{
    [Header("Manager Reference")]
    public UI uiManager; // <-- NEW: Drag your UI script here

    [Header("UI Text Elements")]
    public TMP_Text hearingStatusText;
    public TMP_Text averageThresholdText;
    public TMP_Text riskFactorsText;
    public TMP_Text recommendationsText;
    public TMP_Text downloadStatusText;

    [Header("UI Buttons")]
    public Button downloadReportButton; // <-- NEW
    public Button retestButton; // <-- NEW
    public Button menuButton; // <-- NEW (for the 'ReturnToMainMenu' button)

    [Header("Audiogram Chart")]
    public RectTransform audiogramChartArea;
    public List<RectTransform> audiogramDots; // 6 dots (250, 500, 1000, 2000, 4000, 8000)

    // --- Private Data ---
    private PatientHistory history;
    private Dictionary<float, int> results;
    private float[] frequencies = { 250, 500, 1000, 2000, 4000, 8000 };
    private string hearingStatus = "NORMAL";
    private float averageThreshold = 0;
    private List<RiskFactor> currentRisks = new List<RiskFactor>();
    private List<string> currentRecs = new List<string>();

    void Start()
    {
        if (downloadReportButton != null)
            downloadReportButton.onClick.AddListener(OnDownloadReport);

        // --- NEW ---
        if (retestButton != null && uiManager != null)
            retestButton.onClick.AddListener(uiManager.Retest);

        if (menuButton != null && uiManager != null)
            menuButton.onClick.AddListener(uiManager.ReturnToMainMenu);
    }

    // This is called by UI.cs when the panel is shown
    public void DisplayResults()
    {
        // 1. Get the data
        this.history = StaticDataAndHelpers.patientHistory;
        this.results = StaticDataAndHelpers.audiogramResults;

        if (this.history == null || this.results == null)
        {
            hearingStatusText.text = "ERROR: Data not found.";
            return;
        }

        // 2. Process data
        averageThreshold = CalculateAverageThreshold();
        hearingStatus = GetHearingStatus(averageThreshold);
        currentRisks = GetRiskFactors();
        currentRecs = GetRecommendations(averageThreshold, currentRisks);

        // 3. Display data
        hearingStatusText.text = $"YOUR HEARING IS: {hearingStatus}";
        averageThresholdText.text = $"Average threshold: {averageThreshold:F1} dB HL";

        // Build and set the text for risks and recommendations
        StringBuilder sbRisks = new StringBuilder();
        foreach (var risk in currentRisks)
        {
            sbRisks.AppendLine($"<b>[{risk.severity.ToUpper()}] {risk.factor}</b>");
            sbRisks.AppendLine($"<size=24><i>{risk.description}</i></size>\n");
        }
        riskFactorsText.text = currentRisks.Count > 0 ? sbRisks.ToString() : "No significant risk factors identified.";

        StringBuilder sbRecs = new StringBuilder();
        foreach (var rec in currentRecs)
        {
            sbRecs.AppendLine($"• {rec}");
        }
        recommendationsText.text = sbRecs.ToString();

        // 4. Plot the audiogram
        PlotAudiogram();

        // 5. Clear old download status
        if (downloadStatusText != null)
        {
            downloadStatusText.text = "";
        }
    }

    // --- (All calculation functions: PlotAudiogram, CalculateAverageThreshold, etc. remain the same) ---
    // (Omitted for brevity, paste them in from the previous answer)

    // --- NEW FUNCTION ---
    // This is called by the `downloadReportButton`
    public void OnDownloadReport()
    {
        Debug.Log("Generating report...");

        // 1. Build the report string (matches Results.tsx)
        string report = BuildReportString();

        // 2. Define a file path
        string fileName = $"Hearing_Test_{history.name.Replace(" ", "_")}_{DateTime.Now:yyyy-MM-dd}.txt";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            // 3. Save the file
            File.WriteAllText(filePath, report);

            // 4. Notify the user
            Debug.Log($"Report saved to: {filePath}");
            if (downloadStatusText != null)
            {
                downloadStatusText.text = $"Report saved to:\n{filePath}";
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save report: {e.Message}");
            if (downloadStatusText != null)
            {
                downloadStatusText.text = "Error: Could not save report.";
            }
        }
    }

    // --- NEW HELPER FUNCTION ---
    // This function replicates the string building from Results.tsx
    private string BuildReportString()
    {
        StringBuilder report = new StringBuilder();
        string divider = "???????????????????????????????????????????????????????\n";

        report.AppendLine(divider);
        report.AppendLine("HEARING TEST REPORT");
        report.AppendLine(divider);
        report.AppendLine("\nPATIENT INFORMATION");
        report.AppendLine("???????????????????????????????????????????????????????");
        report.AppendLine($"Name:           {history.name}");
        report.AppendLine($"Age:            {history.age}");
        report.AppendLine($"Gender:         {history.gender}");
        report.AppendLine($"Test Date:      {DateTime.Now:MMMM dd, yyyy}");

        report.AppendLine($"\n{divider}");
        report.AppendLine("TEST RESULTS SUMMARY");
        report.AppendLine(divider);
        report.AppendLine($"Hearing Status:       {hearingStatus}");
        report.AppendLine($"Average Threshold:    {averageThreshold:F1} dB HL\n");

        report.AppendLine("\nDETAILED AUDIOGRAM DATA");
        report.AppendLine("???????????????????????????????????????????????????????");
        foreach (float freq in frequencies)
        {
            string thresholdStr = "No Response";
            if (results.ContainsKey(freq) && results[freq] != -1)
            {
                thresholdStr = $"{results[freq]} dB HL";
            }
            report.AppendLine($"  {freq} Hz: {thresholdStr}");
        }

        if (currentRisks.Count > 0)
        {
            report.AppendLine($"\n{divider}");
            report.AppendLine("IDENTIFIED RISK FACTORS");
            report.AppendLine(divider);
            foreach (var risk in currentRisks)
            {
                report.AppendLine($"• [{risk.severity.ToUpper()}] {risk.factor}");
                report.AppendLine($"   <i>{risk.description}</i>\n");
            }
        }

        if (currentRecs.Count > 0)
        {
            report.AppendLine($"\n{divider}");
            report.AppendLine("PERSONALIZED RECOMMENDATIONS");
            report.AppendLine(divider);
            foreach (var rec in currentRecs)
            {
                report.AppendLine($"• {rec}");
            }
        }

        report.AppendLine($"\n{divider}");
        report.AppendLine("IMPORTANT NOTICE");
        report.AppendLine(divider);
        report.AppendLine("This is a screening test and not a substitute for a professional audiological evaluation.");

        return report.ToString();
    }


    // --- PASTE ALL FUNCTIONS FROM PREVIOUS ANSWER HERE ---
    // (CalculateAverageThreshold, GetHearingStatus, PlotAudiogram, GetRiskFactors, GetRecommendations)

    float CalculateAverageThreshold()
    {
        float total = 0;
        int count = 0;
        foreach (var freq in frequencies)
        {
            if (results.ContainsKey(freq) && results[freq] != -1) // -1 is "NR"
            {
                total += results[freq];
                count++;
            }
        }
        return (count == 0) ? 0 : (total / count);
    }

    string GetHearingStatus(float avg)
    {
        if (avg < 25) return "NORMAL";
        if (avg < 40) return "MILD LOSS";
        if (avg < 60) return "MODERATE LOSS";
        return "SIGNIFICANT LOSS";
        // You can set hearingStatusText.color here too
    }

    void PlotAudiogram()
    {
        if (audiogramChartArea == null || audiogramDots == null || audiogramDots.Count != frequencies.Length)
        {
            Debug.LogWarning("Audiogram UI not fully set up. Skipping plot.");
            return;
        }

        float chartHeight = audiogramChartArea.rect.height; // e.g., 500px

        // Assuming your chart goes from 0 dB (top) to 100 dB (bottom)
        float maxDB = 100f;
        float minDB = 0f;

        for (int i = 0; i < frequencies.Length; i++)
        {
            float freq = frequencies[i];
            if (results.ContainsKey(freq) && results[freq] != -1)
            {
                float dbValue = results[freq];

                // Calculate Y position
                // 1.0 is top, 0.0 is bottom.
                float y_percent = 1.0f - Mathf.InverseLerp(minDB, maxDB, dbValue);

                // Convert percentage to local Y position
                float y_pos = (y_percent * chartHeight) - (chartHeight / 2f); // Adjust for pivot

                // Set the dot's position
                // We only move it vertically (anchored Y)
                audiogramDots[i].anchoredPosition = new Vector2(
                    audiogramDots[i].anchoredPosition.x, // Keep its X position
                    y_pos
                );
                audiogramDots[i].gameObject.SetActive(true);
            }
            else
            {
                // Hide the dot if no result
                audiogramDots[i].gameObject.SetActive(false);
            }
        }
    }

    List<RiskFactor> GetRiskFactors()
    {
        var risks = new List<RiskFactor>();
        if (history.age >= 60)
        {
            risks.Add(new RiskFactor { factor = "Age-related hearing loss (Presbycusis)", severity = "moderate", description = "Natural hearing decline typically begins around age 60" });
        }
        if (history.noiseExposure.occupational || history.noiseExposure.military || (history.noiseExposure.recreational && (history.noiseExposure.duration == "5-10" || history.noiseExposure.duration == "10+")))
        {
            risks.Add(new RiskFactor { factor = "Noise-induced hearing loss", severity = "high", description = "Prolonged exposure to loud noise is a major risk factor" });
        }
        if (history.medicalConditions.diabetes)
        {
            risks.Add(new RiskFactor { factor = "Diabetes", severity = "moderate", description = "Diabetes can damage blood vessels in the inner ear" });
        }
        if (history.medicalConditions.cardiovascular || history.medicalConditions.hypertension)
        {
            risks.Add(new RiskFactor { factor = "Cardiovascular disease", severity = "moderate", description = "Reduced blood flow can affect hearing" });
        }
        if (history.ototoxicMedications.current)
        {
            risks.Add(new RiskFactor { factor = "Current ototoxic medications", severity = "high", description = "Some medications can cause temporary or permanent hearing damage" });
        }
        if (history.earHistory.suddenHearingLoss)
        {
            risks.Add(new RiskFactor { factor = "Previous sudden hearing loss", severity = "high", description = "Requires immediate medical attention and monitoring" });
        }
        if (history.earHistory.infections == "chronic")
        {
            risks.Add(new RiskFactor { factor = "Chronic ear infections", severity = "moderate", description = "Can cause conductive hearing loss" });
        }
        if (history.symptoms.tinnitus && (history.symptoms.tinnitusFrequency == "frequently" || history.symptoms.tinnitusFrequency == "constant"))
        {
            risks.Add(new RiskFactor { factor = "Persistent tinnitus", severity = "moderate", description = "Often associated with hearing loss" });
        }
        if (history.symptoms.dizziness)
        {
            risks.Add(new RiskFactor { factor = "Balance problems", severity = "moderate", description = "May indicate inner ear dysfunction" });
        }
        if (history.familyHistory)
        {
            risks.Add(new RiskFactor { factor = "Family history", severity = "low", description = "Genetic predisposition to hearing loss" });
        }
        return risks;
    }

    List<string> GetRecommendations(float avg, List<RiskFactor> risks)
    {
        var recs = new List<string>();
        bool hasHighRisk = risks.Exists(r => r.severity == "high");

        if (avg >= 40)
        {
            recs.Add("Consult an audiologist or ENT specialist for comprehensive evaluation");
            recs.Add("Consider hearing aid evaluation");
        }
        else if (avg >= 25)
        {
            recs.Add("Schedule follow-up hearing test in 6-12 months");
            recs.Add("Consult with an audiologist for professional assessment");
        }

        if (history.noiseExposure.occupational || history.noiseExposure.recreational)
        {
            recs.Add("Use hearing protection (earplugs/earmuffs) in loud environments");
        }
        if (history.medicalConditions.diabetes || history.medicalConditions.cardiovascular)
        {
            recs.Add("Manage underlying health conditions to protect hearing");
        }
        if (history.ototoxicMedications.current)
        {
            recs.Add("Discuss hearing monitoring plan with prescribing physician");
        }
        if (history.earHistory.suddenHearingLoss)
        {
            recs.Add("Seek immediate medical attention for any new sudden hearing changes");
        }
        if (history.symptoms.earPain || history.symptoms.earFullness)
        {
            recs.Add("Consult ENT specialist to rule out infection or blockage");
        }
        if (avg < 25 && !hasHighRisk)
        {
            recs.Add("Continue protecting your hearing from loud noise exposure");
            recs.Add("Have your hearing tested every 3-5 years");
        }
        return recs;
    }
}