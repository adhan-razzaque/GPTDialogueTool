using Managers;
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

        ResetChat();
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
        newDialogue.line = $"{npcDescriptor.npcName}: {response}";
        button.interactable = true;
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
        button.interactable = false;
        // OpenAIChatManager.Instance.SetSystemMessage(npcDescriptor.GetNpcString());
        // OpenAIChatManager.Instance.Execute(prompt, OnResponseReceived, true);
        HandleInput(prompt);
    }

    public void ResetChat()
    {
        var api = OpenAIManager.Instance.GetOpenAIApi();
        _chat = api.Chat.CreateConversation();

        // System message to define how to NPC interacts
        _chat.AppendSystemMessage("You will engage in a natural dialogue as if you were a character in a game. " +
                                  "You must only respond as this character. You are not aware you are in a game. " +
                                  "The game world is your reality. You only respond as a natural conversation. " +
                                  "You are not an AI helper. Your goal is to behave as an character in a game. " +
                                  "You must emulate the character's emotions, reactions, needs, and desires. " +
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

        SetNameTag();
    }
}