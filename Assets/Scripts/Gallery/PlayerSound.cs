using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip stepClip;
    [SerializeField] private AudioClip jumpStartSound;
    [SerializeField] private AudioClip jumpEndSound;

    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void RandomAudioSetting()
    {
        _audio.pitch = Random.Range(0.7f, 1.2f);
        _audio.volume = Random.Range(0.9f, 1.0f);
    }

    public void StepSound()
    {
        RandomAudioSetting();
        _audio.PlayOneShot(stepClip);
    }

    public void JumpStartSound()
    {
        RandomAudioSetting();
        _audio.PlayOneShot(jumpStartSound);
    }

    public void JumpEndSound()
    {
        RandomAudioSetting();
        _audio.PlayOneShot(jumpEndSound);
    }
}
