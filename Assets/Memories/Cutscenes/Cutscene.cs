using System;
using FMODUnity;
using UnityEngine;

namespace Memories.Cutscenes;

[AddComponentMenu("Cutscenes/Cutscene")]
public sealed class Cutscene : MonoBehaviour
{
    public int timesPlayed;
    public CutsceneData data;
}

[CreateAssetMenu(menuName = "Cutscenes/Cutscene Data")]
public sealed class CutsceneData : ScriptableObject
{
    public string cutsceneName;

    public enum RepeatBehaviour
    {
        Last,
        Random,
        Loop,
    }
    public RepeatBehaviour repeatBehaviour;

    [SerializeReference]
    public DialogueInstruction[] mainLines;
    [SerializeReference]
    public LineSet[] repeatLines;
}

// unity is too weak to serialize a list of arrays
[Serializable]
public sealed class LineSet
{
    [SerializeReference]
    public DialogueInstruction[] lines;
}

[Serializable]
public abstract class DialogueInstruction
{
}

[Serializable]
public sealed class Pause : DialogueInstruction
{
    public float duration;
}

[Serializable]
public sealed class TextLine : DialogueInstruction
{
    public string dialogueActorName;
    [TextArea]
    public string text;
}

[Serializable]
public sealed class CustomSequence : DialogueInstruction
{
    public string sequenceName;
}

[Serializable]
public sealed class PlaySfx : DialogueInstruction
{
    public EventReference sound;
}

[Serializable]
public sealed class MultipleWaitAll : DialogueInstruction
{
    [SerializeReference]
    public DialogueInstruction[] instructions;
}
