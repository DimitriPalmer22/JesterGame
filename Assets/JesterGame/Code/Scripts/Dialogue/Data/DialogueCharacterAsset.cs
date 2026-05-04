using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    /// <summary>
    /// A scriptable object containing a single dialogue character.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterName", menuName = "Data/Dialogue/Character Asset")]
    public class DialogueCharacterAsset : ScriptableObject
    {
        [SerializeField] public DialogueCharacter character;
    }
}