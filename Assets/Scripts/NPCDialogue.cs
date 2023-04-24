using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    public NPC npc;
    public TMP_InputField inputPrompt;
    public Button submitButton;
    public TMP_Text nameTag;
    public GameObject dialoguePrefab;

    private Canvas _mainCanvas;
    private bool _isMainCanvasNull;

    private void Start()
    {
        _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        _isMainCanvasNull = _mainCanvas == null;
        if (_isMainCanvasNull) Debug.LogWarning("Could not find main canvas.");
        
        SetNameTag();
    }

    private void OnEnable()
    {
        Dialogue.OnDialogueStarted += () => { submitButton.interactable = false; };
        Dialogue.OnDialogueFinished += () => { submitButton.interactable = true; };
    }

    public void OnSubmit()
    {
        var prompt = inputPrompt.text;
        inputPrompt.interactable = false;
        submitButton.interactable = false;

        npc.HandleInput(prompt, OnResponseReceived);
    }
    
    public void OnReset()
    {
        npc.ResetChat();
        SetNameTag();
    }

    private void OnResponseReceived(string response)
    {
        Debug.Log($"Received response: {response}");
        var parentTransform = _isMainCanvasNull ? transform : _mainCanvas.transform;
        var newDialogue = Instantiate(dialoguePrefab, parentTransform).GetComponent<Dialogue>();
        newDialogue.line = $"<b>{npc.npcDescriptor.npcName}:</b> {response}";
        submitButton.interactable = true;
        inputPrompt.interactable = true;
        inputPrompt.text = "";
    }

    private void SetNameTag()
    {
        if (nameTag == null) return;
        if (npc.npcDescriptor == null) return;

        nameTag.text = npc.npcDescriptor.npcName;
    }
}