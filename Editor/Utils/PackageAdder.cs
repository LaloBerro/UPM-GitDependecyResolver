using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace GitDependecyResolvers
{
    public class PackageAdder
    {
        private AddRequest _request;

        public void Add(string url)
        {
            _request = Client.Add(url);

            EditorApplication.update += Progress;
        }

        private void Progress()
        {
            if (!_request.IsCompleted)
                return;

            if (_request.Status == StatusCode.Success)
                Debug.Log("Package Added : " + _request.Result.name + " | version: " + _request.Result.version);
            else if (_request.Status >= StatusCode.Failure)
                Debug.Log(_request.Error.message);

            EditorApplication.update -= Progress;
        }
    }
}