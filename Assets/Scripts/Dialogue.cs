using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    public string[] lines;

    public float charactersPerSecond;

    private int _index;

    private bool _started;
    
    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interacting");
        if (!_started)
        {
            StartDialogue();
            return;
        }
        
        if (textComponent.text == lines[_index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = lines[_index];
        }
    }

    private void StartDialogue()
    {
        _index = 0;
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        foreach (var c in lines[_index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(1/charactersPerSecond);
        }
    }

    private void NextLine()
    {
        if (_index < lines.Length - 1)
        {
            ++_index;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
