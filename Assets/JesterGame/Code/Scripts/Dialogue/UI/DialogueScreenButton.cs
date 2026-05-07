using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JesterGame.Code.Scripts.Dialogue.UI
{
    /// <summary>
    /// Buttons for selecting choices in the dialogue screen.
    /// </summary>
    public class DialogueScreenButton : MonoBehaviour
    {
        [SerializeField] public Button button;
        [SerializeField] private TMP_Text buttonText;

        private DialogueScreen _dialogueScreen;
        private int _buttonIndex;

        public void SetText(string text) => buttonText?.SetText(text);

        public void BindToDialogueScreen(DialogueScreen dialogueScreen, int index)
        {
            _dialogueScreen = dialogueScreen;
            _buttonIndex = index;
        }

        public void OnButtonClick()
        {
            _dialogueScreen?.SetCurrentChoiceIndex(_buttonIndex);
        }
    }
}