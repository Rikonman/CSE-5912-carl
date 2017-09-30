using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
	public Toggle fullscreenToggle;
	public Dropdown resolutionDropdown;
	public Dropdown qualityDropdown;
	public Dropdown antiAliasingDropdown;
	public Slider musicVolumeSlider;
	public Slider sfxVolumeSlider;

	private AudioSource musicAudioSource;
	private AudioSource sfxAudioSource;

	private Resolution[] resolutions;

	void OnEnable()
    {
        // Get a reference to the audio sources so we can change them upon moving the volume sliders
        musicAudioSource = GameObject.Find("MusicAudioSource").GetComponent<AudioSource>();
        sfxAudioSource = GameObject.Find("SfxAudioSource").GetComponent<AudioSource>();

        // Load the current settings into the settings controls
        antiAliasingDropdown.value = GameSettings.AntiAliasing;
		qualityDropdown.value = GameSettings.Quality;
        fullscreenToggle.isOn = GameSettings.Fullscreen;
		musicVolumeSlider.value = GameSettings.MusicVolume;
        sfxVolumeSlider.value = GameSettings.SfxVolume;

        // Set the resolution options in the resolution dropdown box
		resolutions = Screen.resolutions;
        resolutionDropdown.options.Clear();
        foreach(Resolution item in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(item.ToString()));
        }

        // Select the current resolution in the resolution dropbox
        resolutionDropdown.value = GameSettings.ResolutionIndex;

        // Set the events for value changes
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        qualityDropdown.onValueChanged.AddListener(delegate { OnQualityChange(); });
        antiAliasingDropdown.onValueChanged.AddListener(delegate { OnAntiAliasingChange(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { OnSfxVolumeChange(); });
    }

	public void OnFullscreenToggle()
	{
		GameSettings.Fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
	}

	public void OnResolutionChange()
	{
        GameSettings.ResolutionIndex = resolutionDropdown.value;
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
	}

	public void OnQualityChange()
	{
        QualitySettings.masterTextureLimit = GameSettings.Quality = qualityDropdown.value;
	}

	public void OnAntiAliasingChange()
	{
        QualitySettings.antiAliasing = GameSettings.AntiAliasing = (int)Mathf.Pow(2f, antiAliasingDropdown.value);
	}

	public void OnMusicVolumeChange()
	{
		musicAudioSource.volume = GameSettings.MusicVolume = musicVolumeSlider.value;
	}

	public void OnSfxVolumeChange()
	{
        sfxAudioSource.volume = GameSettings.SfxVolume = sfxVolumeSlider.value;
	}
}
