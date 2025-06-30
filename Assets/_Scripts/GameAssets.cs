using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : SingletonPersistent<GameAssets>
{
    public SoundAudioClip[] soundAudioClips;

    protected override void Awake()
    {
        base.Awake();

        SoundManager.initialize();
    }
}
