using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class DestroyUnwantedScripts
{

    private static readonly Type[] typesToDeleteOnBuild = {
        typeof(GridLayoutGroup),
        typeof(VerticalLayoutGroup),
        typeof(HorizontalLayoutGroup)
        // etc
    };

    private static readonly string[] objectsToDeleteOnBuild =
    {
        "Debug"
    };

    [PostProcessScene]
    public static void DeleteObjects()
    {
        if (BuildPipeline.isBuildingPlayer)
        {
            /*foreach (var type in typesToDeleteOnBuild)
            {
                Debug.Log($"Destroying all instances of {type.Name} on build!");
                foreach (var obj in  FindObjectsOfTypeAll(type, true))
                {
                    Object.DestroyImmediate(obj);
                }
            }*/
            /*foreach (var tag in objectsToDeleteOnBuild)
            {
                Debug.Log($"Destroying all instances of {tag} on build!");
                foreach (var obj in FindObject(tag, true))
                {
                    Object.DestroyImmediate(obj);
                }
            }*/
        }
    }
    /// Use this method to get all loaded objects of some type, including inactive objects.
     /// This is an alternative to Resources.FindObjectsOfTypeAll (returns project assets, including prefabs), and GameObject.FindObjectsOfTypeAll (deprecated).
    public static List<Component> FindObjectsOfTypeAll(Type _type, bool _findInactive = false)
    {
        var results = new List<Component>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.isLoaded)
            {
                var allGameObjects = s.GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++)
                {
                    var go = allGameObjects[j];
                    results.AddRange(go.GetComponentsInChildren(_type, _findInactive));
                }
            }
        }
        Debug.Log($"Found {results.Count}");
        return results;
    }

    public static GameObject[] FindObject(string tag, bool _findInactive = false)
    {
        var results = GameObject.FindGameObjectsWithTag(tag);
        Debug.Log($"Found {results.Length}");
        return results;
    }
}
