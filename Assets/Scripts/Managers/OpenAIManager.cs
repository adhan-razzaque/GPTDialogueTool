// sample code by unitycoder.com
// Edited by Adhan-Razzaque

using System;
using System.Collections;
using System.IO;
using Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Managers
{
    public class OpenAIManager : Singleton<OpenAIManager>, IGPTCompletion
    {
        // OpenAI API endpoint
        private const string URL = "https://api.openai.com/v1/completions";

        // OpenAI model to use
        [SerializeField] private string modelName = "text-davinci-003";

        // Private state
        private string _apiKey;
        private bool _isRunning;
        private bool _lockInput;

        private void OnEnable()
        {
            Dialogue.OnDialogueStarted += () => { _lockInput = true; };
            Dialogue.OnDialogueFinished += () => { _lockInput = false; };
            LoadAPIKey();
        }

        public void Execute(string prompt, Action<string> responseHandler, bool storeMessage = false)
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

            StartCoroutine(SendRequest(prompt, responseHandler));
        } // execute

        public OpenAI_API.OpenAIAPI GetOpenAIApi()
        {
            return string.IsNullOrEmpty(_apiKey) ? new OpenAI_API.OpenAIAPI() : new OpenAI_API.OpenAIAPI(_apiKey);
        }

        protected IEnumerator SendRequest(string prompt, Action<string> responseHandler)
        {
            // fill in request data
            var requestData = new RequestData()
            {
                model = modelName,
                prompt = prompt + "\n\"",
                temperature = 0.7f,
                max_tokens = 256,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
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

#if (UNITY_WEBGL && !UNITY_EDITOR)
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