using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button applyButton;

    private List<Resolution> resolutions;
    private int currentResolutionIndex;

    private void Start()
    {
        // Populate resolution dropdown and fullscreen toggle
        PopulateResolutionDropdown();
        fullscreenToggle.isOn = Screen.fullScreen;

        // Add listener to Apply button
        applyButton.onClick.AddListener(ApplySettings);
    }

    private void PopulateResolutionDropdown()
    {
        // Get available resolutions
        resolutions = new List<Resolution>(Screen.resolutions);
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        currentResolutionIndex = 0;

        // Add resolutions to the dropdown
        for (int i = 0; i < resolutions.Count; i++)
        {
            string resolutionOption = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(resolutionOption);

            // Check if this resolution matches the current screen resolution
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Update the dropdown
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void ApplySettings()
    {
        // Apply the selected resolution and fullscreen mode
        int selectedResolutionIndex = resolutionDropdown.value;
        Resolution selectedResolution = resolutions[selectedResolutionIndex];
        bool isFullscreen = fullscreenToggle.isOn;

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullscreen);
    }
}