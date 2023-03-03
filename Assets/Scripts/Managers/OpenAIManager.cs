// sample code by unitycoder.com
// Edited by Adhan-Razzaque

using System;
using System.IO;
using Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Managers
{
    public class OpenAIManager : Singleton<OpenAIManager>
    {
        // OpenAI API endpoint
        private const string URL = "https://api.openai.com/v1/completions";

        // OpenAI model to use
        [SerializeField] private string modelName = "text-davinci-003";

        // Private state
        private string _apiKey = null;
        private bool _isRunning = false;

        private void Start()
        {
            LoadAPIKey();
        }

        public void Execute(string prompt, Action<string> responseHandler)
        {
            if (_isRunning)
            {
                Debug.LogError("Already running");
                return;
            }

            _isRunning = true;

            // fill in request data
            RequestData requestData = new RequestData()
            {
                model = modelName,
                prompt = prompt,
                temperature = 0.7f,
                max_tokens = 256,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var jsonData = JsonUtility.ToJson(requestData);

            var postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

            UnityWebRequest request = UnityWebRequest.Post(URL, jsonData);
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            var webRequest = request.SendWebRequest();

            webRequest.completed += (op) =>
            {
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
                    var generatedText = responseData.choices[0].text.TrimStart('\n').TrimStart('\n');

                    responseHandler?.Invoke(generatedText);
                }

                _isRunning = false;
            };
        } // execute

        private void LoadAPIKey()
        {
            // TODO optionally use from env.variable

            // MODIFY path to API key if needed
            var keyPath = Path.Combine(Application.streamingAssetsPath, "secretkey.txt");
            if (File.Exists(keyPath) == false)
            {
                Debug.LogError("Apikey missing: " + keyPath);
            }

            //Debug.Log("Load apikey: " + keyPath);
            _apiKey = File.ReadAllText(keyPath).Trim();
            Debug.Log("API key loaded, len= " + _apiKey.Length);
        }
    }
}