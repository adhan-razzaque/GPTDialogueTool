using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class Dialogue : MonoBehaviour
{
    public static Action OnDialogueStarted;
    public static Action OnDialogueFinished;

    public TextMeshProUGUI textComponent;

    public string line;

    public float charactersPerSecond;

    // private bool _finished;

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Debug.Log("Interacting");

        // if (!_finished)
        // {
        StopAllCoroutines();
        textComponent.text = line;
        DialogueFinished();
        //     return;
        // }

        // CloseDialog();
    }

    public void CloseDialog()
    {
        Destroy(gameObject);
    }

    private void DialogueFinished()
    {
        // _finished = true;
        OnDialogueFinished?.Invoke();
    }

    private void StartDialogue()
    {
        OnDialogueStarted?.Invoke();
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        var isTag = false;
        foreach (var c in line)
        {
            textComponent.text += c;

            isTag = c switch
            {
                '<' => true,
                '>' => false,
                _ => isTag
            };

            if (isTag) continue;

            yield return new WaitForSeconds(1 / charactersPerSecond);
        }

        DialogueFinished();
    }
}