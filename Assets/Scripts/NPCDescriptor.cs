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

    [Serializable]
    public enum LikeLevel
    {
        Hate,
        Dislike,
        Like,
        Love
    }

    [Serializable]
    public struct Relationship
    {
        public LikeLevel level;
        public string name;
    }

    public string npcName;
    public string gender;
    public int age;
    
    [Header("Description")]
    public string temperament;
    public string background;
    public string publicImage;

    public List<Mood> moods;
    public List<Relationship> relationships;

    private static string GetLevel<T>(T level)
    {
        return Enum.GetName(typeof(T), level)?.ToLower();
    }

    public string GetMoodString(Mood mood)
    {
        return $"{GetLevel(mood.level)} {mood.name}";
    }

    public string GetAllMoods()
    {
        var stringBuilder = new StringBuilder();
        var counter = 0;
        
        foreach (var mood in moods)
        {
            stringBuilder.Append(GetMoodString(mood));

            if (++counter != moods.Count)
            {
                stringBuilder.Append(", ");
            }
        }

        return stringBuilder.ToString();
    }
    
    public string GetAllRelationships()
    {
        var stringBuilder = new StringBuilder();
        var counter = 0;
        
        foreach (var relationship in relationships)
        {
            stringBuilder.Append($"{GetLevel(relationship.level)} {relationship.name}");

            if (++counter != relationships.Count)
            {
                stringBuilder.Append(", ");
            }
        }

        return stringBuilder.ToString();
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

            stringBuilder.Append(GetAllMoods());
            
            stringBuilder.Append("right now. ");
        }
        
        if (relationships.Count > 0)
        {
            stringBuilder.Append("Your relationships are that you ");

            stringBuilder.Append(GetAllRelationships());

            stringBuilder.Append(". ");
        }

        return stringBuilder.ToString();
    }

    public string BuildGptDescriptor(string prompt)
    {
        var description = GetNpcString();
        // return $"The player tells you \"{prompt}\". What is your response? Start with \"{name}:\".";
        return $"{description} The player tells you \"{prompt}\". What is your response and feeling? Start with \"{name}:\". End with a new line, then \"Feeling:\", and finally the moods you are feeling.";
    }
}