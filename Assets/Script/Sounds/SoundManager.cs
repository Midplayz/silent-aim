using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource firingAudioSource;

    [Header("Action Sound Sequences")]
    [SerializeField] private List<ActionSoundSequence> actionSoundSequences;

    [Header("Gun Sounds")]
    [SerializeField] private float timeBeforePlayingNextShotSound = 1f;
    [SerializeField] private AudioClip fireSound;

    private Dictionary<string, ActionSoundSequence> actionSoundDictionary;

    private void Start()
    {
        actionSoundDictionary = new Dictionary<string, ActionSoundSequence>();
        foreach (var sequence in actionSoundSequences)
        {
            actionSoundDictionary[sequence.actionName] = sequence;
        }
    }

    public void PlayActionSoundSequence(string actionName)
    {
        if (actionSoundDictionary.ContainsKey(actionName))
        {
            StartCoroutine(PlaySoundSequence(actionSoundDictionary[actionName]));
        }
        else
        {
            Debug.LogWarning("No sound sequence found for action: " + actionName);
        }
    }

    private IEnumerator PlaySoundSequence(ActionSoundSequence sequence)
    {
        for (int i = 0; i < sequence.audioClips.Count; i++)
        {
            AudioClip clip = sequence.audioClips[i];
            audioSource.PlayOneShot(clip);
            float totalDelay = clip.length + sequence.delays[i];
            yield return new WaitForSeconds(totalDelay);
        }
    }

    public void PlayFireSound()
    {
        StartCoroutine(PlayFiringSoundSequence());
    }

    private IEnumerator PlayFiringSoundSequence()
    {
        firingAudioSource.PlayOneShot(fireSound);
        yield return new WaitForSeconds(timeBeforePlayingNextShotSound);
        PlayActionSoundSequence("NextShot");
    }
}
