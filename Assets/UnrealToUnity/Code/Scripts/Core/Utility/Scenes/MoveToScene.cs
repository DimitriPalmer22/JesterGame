using System;
using Eflatun.SceneReference;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnrealToUnity.Code.Scripts.Core.Utility.Scenes
{
    public class MoveToScene : MonoBehaviour
    {
        [SerializeField] private bool bRunOnStart;

        [SerializeField] private bool bSpecifyScene;

        [SerializeField, ShowIf("bSpecifyScene")]
        private SceneReference sceneToMoveTo;

        private void Start()
        {
            if (bRunOnStart)
                MoveToCorrectScene();
        }

        public void MoveToCorrectScene()
        {
            var nextScene = !bSpecifyScene ? SceneManager.GetActiveScene() : sceneToMoveTo.LoadedScene;

            if (!nextScene.IsValid())
                return;

            // Get the loaded scene from the scene reference.
            // If the scene is not loaded, return.
            if (!nextScene.isLoaded)
                return;

            // Get the current scene this object belongs to
            var currentScene = gameObject.scene;
            if (currentScene == nextScene)
                return;

            // Move the object to the new scene
            SceneManager.MoveGameObjectToScene(gameObject, nextScene);
        }
    }
}