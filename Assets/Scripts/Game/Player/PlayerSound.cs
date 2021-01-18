using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Game.Player
{
    /*
     * @brief 플레이어의 애니메이션에 들어있는 사운드재생함수 콜백을 처리하기위한 컴포넌트
     */
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
            var volumeSliders = UI.SettingMenu.VolumeSliders.Get();
            if (!ReferenceEquals(volumeSliders, null))
            {
                var effectVolume = volumeSliders.EffectVolume;
                _audio.volume = Random.Range(effectVolume * 0.7f, effectVolume * 1.0f);
            }
            else
            {
                _audio.volume = 0.0f;
            }
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
}
