using System;
using FMODUnity;
using TriInspector;
using UnityEngine;

namespace Memories.Cutscenes;

[CreateAssetMenu(menuName = "Cutscenes/Cutscene Data")]
public sealed class CutsceneData : ScriptableObject
{
    public string cutsceneName;
    public bool skippable = true;
    public bool repeatable;

    public enum RepeatBehaviour
    {
        Last,
        Random,
        Loop,
    }
    [ShowIf(nameof(repeatable))]
    public RepeatBehaviour repeatBehaviour;

    [SerializeReference]
    public DialogueInstruction[] mainLines;
    [SerializeReference, ShowIf(nameof(repeatable))]
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
public class Pause : DialogueInstruction
{
    public float duration;
    public bool keepTextbox;
}

[Serializable]
public class TextLine : DialogueInstruction
{
    public DialogueActorData actor;
    [TextArea]
    public string text;
}

[Serializable]
public class ClearTextLine : DialogueInstruction
{
    public DialogueActorData actor;
}

[Serializable]
public sealed class DropdownTextLine : TextLine
{
    public int dropdownAtChar;
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

[Serializable]
public sealed class TurnPages : DialogueInstruction
{
    public int pages;
}

[Serializable]
public sealed class CloseBook : DialogueInstruction
{
}

[Serializable]
public sealed class SetMusicVolume : DialogueInstruction
{
    public float volume;
    public float fadeDuration;
}

[Serializable]
public sealed class SetGlobalFmodParameter : DialogueInstruction
{
    public EventReference soundEvent;
    public string parameterName;
    public float value;
}
