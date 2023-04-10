using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Managers
{
    public class OpenAIChatManager: Singleton<OpenAIChatManager>
    {
         // OpenAI API endpoint
        private const string URL = "https://api.openai.com/v1/chat/completions";

        // OpenAI model to use
        [SerializeField] private string modelName = "gpt-3.5-turbo";

        // Private state
        private string _apiKey;
        private bool _isRunning;
        private bool _lockInput;
        private List<Message> _messages;
        private readonly Message _system = new Message() {content = "", role = "system"};

        private void OnEnable()
        {
            Dialogue.OnDialogueStarted += () => { _lockInput = true; };
            Dialogue.OnDialogueFinished += () => { _lockInput = false; };
        }

        private void Start()
        {
            LoadAPIKey();
        }

        public void ClearMessages()
        {
            _messages.RemoveRange(0,_messages.Count);
            _messages.Add(_system);
        }

        public void SetSystemMessage(string prompt)
        {
            _system.content = prompt;
        }

        public void Execute(string prompt, Action<string> responseHandler, bool storeMessage = true)
        {
            Debug.Log(prompt);

            if (_lockInput)
            {
                Debug.Log("Dialogue locked");
            }

            if (_isRunning)
            {
                Debug.LogError("Already running");
                return;
            }

            if (_apiKey.Length == 0)
            {
                Debug.LogError("No api key");
                return;
            }

            _isRunning = true;

            var message = new Message()
            {
                role = "user",
                content = prompt
            };
            
            if (storeMessage)
            {
                _messages.Add(message);
                StartCoroutine(SendRequest(_messages.ToArray(), responseHandler));
            }
            else
            {
                var messages = new Message[_messages.Count + 1];
                _messages.CopyTo(messages);
                messages[_messages.Count] = message;
                StartCoroutine(SendRequest(messages, responseHandler));
            }
        } // execute

        protected IEnumerator SendRequest(Message[] messages, Action<string> responseHandler)
        {
            // fill in request data
            var requestData = new ChatRequestData()
            {
                model = modelName,
                messages = messages
            };

            var jsonData = JsonUtility.ToJson(requestData);

            var postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

            using UnityWebRequest request = new UnityWebRequest(URL);
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);
            request.disposeUploadHandlerOnDispose = true;
            request.disposeDownloadHandlerOnDispose = true;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.error);
                responseHandler?.Invoke(null);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                // parse the results to get values 
                var responseData = JsonUtility.FromJson<OpenAIAPI>(request.downloadHandler.text);
                // sometimes contains 2 empty lines at start?
                var generatedText = responseData.choices[0].text.TrimStart('\n').TrimStart('\n').TrimStart('\"')
                    .TrimEnd('\"');

                responseHandler?.Invoke(generatedText);
            }

            _isRunning = false;

            request.Dispose();
            request.uploadHandler.Dispose();
            request.downloadHandler.Dispose();
        }

        private void LoadAPIKey()
        {
            // TODO optionally use from env.variable

            // MODIFY path to API key if needed
            var keyPath = Path.Combine(Application.streamingAssetsPath, "secretkey.txt");

#if UNITY_WEBGL
            StartCoroutine(RequestStreamingAssets(keyPath));
#else

            if (File.Exists(keyPath) == false)
            {
                Debug.LogError("Apikey missing: " + keyPath);
            }

            //Debug.Log("Load apikey: " + keyPath);
            _apiKey = File.ReadAllText(keyPath).Trim();
            Debug.Log("API key loaded, len= " + _apiKey.Length);
#endif
        }

        private IEnumerator RequestStreamingAssets(string uri)
        {
            using var webRequest = UnityWebRequest.Get(uri);
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    _apiKey = webRequest.downloadHandler.text.Trim();
                    Debug.Log("API key loaded, len= " + _apiKey.Length);
                    break;
            }
            
            webRequest.Dispose();
        }
    }
}