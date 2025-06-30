using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SoundAudioClip
{
    public SoundManager.Sound sound;
    public AudioClip clip;
}

public static class SoundManager
{
    public enum Sound
    {
        NULL,
        LadderClimb,
        LadderPlace,
        PickUp,
        PickUpGreyed,
        BombExplosion,
    }

    private static Dictionary<Sound, float> soundTimeDictionary;

    private static GameObject oneShotGameObject;

    private static AudioSource oneShotAudioSorce;

    public static void initialize()
    {
        soundTimeDictionary = new Dictionary<Sound, float>();
        soundTimeDictionary[Sound.LadderClimb] = 0.0f;
    }

    public static void playSound(Sound sound, Vector3 pos)
    {
        if (canPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = pos;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = getAudioClip(sound);
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.Play();

            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
    }

    public static void playSound(Sound sound)
    {
        if (canPlaySound(sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotAudioSorce = oneShotGameObject.AddComponent<AudioSource>();
            }
            
            oneShotAudioSorce.PlayOneShot(getAudioClip(sound));
        }        
    }

    private static bool canPlaySound(Sound sound)
    {
        switch (sound)
        {
            case Sound.LadderClimb:
                {
                    if (soundTimeDictionary.ContainsKey(sound))
                    {
                        float lastTimePlayed = soundTimeDictionary[sound];
                        float ladderClimbInterval = 0.1f;
                        if (lastTimePlayed + ladderClimbInterval < Time.time)
                        {                            
                            soundTimeDictionary[sound] = Time.time;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }                    
                }

           default:
                {
                    return true;
                }
        }
    }

    private static AudioClip getAudioClip(Sound sound)
    {
        foreach (SoundAudioClip soundAudioClip in GameAssets.getInstance().soundAudioClips)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.clip;
            }
        }

        Debug.LogWarning("Sound clip: " + sound + " not found!");
        return null;
    }

   
}
