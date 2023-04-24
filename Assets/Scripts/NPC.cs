using System;
using System.Threading.Tasks;
using Managers;
using OpenAI_API.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public NPCDescriptor npcDescriptor;
    // public TMP_InputField inputPrompt;
    // public Button button;
    // public TMP_Text nameTag;
    // public GameObject dialoguePrefab;
    //
    // private Canvas _mainCanvas;
    // private bool _isMainCanvasNull;

    private Conversation _chat;

    private void Start()
    {
        // _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        // _isMainCanvasNull = _mainCanvas == null;
        // if (_isMainCanvasNull) Debug.Log("Could not find main canvas.");

        ResetChat();
    }

    // private void SetNameTag()
    // {
    //     if (nameTag == null) return;
    //     if (npcDescriptor == null) return;
    //
    //     nameTag.text = npcDescriptor.npcName;
    // }

    // private void OnEnable()
    // {
    //     Dialogue.OnDialogueStarted += () => { button.interactable = false; };
    //     Dialogue.OnDialogueFinished += () => { button.interactable = true; };
    // }

    // private void OnResponseReceived(string response)
    // {
    //     Debug.Log($"Received response: {response}");
    //     var parentTransform = _isMainCanvasNull ? transform : _mainCanvas.transform;
    //     var newDialogue = Instantiate(dialoguePrefab, parentTransform).GetComponent<Dialogue>();
    //     newDialogue.line = $"{npcDescriptor.npcName}: {response}";
    //     button.interactable = true;
    //     inputPrompt.interactable = true;
    //     inputPrompt.text = "";
    // }

    public async Task<string> HandleInput(string prompt)
    {
        _chat.AppendUserInput(prompt);
        var response = await _chat.GetResponseFromChatbotAsync();
        Debug.Log(response);

        return response;
    }
    
    public async void HandleInput(string prompt, Action<string> callback)
    {
        _chat.AppendUserInput(prompt);
        var response = await _chat.GetResponseFromChatbotAsync();
        Debug.Log(response);

        callback.Invoke(response);
    }

    // public void OnSubmit()
    // {
    //     // var prompt = npcDescriptor.BuildGptDescriptor(inputPrompt.text);
    //     var prompt = inputPrompt.text;
    //     inputPrompt.interactable = false;
    //     button.interactable = false;
    //     // OpenAIChatManager.Instance.SetSystemMessage(npcDescriptor.GetNpcString());
    //     // OpenAIChatManager.Instance.Execute(prompt, OnResponseReceived, true);
    //     HandleInput(prompt);
    // }

    public void ResetChat()
    {
        var api = OpenAIManager.Instance.GetOpenAIApi();
        _chat = api.Chat.CreateConversation();

        // System message to define how to NPC interacts
        _chat.AppendSystemMessage("Engage in a natural dialogue as a character in a game. " +
                                  "Only respond as this character. The character is not aware they are in a game. " +
                                  "The game world is the character's reality. Only respond as a natural conversation. " +
                                  "You are not an AI, you are the character.  Your goal is to behave as the character. " +
                                  "Emulate the character's emotions, reactions, needs, and desires. " +
                                  "When asked about emotions, reactions, needs, or desires, respond with the character's. " +
                                  "Your character description is as follows: " + npcDescriptor.GetNpcString());

        // Provide some examples of the NPC interaction
        _chat.AppendUserInput("What is your name?");
        _chat.AppendExampleChatbotOutput($"My name is {npcDescriptor.npcName}.");
        _chat.AppendUserInput("How old are you?");
        _chat.AppendExampleChatbotOutput($"I am {npcDescriptor.age} years old.");
        _chat.AppendUserInput("What languages do you speak?");
        _chat.AppendExampleChatbotOutput("I speak " + string.Join(", ", npcDescriptor.knownLanguages));
        _chat.AppendUserInput("How are you feeling?");
        _chat.AppendExampleChatbotOutput($"I am feeling {npcDescriptor.GetAllMoods()}");
        _chat.AppendUserInput("");
        _chat.AppendExampleChatbotOutput("");

        // SetNameTag();
    }
}