using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class NPCDescriptor : ScriptableObject
{
    public enum MoodLevel
    {
        Not = 0,
        ALittle = 1,
        ALot = 2,
        Extremely = 3
    }

    private string[] _moodLevelStrings = {"not", "a little", "a lot", "extremely"};
    
    public string npcName;
    public string temperament;
    public string background;
    public string gender;
    public int age;

    public string GetMoodLevel(MoodLevel moodLevel)
    {
        return _moodLevelStrings[moodLevel.GetHashCode()];
    }

    public string BuildGptDescriptor(string prompt)
    {
        var stringBuilder = new StringBuilder("You are an npc");

        if (npcName.Length > 0)
        {
            stringBuilder.Append($"named {npcName}, ");
        }
        
        if (gender.Length > 0)
        {
            stringBuilder.Append($"a {gender}, ");
        }

        if (age != 0)
        {
            stringBuilder.Append($"a {age} year old, ");
        }

        if (temperament.Length > 0)
        {
            stringBuilder.Append($"{temperament}, ");
        }

        if (background.Length > 0)
        {
            stringBuilder.Append($"{background}, ");
        }

        stringBuilder.Append($"the player tells you \"{prompt}\". What is your response?");

        return stringBuilder.ToString();
    }
}
