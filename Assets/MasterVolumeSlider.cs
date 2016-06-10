using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MasterVolumeSlider : MonoBehaviour {

	public void changeValue()
    {
        float value = GetComponent<Slider>().value;
        SoundMaster.Game_Volume = value;
    }

    public void changeMusicVolume()
    {
        float value = GetComponent<Slider>().value;
        AudioSource a = Camera.main.GetComponent<AudioSource>();
        if( a != null)
        {
            a.volume = value;
        }
    }
}
