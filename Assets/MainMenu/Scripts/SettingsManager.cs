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
	public Slider soundEffectsVolumeSlider;
	public AudioSource musicAudioSource;
	public AudioSource sfxAudioSource;

	private Resolution[] resolutions;

	void OnEnable()
	{
		fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
		resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
		qualityDropdown.onValueChanged.AddListener(delegate { OnQualityChange(); });
		antiAliasingDropdown.onValueChanged.AddListener(delegate { OnAntiAliasingChange(); });
		musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
		soundEffectsVolumeSlider.onValueChanged.AddListener(delegate { OnSfxVolumeChange(); });

        GameSettings.AntiAliasing = antiAliasingDropdown.value = GetCurrentAntiAliasingSetting();
		GameSettings.Quality = qualityDropdown.value = QualitySettings.masterTextureLimit;
        GameSettings.Fullscreen = fullscreenToggle.isOn = Screen.fullScreen;
		musicVolumeSlider.value = musicAudioSource.volume;
        soundEffectsVolumeSlider.value = GameSettings.SfxVolume;

        // Set the resolution options in the resolution dropdown box
		resolutions = Screen.resolutions;
        resolutionDropdown.options.Clear();
        foreach(Resolution item in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(item.ToString()));
        }

        // Set the current resolution...this isn't exactly the most efficient way of doing this
        // but at least it works.
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].Equals(Screen.currentResolution))
            {
                resolutionDropdown.value = i;
                break;
            }
        }
	}

	public void OnFullscreenToggle()
	{
		GameSettings.Fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
	}

	public void OnResolutionChange()
	{
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
        sfxAudioSource.volume = GameSettings.SfxVolume = soundEffectsVolumeSlider.value;
	}

    private int GetCurrentAntiAliasingSetting()
    {
        int output = 0;

        for (int i = 0; i < 3; i++)
        {
            if (QualitySettings.antiAliasing == (int)Mathf.Pow(2f, i))
            {
                output = i;
                break;
            }
        }

        return output;
    }
}
