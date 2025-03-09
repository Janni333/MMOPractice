using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemConfig : UIWindow {
	public Toggle toggleMusic;
	public Toggle toggleSound;

	public Slider sliderMusic;
	public Slider sliderSound;


	// Use this for initialization
	void Start () {
		this.toggleMusic.isOn = Config.MusicOn;
		this.toggleMusic.isOn = Config.SoundOn;
		this.sliderMusic.value = Config.MusicVolume;
		this.sliderSound.value = Config.SoundVolume;
	}

	public override void OnClickYes()
	{
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		PlayerPrefs.Save();
		base.OnClickYes();
	}
	public void MusicVolume(int vol)
	{
		Config.MusicVolume = vol;
		PlaySound();
	}

	public void SoundVolume(int vol) 
	{
        Config.SoundVolume = vol;
        PlaySound();
    }

    public void MusicToggle(bool on)
    {
		Config.MusicOn = on;
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
	}

    public void SoundToggle(bool on)
    {
        Config.SoundOn = on;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

	float lastPlay = 0;
	private void PlaySound()
	{
		if (Time.realtimeSinceStartup - lastPlay > 0.1)
		{
			lastPlay = Time.realtimeSinceStartup;
			SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		}
	}
}
