using System;
using System.Collections;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnrealToUnity.Code.Scripts.Core.Utility
{
    public class EnsureSubscenesLoaded : MonoBehaviour
    {
        [SerializeField] private bool bLoadOnAwake;

        [SerializeField] private SceneReference[] subscenesToLoad;

        private void Awake()
        {
            if (bLoadOnAwake)
                LoadScenes();
        }

        public void LoadScenes()
        {
            // Load sub scenes synchronously
            foreach (var sceneReference in subscenesToLoad)
            {
                if (sceneReference != null && sceneReference.TryGetPath(out _))
                    LoadSceneIfNecessary(sceneReference);
            }
        }

        private Scene LoadSceneIfNecessary(SceneReference sceneReference)
        {
            if (sceneReference == null)
                return default;

            var sceneByPath = SceneManager.GetSceneByPath(sceneReference.Path);

            if (sceneByPath.isLoaded)
            {
                // Debug.Log($"{sceneByPath.name} is already loaded.");
                return sceneByPath;
            }

            SceneManager.LoadScene(sceneReference.Path, LoadSceneMode.Additive);
            return sceneReference.LoadedScene;
        }
    }
}