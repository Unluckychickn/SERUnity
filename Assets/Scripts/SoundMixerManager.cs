using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

   private void Awake(){
    ItemAssets.Instance.LoadVolumeSettings();
   }

    public void SetGeneralVolume(float level)
    {
        audioMixer.SetFloat("MainMixer", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("General", level);
        PlayerPrefs.Save();
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SoundMixer", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("Sound", level);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicMixer", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("Music", level);
        PlayerPrefs.Save();
    }


}
