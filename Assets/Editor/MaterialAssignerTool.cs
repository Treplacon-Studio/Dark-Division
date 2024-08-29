#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MaterialAssignerTool : EditorWindow
{
    private GameObject targetObject;
    private List<PrefixMaterialPair> prefixMaterialPairs = new List<PrefixMaterialPair>();

    [MenuItem("Tools/Material Assigner Tool")]
    public static void ShowWindow()
    {
        GetWindow<MaterialAssignerTool>("Material Assigner Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Assigner Tool", EditorStyles.boldLabel);

        // Input field for the target object
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

        // List of prefix-material pairs
        GUILayout.Label("Prefix-Material Pairs", EditorStyles.boldLabel);

        if (prefixMaterialPairs.Count == 0)
        {
            prefixMaterialPairs.Add(new PrefixMaterialPair());
        }

        for (int i = 0; i < prefixMaterialPairs.Count; i++)
        {
            GUILayout.BeginHorizontal();
            prefixMaterialPairs[i].prefix = EditorGUILayout.TextField("Prefix", prefixMaterialPairs[i].prefix);
            prefixMaterialPairs[i].material = (Material)EditorGUILayout.ObjectField("Material", prefixMaterialPairs[i].material, typeof(Material), false);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                prefixMaterialPairs.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Prefix-Material Pair"))
        {
            prefixMaterialPairs.Add(new PrefixMaterialPair());
        }

        // Assign Materials button
        if (GUILayout.Button("Assign Materials"))
        {
            if (targetObject != null && prefixMaterialPairs.Count > 0)
            {
                AssignMaterials(targetObject.transform, prefixMaterialPairs);
            }
            else
            {
                Debug.LogWarning("Please specify a target object and at least one prefix-material pair.");
            }
        }
    }

    private void AssignMaterials(Transform parent, List<PrefixMaterialPair> pairs)
    {
        foreach (Transform child in parent)
        {
            foreach (var pair in pairs)
            {
                // Check if the child's name starts with the prefix
                if (child.name.StartsWith(pair.prefix))
                {
                    Renderer renderer = child.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.sharedMaterial = pair.material;
                        Debug.Log($"Assigned material: {pair.material.name} to {child.name} (matched prefix: {pair.prefix})");
                    }
                    else
                    {
                        Debug.LogWarning($"{child.name} does not have a Renderer component.");
                    }
                    break; // Break the loop once a matching prefix is found
                }
            }

            // Recursively check the child's children
            AssignMaterials(child, pairs);
        }
    }
}

[System.Serializable]
public class PrefixMaterialPair
{
    public string prefix;
    public Material material;
}
#endif