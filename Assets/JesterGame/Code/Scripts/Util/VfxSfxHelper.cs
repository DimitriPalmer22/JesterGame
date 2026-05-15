using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnityEngine.VFX;
using UnrealToUnity.Code.Scripts.Core.Utility.Interfaces;

namespace JesterGame.Code.Scripts.Util
{
    public class VfxSfxHelper : MonoBehaviour, IRunnable<JesterGameEventArgs>
    {
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private VisualEffect vfxAsset;
        [SerializeField] private AudioSource audioSource;

        public void Play()
        {
            Run(new JesterGameEventArgs());
        }

        public void Run(JesterGameEventArgs _)
        {
            if (particles != null)
                particles.Play();

            if (vfxAsset != null)
                vfxAsset.Play();

            if (audioSource != null)
                audioSource.Play();
        }
    }
}