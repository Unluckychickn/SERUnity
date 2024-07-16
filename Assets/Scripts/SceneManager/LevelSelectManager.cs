using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{   [SerializeField] private RectTransform settingsUI;
    [SerializeField] private AudioSource testAudio;
    [SerializeField] private Slider generalVolumeSlider;
    [SerializeField] private Slider soundFXVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;

    
   public void StartEndlessRun()
   {
       SceneManager.LoadScene("Endless Runner");
   }

   public void Shop()
   {
        SceneManager.LoadScene("Shop");
   }

   public void Quit()
    {
            
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ShowSettings(){
        LoadSlider();
        settingsUI.gameObject.SetActive(true);
    }

    public void HideSettings(){
        settingsUI.gameObject.SetActive(false);
    }

    public void OnSliderValueChanged()
    {
        // Play the audio when the slider value changes
        if (testAudio != null && !testAudio.isPlaying)
        {
            testAudio.Play();
        }
    }
    private void LoadSlider()
    {
       
        if (audioMixer.GetFloat("MainMixer", out float generalVolume))
        {
            generalVolumeSlider.value = ConvertMixerValueToSliderValue(generalVolume);
            Debug.Log($"Loaded General Volume: {generalVolume}");
        }

        
        if (audioMixer.GetFloat("SoundMixer", out float soundFXVolume))
        {
            soundFXVolumeSlider.value = ConvertMixerValueToSliderValue(soundFXVolume);
            Debug.Log($"Loaded Sound FX Volume: {soundFXVolume}");
        }

        
        if (audioMixer.GetFloat("MusicMixer", out float musicVolume))
        {
            musicVolumeSlider.value = ConvertMixerValueToSliderValue(musicVolume);
            Debug.Log($"Loaded Music Volume: {musicVolume}");
        }
    }


    private float ConvertMixerValueToSliderValue(float mixerValue)
    {
        // Assuming the slider ranges from 0.0001 to 1
        return Mathf.Pow(10, mixerValue / 20f);
    }

}
