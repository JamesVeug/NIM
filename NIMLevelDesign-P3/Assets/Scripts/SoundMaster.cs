using UnityEngine;
using System.Collections;

public static class SoundMaster {

    [Range(min:0, max:100)]
    public static float Master_Volume = 100;

    // Play a random sound from a given array of sound files
    public static void playRandomSound(AudioClip[] clips, float[] volumes, Vector3 position)
    {
        // Don't try playing a sound if there aren't any
        if (clips == null || clips.Length <= 0)
        {
            return;
        }

        // Play random sound
        int index = Random.Range(0, clips.Length);
        AudioClip clip = clips[index];

        float volume = volumes.Length <= index ? 100 : volumes[index];
        AudioSource.PlayClipAtPoint(clip, position, (volume / 100) * (SoundMaster.Master_Volume/100));
    }

    public static void setMasterVolume(float v)
    {
        Master_Volume = v;
    }
}
