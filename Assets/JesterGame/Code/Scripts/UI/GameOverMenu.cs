using System.Collections;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JesterGame.Code.Scripts.UI
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] private SceneReference mainMenuScene;

        public void StartGame()
        {
            SceneManager.LoadScene(mainMenuScene.Path, LoadSceneMode.Single);
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