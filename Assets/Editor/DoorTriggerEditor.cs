using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(DoorTrigger))]
public class DoorTriggerEditor : Editor
{
    SerializedProperty sceneToLoadProp;
    SerializedProperty playerTagProp;

    void OnEnable()
    {
        sceneToLoadProp = serializedObject.FindProperty("sceneToLoad");
        playerTagProp = serializedObject.FindProperty("playerTag");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Player tag field
        EditorGUILayout.PropertyField(playerTagProp);

        // Resolve current scene asset from the stored scene name (if any)
        string currentSceneName = sceneToLoadProp.stringValue;
        SceneAsset selected = null;
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            string[] guids = AssetDatabase.FindAssets(currentSceneName + " t:scene");
            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                if (Path.GetFileNameWithoutExtension(path) == currentSceneName)
                {
                    selected = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                    break;
                }
            }
        }

        // SceneAsset picker
        SceneAsset newScene = (SceneAsset)EditorGUILayout.ObjectField("Scene", selected, typeof(SceneAsset), false);
        if (newScene != selected)
        {
            if (newScene == null)
                sceneToLoadProp.stringValue = "";
            else
            {
                string path = AssetDatabase.GetAssetPath(newScene);
                sceneToLoadProp.stringValue = Path.GetFileNameWithoutExtension(path);
            }
        }

        // Show the saved scene name (read-only)
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(sceneToLoadProp, new GUIContent("Scene Name"));
        EditorGUI.EndDisabledGroup();

        // Show whether scene is in Build Settings and offer to add it
        bool inBuild = IsSceneInBuildSettings(sceneToLoadProp.stringValue);
        EditorGUILayout.LabelField("In Build Settings:", inBuild ? "Yes" : "No");
        if (!inBuild && GUILayout.Button("Add to Build Settings"))
        {
            AddSceneToBuildSettings(sceneToLoadProp.stringValue);
        }

        serializedObject.ApplyModifiedProperties();
    }

    bool IsSceneInBuildSettings(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return false;
        foreach (var s in EditorBuildSettings.scenes)
        {
            if (Path.GetFileNameWithoutExtension(s.path) == sceneName)
                return true;
        }
        return false;
    }

    void AddSceneToBuildSettings(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("No scene selected to add to Build Settings.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets(sceneName + " t:scene");
        if (guids.Length == 0)
        {
            Debug.LogWarning($"Could not find scene asset named '{sceneName}' to add to Build Settings.");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        if (!list.Exists(s => s.path == path))
        {
            list.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = list.ToArray();
            Debug.Log($"Added scene '{sceneName}' to Build Settings.");
        }
        else
        {
            Debug.Log($"Scene '{sceneName}' is already in Build Settings.");
        }
    }
}
