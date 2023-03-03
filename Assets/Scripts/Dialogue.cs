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

    public string line;

    public float charactersPerSecond;
    
    private bool _finished;
    
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

        if (!_finished)
        {
            StopAllCoroutines();
            textComponent.text = line;
            _finished = true;
            return;
        }
        
        Destroy(gameObject);
    }

    private void StartDialogue()
    {
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        foreach (var c in line)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(1/charactersPerSecond);
        }

        _finished = true;
    }
}
