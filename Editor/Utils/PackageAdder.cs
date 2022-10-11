using System.Collections.Generic;
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

        public static void Add(string url)
        {
            _urls.Enqueue(url);

            if (null == _request || _request.IsCompleted)
                DownloadNextPackage();
        }

        private static void DownloadNextPackage()
        {
            _currentUrl = _urls.Dequeue();

            _request = Client.Add(_currentUrl);

            EditorApplication.update += Progress;
        }

        private static void Progress()
        {
            EditorUtility.DisplayProgressBar("Download Package", "Downloading: " + _currentUrl, 0.5f);

            if (!_request.IsCompleted)
                return;

            if (_request.Status == StatusCode.Success)
                Debug.Log("Package Added : " + _request.Result.name + " | version: " + _request.Result.version);
            else if (_request.Status >= StatusCode.Failure)
                Debug.Log(_request.Error.message);

            EditorApplication.update -= Progress;

            if (_urls.Count > 0)
                DownloadNextPackage();

            EditorUtility.ClearProgressBar();
        }
    }
}