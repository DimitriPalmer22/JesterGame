using System;
using System.Collections;
using System.Linq;
using JesterGame.Code.Scripts.Core;
using Unity.Cinemachine;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Cutscenes;
using UnrealToUnity.Code.Scripts.Core.Player;

namespace JesterGame.Code.Scripts.Progression
{
    public class DeadNpcFadeCutscene : CutsceneComponent<JesterGameEventArgs>
    {
        private const int MAX_PRIORITY = 9999;
        private const int MIN_PRIORITY = -9999;

        private static readonly int AlphaMatParam = Shader.PropertyToID("_Alpha");

        [SerializeField] private AnimationCurve animationCurve;

        [SerializeField] private CinemachineCamera cutsceneCamera;
        [SerializeField] private float blendTime = 1f;

        private void Awake()
        {
            SetCameraPriority(false);
        }

        protected override IEnumerator CustomRunCutscene(JesterGameEventArgs cutsceneStruct)
        {
            PlayerController playerController = null;
            if (cutsceneStruct.controller is PlayerController controller)
            {
                playerController = controller;
                playerController.AddInputBlocker(this);
            }

            // Set the camera's priority to max real quick
            SetCameraPriority(true);
            yield return new WaitForSeconds(blendTime);

            var startTime = Time.time;
            var endTime = Time.time + animationCurve.keys[^1].time;

            // Get the renderers on the game object
            var renderers = GetComponentsInChildren<Renderer>();
            var materials = renderers.SelectMany(r => r.materials).ToArray();

            // Loop through the animation curve and set the fade value based on the curve's evaluation at the current time
            while (Time.time < endTime)
            {
                var interpValue = Time.time - startTime;
                var alpha = animationCurve.Evaluate(interpValue);
                SetFadeValue(alpha, materials);

                yield return null;
            }

            // Ensure the final value is set at the end of the animation curve
            SetFadeValue(animationCurve.keys[^1].value, materials);

            // Reset the priority to deprioritize the camera.
            SetCameraPriority(false);
            yield return new WaitForSeconds(blendTime);

            if (playerController != null)
                playerController.RemoveInputBlocker(this);

            // Destroy the game object after a little second
            Destroy(gameObject, 1f);
        }

        private void SetFadeValue(float value, Material[] materials)
        {
            foreach (var mat in materials)
            {
                if (!mat.HasFloat(AlphaMatParam))
                    continue;

                mat.SetFloat(AlphaMatParam, value);
            }
        }

        private void SetCameraPriority(bool value)
        {
            if (!cutsceneCamera)
                return;

            cutsceneCamera.Priority = new PrioritySettings()
            {
                Enabled = true,
                Value = value ? MAX_PRIORITY : MIN_PRIORITY
            };

            cutsceneCamera.enabled = value;

            cutsceneCamera.Prioritize();
        }
    }
}