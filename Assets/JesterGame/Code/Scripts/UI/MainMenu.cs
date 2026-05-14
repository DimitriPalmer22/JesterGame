using System.Collections;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JesterGame.Code.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private SceneReference mainGameScene;
        [SerializeField] private SceneReference[] subScenesToLoad;

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            var activeScene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(mainGameScene.Path, LoadSceneMode.Additive);

            foreach (var sceneReference in subScenesToLoad)
                SceneManager.LoadScene(sceneReference.Path, LoadSceneMode.Additive);

            yield return null;

            // Set the loaded main game scene as active
            SceneManager.SetActiveScene(mainGameScene.LoadedScene);

            // Unload the current scene (main menu)
            SceneManager.UnloadSceneAsync(activeScene);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}