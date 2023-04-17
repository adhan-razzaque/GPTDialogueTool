using Managers;
using OpenAI_API;
using OpenAI_API.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public NPCDescriptor npcDescriptor;
    public TMP_InputField inputPrompt;
    public Button button;
    public TMP_Text nameTag;
    public GameObject dialoguePrefab;

    private Canvas _mainCanvas;
    private bool _isMainCanvasNull;

    private Conversation _chat;

    private void Start()
    {
        _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        _isMainCanvasNull = _mainCanvas == null;
        if (_isMainCanvasNull) Debug.Log("Could not find main canvas.");

        SetNameTag();

        var api = new OpenAIAPI();
        _chat = api.Chat.CreateConversation();
        
        // System message to define how to NPC interacts
        _chat.AppendSystemMessage("You will engage in a natural dialogue as if you were an npc in a game. " + 
                                  npcDescriptor.GetNpcString() + " You must only respond as this npc. " +
                                  "You only respond as if you were in a natural conversation, not as an AI helper.");
        
        // Provide some examples of the NPC interaction
        _chat.AppendUserInput("What is your name?");
        _chat.AppendExampleChatbotOutput($"My name is {npcDescriptor.npcName}.");
        _chat.AppendUserInput("How old are you?");
        _chat.AppendExampleChatbotOutput($"I am {npcDescriptor.age} years old.");
    }

    private void SetNameTag()
    {
        if (nameTag == null) return;
        if (npcDescriptor == null) return;

        nameTag.text = npcDescriptor.npcName;
    }

    private void OnEnable()
    {
        Dialogue.OnDialogueStarted += () => { button.interactable = false; };
        Dialogue.OnDialogueFinished += () => { button.interactable = true; };
    }

    private void OnResponseReceived(string response)
    {
        Debug.Log($"Received response: {response}");
        var parentTransform = _isMainCanvasNull ? transform : _mainCanvas.transform;
        var newDialogue = Instantiate(dialoguePrefab, parentTransform).GetComponent<Dialogue>();
        newDialogue.line = response;
    }
    
    private async void HandleInput(string prompt)
    {
        _chat.AppendUserInput(prompt);
        var response = await _chat.GetResponseFromChatbotAsync();
        Debug.Log(response);
        OnResponseReceived(response);
    }

    public void OnSubmit()
    {
        // var prompt = npcDescriptor.BuildGptDescriptor(inputPrompt.text);
        var prompt = inputPrompt.text;
        inputPrompt.text = "";
        // OpenAIChatManager.Instance.SetSystemMessage(npcDescriptor.GetNpcString());
        // OpenAIChatManager.Instance.Execute(prompt, OnResponseReceived, true);
        HandleInput(prompt);
    }
}
