using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionSoundSequence
{
    public string actionName;
    public List<AudioClip> audioClips;
    public List<float> delays;
}
