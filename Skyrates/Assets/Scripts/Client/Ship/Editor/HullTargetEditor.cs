﻿using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Client.Ship;
using UnityEditor;
using UnityEngine;

using ComponentType = ShipData.ComponentType;

namespace Skyrates.Client.Ship
{

    [CustomEditor(typeof(ShipHull))]
    public class HullTargetEditor : Editor
    {

        private ShipHull _instance;

        private static bool[] ToggleComp = new bool[ShipData.NonHullComponents.Length];
        private static bool ToggleLootMounts = false;

        public void OnEnable()
        {
            this._instance = this.target as ShipHull;
        }

        public override void OnInspectorGUI()
        {
            this.DrawScriptField(this._instance);

            if (this._instance.Mounts == null) this._instance.Mounts = new ShipHull.Mount[ShipData.NonHullComponents.Length];
            if (this._instance.Mounts.Length != ShipData.NonHullComponents.Length)
            {
                Array.Resize(ref this._instance.Mounts, ShipData.NonHullComponents.Length);
            }
            if (ToggleComp.Length != ShipData.NonHullComponents.Length)
                Array.Resize(ref ToggleComp, ShipData.NonHullComponents.Length);

            foreach (ComponentType compType in ShipData.NonHullComponents)
            {
                EditorGUILayout.Separator();

                int iComp = ShipData.HulllessComponentIndex[(int) compType];

                Transform[] roots = this._instance.Mounts[iComp].Roots;
                if (roots == null) roots = new Transform[0];

                ToggleComp[iComp] = this.DrawArray(
                    compType.ToString(), ref roots,
                    true, ToggleComp[iComp],
                    DrawBlock: (t => (Transform)EditorGUILayout.ObjectField(
                        t == null ? "Pos/Rot" : t.name,
                        t, typeof(Transform), allowSceneObjects:true
                    ))
                );

                this._instance.Mounts[iComp].Roots = roots;

            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            this.DropAreaGUI();

            ToggleLootMounts = this.DrawArray("Loot", ref this._instance.LootMounts, togglable:true, isToggled:ToggleLootMounts,
                DrawBlock:(mount =>
                {
                    mount = (Transform) EditorGUILayout.ObjectField("Pos/Rot/Scale", mount, typeof(Transform), allowSceneObjects: true);
                    return mount;
                })
            );

            EditorUtility.SetDirty(this._instance);
        }

        public void DropAreaGUI()
        {
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, "Drag loot set");

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        List<Transform> newLootArray = new List<Transform>();
                        foreach (object dragged_object in DragAndDrop.objectReferences)
                        {
                            if (dragged_object is GameObject)
                            {
                                newLootArray.Add((dragged_object as GameObject).transform);
                            }
                            else
                            {
                                Debug.Log(string.Format("{0} is no game object", dragged_object));
                            }
                        }
                        this._instance.LootMounts = newLootArray.ToArray();
                    }
                    break;
            }
        }

    }

}
