using System.Collections;
using System.Linq;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Cutscenes;

namespace JesterGame.Code.Scripts.Progression
{
    public class DeadNpcFadeCutscene : CutsceneComponent<JesterGameEventArgs>
    {
        private static readonly int AlphaMatParam = Shader.PropertyToID("_Alpha");

        [SerializeField] private AnimationCurve animationCurve;

        protected override IEnumerator CustomRunCutscene(JesterGameEventArgs cutsceneStruct)
        {
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

            // Destroy the game object after a little second
            Destroy(gameObject, .5f);
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
    }
}