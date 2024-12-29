using FMODUnity;
using UnityEngine;

namespace Memories.Cutscenes;

[CreateAssetMenu(menuName = "Cutscenes/Dialogue Actor Data", fileName = "DialogueActor", order = 0)]
public sealed class DialogueActorData : ScriptableObject
{
    public string dialogueActorName;
    public Color textColor = Color.white;
    public EventReference talkNoise;
}
