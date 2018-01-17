using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{

    private Image _sfxOnBtn;
    private Image _musicOnBtn;
    private Image _sfxOffBtn;
    private Image _musicOffBtn;

    private Slider _sfxSlider;
    private Slider _musicSlider;

    public static int _sfxOn = 1;
    public static int _musicOn = 1;

    public static float _sfxVolume = 0.0f;
    public static float _musicVolume = 0.0f;

    private const float LOWEST_VOLUME = -39.0f;
    public const int TRUE = 1;
    public const int FALSE = 0;

    [SerializeField]
    private AudioMixer _masterMixer;
    [SerializeField]
    private bool _inOptionMenu = true;

    public void Start()
    {
        if (!_inOptionMenu) return;
        _sfxOnBtn = GameObject.Find("OnBtnSE").GetComponent<Image>();
        _sfxOffBtn = GameObject.Find("OffBtnSE").GetComponent<Image>();

        _musicOnBtn = GameObject.Find("OnBtnM").GetComponent<Image>();
        _musicOffBtn = GameObject.Find("OffBtnM").GetComponent<Image>();

        _sfxSlider = GameObject.Find("SfxSlider").GetComponent<Slider>();
        _musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();

        _sfxSlider.value = _sfxVolume;
        _musicSlider.value = _musicVolume;

        if (_sfxOn != TRUE)
        {
            Color temp = _sfxOnBtn.color;
            _sfxOnBtn.color = _sfxOffBtn.color;
            _sfxOffBtn.color = temp;
        }


        if (_musicOn != TRUE)
        {
            Color temp = _musicOnBtn.color;
            _musicOnBtn.color = _musicOffBtn.color;
            _musicOffBtn.color = temp;
        }

    }

    public static void LoadOptions()
    {
        _sfxVolume = PlayerPrefs.GetFloat("_sfxVolume");
        _musicVolume = PlayerPrefs.GetFloat("_musicVolume");
        _musicOn = PlayerPrefs.GetInt("_musicOn");
        _sfxOn = PlayerPrefs.GetInt("_sfxOn");
    }

    public void ChangeMusic()
    {
        _musicVolume = _musicSlider.value;
        if (_musicVolume < LOWEST_VOLUME)
        {
            _musicVolume = -80.0f;
        }
        if (_musicOn == TRUE)
        {
            _masterMixer.SetFloat("MusicVol", _musicVolume);
        }
        PlayerPrefs.SetFloat("_musicVolume", _musicVolume);
    }

    public void ChangeSfx()
    {
        _sfxVolume = _sfxSlider.value;
        if (_sfxVolume < LOWEST_VOLUME)
        {
            _sfxVolume = -80.0f;
        }
        if (_sfxOn == TRUE)
        {
            _masterMixer.SetFloat("SfxVol", _sfxVolume);
        }

        PlayerPrefs.SetFloat("_sfxVolume", _sfxVolume);
    }

    public void ToggleSoundEffects(int on)
    {
        if (on == _sfxOn)
            return;
        else
            _sfxOn = on;

        if (_sfxOn == TRUE)
        {
            Color temp = _sfxOnBtn.color;
            _sfxOnBtn.color = _sfxOffBtn.color;
            _sfxOffBtn.color = temp;
            _masterMixer.SetFloat("SfxVol", _sfxVolume);
        }
        else
        {
            Color temp = _sfxOnBtn.color;
            _sfxOnBtn.color = _sfxOffBtn.color;
            _sfxOffBtn.color = temp;
            _masterMixer.SetFloat("SfxVol", -80.0f);
        }

        PlayerPrefs.SetInt("_sfxOn", _sfxOn);
    }

    public void ToggleMusic(int on)
    {
        if (on == _musicOn)
            return;
        else
            _musicOn = on;

        if (_musicOn == TRUE)
        {
            Color temp = _musicOnBtn.color;
            _musicOnBtn.color = _musicOffBtn.color;
            _musicOffBtn.color = temp;
            _masterMixer.SetFloat("MusicVol", _musicVolume);
        }
        else
        {
            Color temp = _musicOnBtn.color;
            _musicOnBtn.color = _musicOffBtn.color;
            _musicOffBtn.color = temp;
            _masterMixer.SetFloat("MusicVol", -80.0f);
        }

        PlayerPrefs.SetInt("_musicOn", _musicOn);
    }

    public void ToggleSoundEffects()
    {
        if (_sfxOn == TRUE)
        {
            _sfxOn = FALSE;
            _masterMixer.SetFloat("SfxVol", -80.0f);
        }
        else
        {
            _sfxOn = TRUE;
            _masterMixer.SetFloat("SfxVol", _sfxVolume);
        }

        PlayerPrefs.SetInt("_sfxOn", _sfxOn);
    }

    public void ToggleMusic()
    {
        if (_musicOn == TRUE)
        {
            _musicOn = FALSE;
            _masterMixer.SetFloat("MusicVol", -80.0f);
        }
        else
        {
            _musicOn = TRUE;
            _masterMixer.SetFloat("MusicVol", _musicVolume);
        }

        PlayerPrefs.SetInt("_musicOn", _musicOn);
    }
}
