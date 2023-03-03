using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public NPCDescriptor npcDescriptor;
    public TMP_InputField inputPrompt;
    public GameObject dialoguePrefab;

    private void OnResponseReceived(string response)
    {
        Debug.Log($"Received response: {response}");
        var newDialogue = Instantiate(dialoguePrefab).GetComponent<Dialogue>();
        newDialogue.line = response;
    }

    public void OnSubmit()
    {
        string prompt = npcDescriptor.BuildGptDescriptor(inputPrompt.text);
        OpenAIManager.Instance.Execute(prompt, OnResponseReceived);
    }
}
