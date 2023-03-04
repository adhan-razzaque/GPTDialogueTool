using System;
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
    public Button button;
    public GameObject dialoguePrefab;

    private void OnEnable()
    {
        Dialogue.OnDialogueStarted += () => { button.interactable = false; };
        Dialogue.OnDialogueFinished += () => { button.interactable = true; };
    }

    private void OnResponseReceived(string response)
    {
        Debug.Log($"Received response: {response}");
        var newDialogue = Instantiate(dialoguePrefab).GetComponent<Dialogue>();
        newDialogue.line = response;
    }

    public void OnSubmit()
    {
        var prompt = npcDescriptor.BuildGptDescriptor(inputPrompt.text);
        inputPrompt.text = "";
        OpenAIManager.Instance.Execute(prompt, OnResponseReceived);
    }
}
