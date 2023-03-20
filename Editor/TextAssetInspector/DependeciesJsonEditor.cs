#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using TextAsset = UnityEngine.TextAsset;

namespace GitDependecyResolvers
{
    [CustomEditor(typeof(TextAsset))]
    public class DependeciesJsonEditor : Editor
    {
        private bool _isEditModeActive;
        private Dependencies _dependencies;
        private TextAsset _textAsset;

        private void OnEnable()
        {
            _textAsset = (TextAsset)target;
            ResetDependencies();
        }

        private void ResetDependencies()
        {
            try
            {
                _dependencies = JsonUtility.FromJson<Dependencies>(_textAsset.text);
            }
            catch (System.Exception)
            {
            }
        }

        private void OnDisable()
        {
            _dependencies = null;
        }

        public override void OnInspectorGUI()
        {
            if (_textAsset.name != "Dependencies")
            {
                base.DrawDefaultInspector();
                return;
            }

            GUI.enabled = true;

            if (null == _dependencies)
                return;

            GUILayout.BeginVertical("Box");

            GUILayout.Label("Dependencies", EditorStyles.boldLabel);

            if (_isEditModeActive)
                DrawEditMode();
            else
                DrawReadOnlyMode();

            GUILayout.EndVertical();
        }

        private void DrawReadOnlyMode()
        {
            foreach (var dependency in _dependencies.dependencies)
            {
                GUILayout.BeginVertical("Box");

                GUILayout.Label(dependency.name);

                GUILayout.BeginHorizontal("Box");

                if (GUILayout.Button(EditorGUIUtility.IconContent("Clipboard"), GUILayout.Width(20), GUILayout.Height(20)))
                {
                    Debug.Log("Copied to Clipboard:  " + dependency.gitUrl);
                    EditorGUIUtility.systemCopyBuffer = dependency.gitUrl;
                }

                GUILayout.Label(dependency.gitUrl);

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            if (GUILayout.Button("Edit"))
                _isEditModeActive = true;
        }

        private void DrawEditMode()
        {
            for (int i = 0; i < _dependencies.dependencies.Count; i++)
            {
                Dependency dependency = _dependencies.dependencies[i];

                GUILayout.BeginHorizontal("Box");
                GUILayout.BeginVertical("Box");

                dependency.name = GUILayout.TextArea(dependency.name);
                dependency.gitUrl = GUILayout.TextArea(dependency.gitUrl);


                GUILayout.EndVertical();

                if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close"), GUILayout.Width(25)))
                {
                    _dependencies.dependencies.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
            {
                _dependencies.dependencies.Add(new Dependency());
            }

            if (GUILayout.Button("Apply"))
            {
                _isEditModeActive = false;
                ApplyChanges();
            }

            if (GUILayout.Button("Cancel"))
            {
                _isEditModeActive = false;
                ResetDependencies();
            }

            GUILayout.EndHorizontal();
        }

        private void ApplyChanges()
        {
            string json = JsonUtility.ToJson(_dependencies);

            string directoryPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            File.WriteAllText(directoryPath, json);

            AssetDatabase.Refresh();
        }
    }
}
#endif