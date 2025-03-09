using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundDefine
{
	public const string Music_Login = "bgm-login";
	public const string Music_Select = "bgm-select";

	public const string SFX_Message_Info = "ui/sfx_msg_info";
	public const string SFX_Message_Error = "ui/sfx_msg_error";

	public const string SFX_UI_Click = "ui/sfx_click1";
	public const string SFX_UI_Confirm = "ui/sfx_accept1";
	public const string SFX_UI_Win_Open = "ui/sfx_win_show";
	public const string SFX_UI_Win_Close = "ui/sfx_win_close";
}
public class SoundManager : MonoSingleton<SoundManager> {
    public AudioMixer audioMixer;
    public AudioSource MusicSource;
    public AudioSource SoundSource;

    const string MusicPath = "Music/";
    const string SoundPath = "Sound/";

    private bool musicOn;
    public bool MusicOn
    {
        get { return musicOn; }
        set
        {
            musicOn = value;
            this.MusicMute(!musicOn);
        }
    }

    private bool soundOn;
    public bool SoundOn
    {
        get { return soundOn; }
        set
        {
            soundOn = value;
            this.SoundMute(!soundOn);
        }
    }

    private int musicVolume;
    public int MusicVolume
    {
        get { return musicVolume; }
        set 
        {
            if (musicVolume != value)
            {
                musicVolume = value;
                this.SetVolume("MusicVolume", musicVolume);
            }
        }
    }

    private int soundVolume;
    public int SoundVolume
    {
        get { return soundVolume; }
        set
        {
            if (soundVolume != value)
            {
                soundVolume = value;
                this.SetVolume("SoundVolume", soundVolume);
            }
        }
    }

    void Start()
    {
        this.MusicVolume = Config.MusicVolume;
        this.SoundVolume = Config.SoundVolume;
        this.MusicOn = Config.MusicOn;
        this.SoundOn = Config.SoundOn;  
    }

    private void SoundMute(bool v)
    {
        this.SetVolume("SoundVolume", v ? 0 : soundVolume);
    }

    private void MusicMute(bool v)
    {
        this.SetVolume("MusicVolume", v ? 0 : musicVolume);
    }

    public void SetVolume(string name, int value)
    {
        float volume = value * 0.5f - 50f;
        this.audioMixer.SetFloat(name, volume);
    }
    internal void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if (clip == null)
        {
            return;
        }
        if (MusicSource.isPlaying)
        { 
            MusicSource.Stop();
        }

        MusicSource.clip = clip;
        MusicSource.Play();
    }

    internal void PlaySound(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(SoundPath + name);
        if (clip == null)
        {
            return;
        }
        SoundSource.PlayOneShot(clip);
    }
}
