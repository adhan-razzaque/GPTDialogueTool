using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu]
public class NPCDescriptor : ScriptableObject
{
    [Serializable]
    public enum MoodLevel
    {
        Not,
        Slightly,
        Moderately,
        Very,
        Extremely
    }
    
    [Serializable]
    public struct Mood
    {
        public string name;
        public MoodLevel level;
    }

    public string npcName;
    public string gender;
    public int age;
    
    [Header("Description")]
    public string temperament;
    public string background;
    public string publicImage;

    public List<Mood> moods;

    private static string GetMoodLevel(MoodLevel moodLevel)
    {
        return Enum.GetName(typeof(MoodLevel), moodLevel)?.ToLower();
    }

    public string GetMoodString(Mood mood)
    {
        return $"{GetMoodLevel(mood.level)} {mood.name}";
    }

    public string GetNpcString()
    {
        var stringBuilder = new StringBuilder("You are an npc ");
        
        if (!string.IsNullOrEmpty(npcName))
        {
            stringBuilder.Append($"named {npcName}, ");
        }

        if (!string.IsNullOrEmpty(gender))
        {
            stringBuilder.Append($"a {gender}, ");
        }

        if (age != 0)
        {
            stringBuilder.Append($"a {age} year old, ");
        }

        if (!string.IsNullOrEmpty(temperament))
        {
            stringBuilder.Append($"{temperament}, ");
        }

        if (!string.IsNullOrEmpty(background))
        {
            stringBuilder.Append($"{background}, ");
        }

        if (!string.IsNullOrEmpty(publicImage))
        {
            stringBuilder.Append($"{publicImage}, ");
        }

        stringBuilder.Append("and you are in this game. ");

        if (moods.Count > 0)
        {
            stringBuilder.Append("You feel ");

            foreach (var mood in moods)
            {
                stringBuilder.Append($"{GetMoodString(mood)}, ");
            }

            stringBuilder.Append("right now.");
        }

        return stringBuilder.ToString();
    }

    public string BuildGptDescriptor(string prompt)
    {
        var description = GetNpcString();
        return $"{description} The player tells you \"{prompt}\". What is your response and feeling? Start with \"{name}:\". End with a new line, then \"Feeling:\", and finally the moods you are feeling.";
    }
}