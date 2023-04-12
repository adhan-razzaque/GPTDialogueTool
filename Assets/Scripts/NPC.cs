using Managers;
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

    private void Start()
    {
        _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        _isMainCanvasNull = _mainCanvas == null;
        if (_isMainCanvasNull) Debug.Log("Could not find main canvas.");

        SetNameTag();
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

    public void OnSubmit()
    {
        var prompt = npcDescriptor.BuildGptDescriptor(inputPrompt.text);
        // var prompt = inputPrompt.text;
        inputPrompt.text = "";
        OpenAIChatManager.Instance.SetSystemMessage(npcDescriptor.GetNpcString());
        OpenAIChatManager.Instance.Execute(prompt, OnResponseReceived, true);
    }
}
