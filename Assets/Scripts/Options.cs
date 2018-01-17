using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{

    private Image sfxOnBtn;
    private Image musicOnBtn;
    private Image sfxOffBtn;
    private Image musicOffBtn;

    private Slider sfxSlider;
    private Slider musicSlider;

    public static int SfxOn = 1;
    public static int MusicOn = 1;

    public static float SfxVolume = 0.0f;
    public static float MusicVolume = 0.0f;

    private const float LowestVolume = -39.0f;
    public const int True = 1;
    public const int False = 0;

    [SerializeField]
    private AudioMixer masterMixer;

    public void Start()
    {
        sfxOnBtn = GameObject.Find("OnBtnSE").GetComponent<Image>();
        sfxOffBtn = GameObject.Find("OffBtnSE").GetComponent<Image>();

        musicOnBtn = GameObject.Find("OnBtnM").GetComponent<Image>();
        musicOffBtn = GameObject.Find("OffBtnM").GetComponent<Image>();

        sfxSlider = GameObject.Find("SfxSlider").GetComponent<Slider>();
        musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();

        sfxSlider.value = SfxVolume;
        musicSlider.value = MusicVolume;

        if (SfxOn != True)
        {
            Color temp = sfxOnBtn.color;
            sfxOnBtn.color = sfxOffBtn.color;
            sfxOffBtn.color = temp;
        }


        if (MusicOn != True)
        {
            Color temp = musicOnBtn.color;
            musicOnBtn.color = musicOffBtn.color;
            musicOffBtn.color = temp;
        }

    }

    public static void LoadOptions()
    {
        SfxVolume = PlayerPrefs.GetFloat("_sfxVolume");
        MusicVolume = PlayerPrefs.GetFloat("_musicVolume");
        MusicOn = PlayerPrefs.GetInt("_musicOn");
        SfxOn = PlayerPrefs.GetInt("_sfxOn");
    }

    public void ChangeMusic()
    {
        MusicVolume = musicSlider.value;
        if (MusicVolume < LowestVolume)
        {
            MusicVolume = -80.0f;
        }
        if (MusicOn == True)
        {
            masterMixer.SetFloat("MusicVol", MusicVolume);
        }
        PlayerPrefs.SetFloat("_musicVolume", MusicVolume);
    }

    public void ChangeSfx()
    {
        SfxVolume = sfxSlider.value;
        if (SfxVolume < LowestVolume)
        {
            SfxVolume = -80.0f;
        }
        if (SfxOn == True)
        {
            masterMixer.SetFloat("SfxVol", SfxVolume);
        }

        PlayerPrefs.SetFloat("_sfxVolume", SfxVolume);
    }

    public void ToggleSoundEffects(int on)
    {
        if (on == SfxOn)
            return;
        else
            SfxOn = on;

        if (SfxOn == True)
        {
            Color temp = sfxOnBtn.color;
            sfxOnBtn.color = sfxOffBtn.color;
            sfxOffBtn.color = temp;
            masterMixer.SetFloat("SfxVol", SfxVolume);
        }
        else
        {
            Color temp = sfxOnBtn.color;
            sfxOnBtn.color = sfxOffBtn.color;
            sfxOffBtn.color = temp;
            masterMixer.SetFloat("SfxVol", -80.0f);
        }

        PlayerPrefs.SetInt("_sfxOn", SfxOn);
    }

    public void ToggleMusic(int on)
    {
        if (on == MusicOn)
            return;
        else
            MusicOn = on;

        if (MusicOn == True)
        {
            Color temp = musicOnBtn.color;
            musicOnBtn.color = musicOffBtn.color;
            musicOffBtn.color = temp;
            masterMixer.SetFloat("MusicVol", MusicVolume);
        }
        else
        {
            Color temp = musicOnBtn.color;
            musicOnBtn.color = musicOffBtn.color;
            musicOffBtn.color = temp;
            masterMixer.SetFloat("MusicVol", -80.0f);
        }

        PlayerPrefs.SetInt("_musicOn", MusicOn);
    }

    public void ToggleSoundEffects()
    {
        if (SfxOn == True)
        {
            SfxOn = False;
            masterMixer.SetFloat("SfxVol", -80.0f);
        }
        else
        {
            SfxOn = True;
            masterMixer.SetFloat("SfxVol", SfxVolume);
        }

        PlayerPrefs.SetInt("_sfxOn", SfxOn);
    }

    public void ToggleMusic()
    {
        if (MusicOn == True)
        {
            MusicOn = False;
            masterMixer.SetFloat("MusicVol", -80.0f);
        }
        else
        {
            MusicOn = True;
            masterMixer.SetFloat("MusicVol", MusicVolume);
        }

        PlayerPrefs.SetInt("_musicOn", MusicOn);
    }
}
