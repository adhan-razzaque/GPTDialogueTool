using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class NPCSwitcher : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public GameObject npcPrefab;

    private NPCDescriptor[] _npcDescriptors;
    private GameObject[] _npcPrefabs;
    private GameObject _currentNpc;

    // Start is called before the first frame update
    private void Start()
    {
        if (!dropdown)
        {
            Debug.LogError("Missing dropdown");
        }

        if (!npcPrefab)
        {
            Debug.LogError("Missing NPC Prefab");
        }

        npcPrefab.SetActive(false);

        _npcDescriptors = Resources.LoadAll<NPCDescriptor>("NPC Descriptors");
        _npcPrefabs = new GameObject[_npcDescriptors.Length];

        dropdown.options.Clear();

        if (_npcDescriptors.Length == 0) return;

        for (var i = 0; i < _npcDescriptors.Length; ++i)
        {
            var npcDescriptor = _npcDescriptors[i];

            var npcPrefabInstance = Instantiate(npcPrefab, transform);
            npcPrefabInstance.SetActive(false);
            var npcRef = npcPrefabInstance.GetComponent<NPC>();
            npcRef.npcDescriptor = npcDescriptor;

            _npcPrefabs[i] = npcPrefabInstance;

            var option = new TMP_Dropdown.OptionData(npcDescriptor.npcName);
            dropdown.options.Add(option);
        }

        _currentNpc = _npcPrefabs[0];
        _currentNpc.SetActive(true);
    }

    private void OnEnable()
    {
        dropdown.onValueChanged.AddListener(HandleValueChanged);
    }

    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(HandleValueChanged);
    }

    private void HandleValueChanged(int index)
    {
        if (_currentNpc)
        {
            _currentNpc.SetActive(false);
        }

        _currentNpc = _npcPrefabs[index];
        _currentNpc.SetActive(true);
    }
}