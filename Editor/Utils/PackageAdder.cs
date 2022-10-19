using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace GitDependecyResolvers
{
    public static class PackageAdder
    {
        private static AddRequest _request;

        private static Queue<string> _urls = new Queue<string>();

        private static string _currentUrl;

        private static bool _isProcessing;

        public static void Add(string url)
        {
            _urls.Enqueue(url);

            if (!_isProcessing)
                DownloadNextPackage();
        }

        private static void DownloadNextPackage()
        {
            _isProcessing = true;

            _currentUrl = _urls.Dequeue();

            _request = Client.Add(_currentUrl);

            EditorApplication.update += Progress;
        }

        private static async void Progress()
        {
            EditorUtility.DisplayProgressBar("Resolving Git Dependency", "Downloading: " + _currentUrl, 0.5f);

            if (!_request.IsCompleted)
                return;

            if (_request.Status == StatusCode.Success)
                Debug.Log("Package Added : " + _request.Result.name + " | version: " + _request.Result.version);
            else if (_request.Status >= StatusCode.Failure)
                Debug.Log(_request.Error.message);

            AssetDatabase.Refresh();

            EditorApplication.update -= Progress;

            await Task.Delay(TimeSpan.FromSeconds(1));

            EditorUtility.ClearProgressBar();

            EditorApplication.update += ContinueWithNextDownload;
        }

        private static void ContinueWithNextDownload()
        {
            if (EditorApplication.isCompiling)
                return;

            if (_urls.Count > 0)
                DownloadNextPackage();
            else
                _isProcessing = false;

            EditorApplication.update -= ContinueWithNextDownload;
        }
    }
}