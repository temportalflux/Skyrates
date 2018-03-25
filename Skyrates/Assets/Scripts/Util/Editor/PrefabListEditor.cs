using System;
using Skyrates.Client.Util;
using Skyrates.Util;
using UnityEditor;
using UnityEngine;

public abstract class PrefabListEditor<TKey, TValue> : Editor where TValue: MonoBehaviour
{

    protected PrefabList Instance;

    public void OnEnable()
    {
        this.Instance = this.target as PrefabList;
    }

    /// <summary>
    /// Return a STATIC boolean array which represents if the foldout field blocks are open or closed
    /// </summary>
    /// <returns></returns>
    protected abstract bool[] GetStaticKeyToggleArray();

    /// <summary>
    /// Sets a STATIC boolean array which represents if the foldout field blocks are open or closed
    /// </summary>
    /// <returns></returns>
    protected abstract void SetStaticKeyToggleArray(bool[] keyBlocksToggled);

    protected virtual void PreDraw()
    {
    }

    public override void OnInspectorGUI()
    {
        this.DrawScriptField(this.Instance);

        this.PreDraw();

        PrefabList.Category[] categories = this.Instance.Categories
            ?? new PrefabList.Category[this.Instance.OrderedKeys.Length];
        if (categories.Length != this.Instance.OrderedKeys.Length)
            Array.Resize(ref categories, this.Instance.OrderedKeys.Length);

        // Get the list of which categories are already folded out
        bool[] keysEnabled = this.GetStaticKeyToggleArray() ?? new bool[categories.Length];
        if (keysEnabled.Length != categories.Length)
            Array.Resize(ref keysEnabled, categories.Length);
        
        foreach (TKey key in this.Instance.OrderedKeys)
        {
            // Get the index of the key
            int keyIndex = this.Instance.GetIndexFrom(key);

            if (categories[keyIndex] == null)
            {
                categories[keyIndex] = new PrefabList.Category();
            }

            // Get array of prefab slots in the instance
            // Ensure that the prefab list is valid
            MonoBehaviour[] values = categories[keyIndex].Prefabs ?? new MonoBehaviour[0];
            string[] names = categories[keyIndex].Names ?? new string[0];

            // Fetch the list of componets of this type
            //ShipComponentList.ComponentList components = this.instance.components[arrayIndex];
            //if (components.components == null) components.components = new ShipComponent[0];
            // Get the script class type of this component type
            //Type shipComponentType = ShipData.ComponentClassTypes[(int)compType];
            Type keyValueType = this.Instance.KeyValueTypes[keyIndex];

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            {
                keysEnabled[keyIndex] = EditorGUILayout.Foldout(
                    keysEnabled[keyIndex], key.ToString());
                
                GUILayout.FlexibleSpace();

                // Check resizing
                int prevSize = values.Length;
                int arrSize = EditorGUILayout.IntField(prevSize);

                if (GUILayout.Button("+"))
                {
                    arrSize++;
                }
                if (GUILayout.Button("-"))
                {
                    arrSize--;
                }

                if (prevSize != arrSize)
                {
                    Array.Resize(ref values, arrSize);
                    Array.Resize(ref names, arrSize);
                }

            }
            EditorGUILayout.EndHorizontal();

            if (keysEnabled[keyIndex])
            {
                for (int iValue = 0; iValue < values.Length; iValue++)
                {
                    MonoBehaviour prev = values[iValue];
                    string valueName = prev == null ? "None" : prev.name;
                    values[iValue] = (TValue)EditorGUILayout.ObjectField(
                        iValue + ") " + valueName, prev, keyValueType, allowSceneObjects:true);
                    names[iValue] = values[iValue] == null ? "None" : values[iValue].name;
                }
            }

            categories[keyIndex].Prefabs = values;
            categories[keyIndex].Names = names;
        }

        this.Instance.Categories = categories;
        this.SetStaticKeyToggleArray(keysEnabled);

        Undo.RecordObject(this.Instance, string.Format("Edit {0}", this.Instance.name));

    }


}
