using System.Collections.Generic;
using JesterGame.Code.Scripts.Dialogue.Data;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.GameMode;

namespace JesterGame.Code.Scripts.Core
{
    public class ImpostorGameMode : GameMode
    {
        [SerializeField] private Dictionary<DialogueCharacterAsset, CharacterInstance> characterInstanceMap;
    }
}