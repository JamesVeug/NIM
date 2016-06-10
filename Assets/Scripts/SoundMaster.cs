using UnityEngine;
using System.Collections;

public static class SoundMaster
{
    [Range(min: 0, max: 500)]
    public static float Master_Volume = 500;

    // Make the sound louder
    public static float Game_Volume = 0.5f;
    private const float Master_Volume_SCALAR = 0.6f;

    // Play a random sound from a given array of sound files
    public static void playRandomSound(AudioClip[] clips, float[] volumes, AudioSource s)
    {
        // Don't try playing a sound if there aren't any
        if (clips == null || clips.Length <= 0)
        {
            return;
        }

        // Play random sound
        int index = Random.Range(0, clips.Length);
        AudioClip clip = clips[index];
        if (clip == null)
        {
            Debug.Log("Audio Clip at position " + index + " is null");
            return;
        }

        float volume = volumes.Length <= index ? 100 : volumes[index];
        s.PlayOneShot(clip,(volume/100)*(Master_Volume/100)* (Master_Volume_SCALAR* Game_Volume));
    }

    public static void setMasterVolume(float v)
    {
        Master_Volume = v;
    }
}
