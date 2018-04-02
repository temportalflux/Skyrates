using System;
using UnityEditor;

namespace Skyrates.AI.State
{

    [CustomEditor(typeof(StateMachine))]
    public class StateMachineEditor : Editor
    {
        // Cached scriptable object editor
        private Editor editor = null;

        private StateMachine _instance;

        private static bool ToggleStateBlock = true;
        private static bool ToggleStates = true;
        private static bool[] ToggleStateEntries = new bool[0];
        private static bool ToggleTransitions = true;
        private static bool ToggleIdleTransitions = true;
        private static bool[] ToggleTransitionStates = new bool[0];
        private static bool[] ToggleIdleTransitionStates = new bool[0];
        private static bool[][] ToggleTransitionEntries = new bool[0][];

        void OnEnable()
        {
            this._instance = this.target as StateMachine;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            this.DrawScriptField(this._instance);

            EditorGUILayout.LabelField("Execution Frequency");
            {
                this._instance.ExecuteFrequency =
                    EditorGUILayout.FloatField("Frequency (Sec)", this._instance.ExecuteFrequency);
                this._instance.ScatterExecute =
                    EditorGUILayout.Toggle("Scatter Execution Start", this._instance.ScatterExecute);
            }

            EditorGUILayout.Separator();
            
            ToggleStateBlock = EditorGUILayout.Foldout(ToggleStateBlock, "States");
            if (ToggleStateBlock)
            {
                if (this._instance.IdleState == null)
                    this._instance.IdleState = new State(){StateName = "Idle"};
                this._instance.IdleState.Behavior =
                    (Behavior)EditorGUILayout.ObjectField("Idle", this._instance.IdleState.Behavior,
                    typeof(Behavior), allowSceneObjects:false);

                EditorGUILayout.Separator();

                EditorGUI.indentLevel++;
                if (this._instance.StateNames == null) this._instance.StateNames = new string[1]{"Idle"};
                this._instance.StateNames[0] = "Idle";

                ToggleStates = this.DrawArray("List", ref this._instance.States,
                    true, ToggleStates,
                    false, ref ToggleStateEntries,
                    DrawBlock: ((state, i) =>
                    {
                        if (this._instance.StateNames.Length != this._instance.States.Length + 1)
                            Array.Resize(ref this._instance.StateNames, this._instance.States.Length + 1);

                        //EditorGUILayout.BeginHorizontal();
                        {
                            if (state == null) state = new State();

                            //ToggleStateEntries[i] = EditorGUILayout.Foldout(ToggleStateEntries[i], "");

                            //EditorGUIUtility.labelWidth = 1;
                            state.StateName = EditorGUILayout.TextField(state.StateName);//, GUILayout.Width(100));

                            EditorGUI.indentLevel++;
                            state.Behavior = (Behavior) EditorGUILayout.ObjectField(
                                state.Behavior, typeof(Behavior), false);
                            EditorGUI.indentLevel--;
                            //EditorGUIUtility.labelWidth = 0;

                            this._instance.StateNames[i + 1] = state.StateName;
                        }
                        //EditorGUILayout.EndHorizontal();

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

                if (ToggleTransitionStates.Length != this._instance.States.Length)
                {
                    Array.Resize(ref ToggleTransitionStates, this._instance.States.Length);
                    Array.Resize(ref ToggleTransitionEntries, this._instance.States.Length);
                }

                {
                    bool[] entryToggles = ToggleIdleTransitionStates;
                    if (entryToggles == null)
                        entryToggles = new bool[0];
                    this.DrawTransitions("Idle",
                        this._instance.IdleState,
                        ref ToggleIdleTransitions,
                        ref this._instance.IdleState.Transitions,
                        ref ToggleIdleTransitionStates);
                    ToggleIdleTransitionStates = entryToggles;
                }

                for (int iState = 0; iState < this._instance.States.Length; iState++)
                {
                    State state = this._instance.States[iState];

                    if (state == null)
                    {
                        EditorGUILayout.LabelField("No state");
                        continue;
                    }

                    {
                        bool[] entryToggles = ToggleTransitionEntries[iState] ?? new bool[0];
                        this.DrawTransitions(
                            iState + ") " + state.StateName,
                            state,
                            ref ToggleTransitionStates[iState],
                            ref state.Transitions, ref entryToggles
                        );
                        ToggleTransitionEntries[iState] = entryToggles;
                    }
                }
                EditorGUI.indentLevel--;
            }

            //serializedObject.ApplyModifiedProperties();
            //Undo.RecordObject(this._stateMachine, string.Format("Edit {0}", this._stateMachine.name));
            EditorUtility.SetDirty(this._instance);

        }

        private void DrawTransitions(string label, State state,
            ref bool toggle, ref StateTransition[] elements, ref bool[] entryToggles)
        {
            StateTransition[] transitions = elements;
            bool[] allEntryToggles = entryToggles;
            toggle = this.DrawArray(
                label,
                ref transitions,
                true, toggle,
                false, ref allEntryToggles,
                GetFieldName: ((transition, i) =>
                {
                    if (transition == null) return "Null transition????";
                    
                    State stateDest = this._instance.GetState(state.TransitionDestinations[i]);
                    return state.StateName + " -> " +
                           (stateDest == null ? "Nil" : stateDest.StateName);
                }),
                DrawBlock: (transition, i) =>
                {
                    if (state.TransitionDestinations.Length != transitions.Length)
                        Array.Resize(ref state.TransitionDestinations, transitions.Length);

                    EditorGUI.indentLevel++;

                    this.DrawTransition(ref transition,
                        transition == null ? i.ToString() : state.StateName,
                        ref state.TransitionDestinations[i],
                        ref allEntryToggles[i]);

                    EditorGUI.indentLevel--;
                    return transition;
                }
            );
            entryToggles = allEntryToggles;
            elements = transitions;
        }

        private void DrawTransition(ref StateTransition transition, string stateName, ref int destination, ref bool toggled)
        {
            EditorGUILayout.BeginHorizontal();

            if (transition != null)
            {
                State stateDest = this._instance.GetState(destination);
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
                destination = EditorGUILayout.Popup(destination + 1, this._instance.StateNames) - 1;
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
