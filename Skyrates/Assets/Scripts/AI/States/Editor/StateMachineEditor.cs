using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.AI;
using UnityEditor;
using UnityEngine;

namespace Skyrates.Common.AI
{

    [CustomEditor(typeof(StateMachine))]
    public class StateMachineEditor : Editor
    {
        // Cached scriptable object editor
        private Editor editor = null;

        private StateMachine _stateMachine;

        private static bool ToggleStateBlock = true;
        private static bool ToggleStates = true;
        private static bool[] ToggleStateEntries = new bool[0];
        private static bool ToggleTransitions = true;
        private static bool[] ToggleTransitionStates = new bool[0];
        private static bool[][] ToggleTransitionEntries = new bool[0][];

        void OnEnable()
        {
            this._stateMachine = this.target as StateMachine;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            this.DrawScriptField(this._stateMachine);

            EditorGUILayout.LabelField("Execution Frequency");
            {
                this._stateMachine.ExecuteFrequency =
                    EditorGUILayout.FloatField("Frequency (Sec)", this._stateMachine.ExecuteFrequency);
                this._stateMachine.ScatterExecute =
                    EditorGUILayout.Toggle("Scatter Execution Start", this._stateMachine.ScatterExecute);
            }

            EditorGUILayout.Separator();
            
            ToggleStateBlock = EditorGUILayout.Foldout(ToggleStateBlock, "States");
            if (ToggleStateBlock)
            {
                this._stateMachine.IdleBehavior =
                    (Behavior)EditorGUILayout.ObjectField("Idle", this._stateMachine.IdleBehavior,
                    typeof(Behavior), allowSceneObjects:false);

                EditorGUILayout.Separator();

                EditorGUI.indentLevel++;
                if (this._stateMachine.StateNames == null) this._stateMachine.StateNames = new string[0];
                ToggleStates = this.DrawArray("List", ref this._stateMachine.States,
                    true, ToggleStates,
                    false, ref ToggleStateEntries,
                    DrawBlock: ((state, i) =>
                    {
                        if (this._stateMachine.StateNames.Length != this._stateMachine.States.Length)
                            Array.Resize(ref this._stateMachine.StateNames, this._stateMachine.States.Length);

                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();

                        if (state != null)
                        {
                            ToggleStateEntries[i] = EditorGUILayout.Foldout(ToggleStateEntries[i],
                                state.StateName);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(i.ToString());
                        }
                        
                        this.RenderEditor(ref state, ref ToggleStateEntries[i]);

                        this._stateMachine.StateNames[i] = state == null ? "INVALID" : state.StateName;

                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                        return state;
                    })
                );
                EditorGUI.indentLevel--;

            }

            EditorGUILayout.Separator();

            ToggleTransitions = EditorGUILayout.Foldout(ToggleTransitions, "Transitions");
            if (ToggleTransitions)
            {
                EditorGUI.indentLevel++;

                if (ToggleTransitionStates.Length != this._stateMachine.States.Length)
                {
                    Array.Resize(ref ToggleTransitionStates, this._stateMachine.States.Length);
                    Array.Resize(ref ToggleTransitionEntries, this._stateMachine.States.Length);
                }



                for (int iState = 0; iState < this._stateMachine.States.Length; iState++)
                {
                    State state = this._stateMachine.States[iState];

                    if (state == null)
                    {
                        EditorGUILayout.LabelField("No state");
                        continue;
                    }

                    bool[] entryToggles = ToggleTransitionEntries[iState];
                    if (entryToggles == null)
                        entryToggles = new bool[0];
                    
                    ToggleTransitionStates[iState] = this.DrawArray(
                        iState.ToString() + ") " + state.StateName,
                        ref state.Transitions,
                        true, ToggleTransitionStates[iState],
                        false, ref entryToggles,
                        GetFieldName: ((transition, i) =>
                        {
                            if (transition != null)
                            {
                                State stateDest = transition.GetStateDestination(this._stateMachine);
                                return state.StateName + " -> " +
                                       (stateDest == null ? "Nil" : stateDest.StateName);
                            }
                            else
                            {
                                return "Null transition????";
                            }
                        }),
                        DrawBlock: ((transition, i) =>
                        {
                            //if (ToggleTransitionEntries[iState].Length != state.Transitions.Length)
                            //   Array.Resize(ref ToggleTransitionEntries[iState], state.Transitions.Length);

                            EditorGUI.indentLevel++;
                            
                            this.DrawTransition(transition, transition == null ? i.ToString() : state.StateName, ref entryToggles[i]);

                            EditorGUI.indentLevel--;
                            return transition;
                        })
                    );
                    ToggleTransitionEntries[iState] = entryToggles;
                }
                EditorGUI.indentLevel--;
            }

            EditorUtility.SetDirty(this._stateMachine);
        }

        private void DrawTransition(StateTransition transition, string stateName, ref bool toggled)
        {
            EditorGUILayout.BeginHorizontal();

            if (transition != null)
            {
                State stateDest = transition.GetStateDestination(this._stateMachine);
                string str = stateName + " -> " +
                             (stateDest == null ? "NONE" : stateDest.StateName);
                toggled = EditorGUILayout.Foldout(toggled, str);
            }

            EditorGUILayout.EndHorizontal();

            transition = (StateTransition) EditorGUILayout.ObjectField(
                transition, typeof(StateTransition),
                allowSceneObjects: false);

            if (transition != null)
            {

                transition.StateDestination = EditorGUILayout.Popup(transition.StateDestination, this._stateMachine.StateNames);

            }
        }

        private void RenderEditor<T>(ref T data, ref bool toggled) where T:UnityEngine.Object
        {
            data = (T)EditorGUILayout.ObjectField(
                data, typeof(T),
                allowSceneObjects: false);

            if (toggled && data != null)
            {
                EditorGUI.indentLevel++;

                if (!editor)
                    Editor.CreateCachedEditor(data, null, ref editor);
                try
                {
                    editor.OnInspectorGUI();
                }
                catch (Exception)
                {
                    // ignored
                }

                EditorGUI.indentLevel--;
            }
        }

    }

}
