using System.IO;
using _01.Scripts.ObjectPool.Runtime;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace _01.Scripts.ObjectPool.Editor
{
    [CustomEditor(typeof(PoolingItemSO))]
    public class PoolingItemSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset = default;
        
        public override VisualElement CreateInspectorGUI()
        {
            CheckVisualTreeAsset();
            VisualElement root = new VisualElement();
            visualTreeAsset.CloneTree(root);
            
            TextField nameField = root.Q<TextField>("PoolingNameField");
            nameField.RegisterValueChangedCallback(HandleAssetNameChange);

            return root;
        }

        private void CheckVisualTreeAsset()
        {
            if (visualTreeAsset == null)
            {
                MonoScript script = MonoScript.FromScriptableObject(this);
                string scriptPath = AssetDatabase.GetAssetPath(script);
                string path = Path.GetDirectoryName(scriptPath).Replace("\\", "/") + "/PoolingItemSOeditor.uxml";
                visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            }
        }

        private void HandleAssetNameChange(ChangeEvent<string> evt)
        {
            if (string.IsNullOrEmpty(evt.newValue))
            {
                EditorUtility.DisplayDialog("Error", "Name cannot be empty", "OK");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(target); 
            string newName = $"{evt.newValue}"; 
            string message = AssetDatabase.RenameAsset(assetPath, newName); 

            if (string.IsNullOrEmpty(message))
            {
                target.name = newName;
            }
            else
            {
                ((TextField)evt.target).SetValueWithoutNotify(evt.previousValue);
                EditorUtility.DisplayDialog("Error", message, "OK");
            }
        }
    }
}

