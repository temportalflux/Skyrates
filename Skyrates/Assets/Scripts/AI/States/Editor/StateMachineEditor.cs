using System;
using UnityEditor;

namespace Skyrates.AI.State
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
        private static bool ToggleIdleTransitions = true;
        private static bool[] ToggleTransitionStates = new bool[0];
        private static bool[] ToggleIdleTransitionStates = new bool[0];
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
                if (this._stateMachine.IdleState == null)
                    this._stateMachine.IdleState = new State(){StateName = "Idle"};
                this._stateMachine.IdleState.Behavior =
                    (Behavior)EditorGUILayout.ObjectField("Idle", this._stateMachine.IdleState.Behavior,
                    typeof(Behavior), allowSceneObjects:false);

                EditorGUILayout.Separator();

                EditorGUI.indentLevel++;
                if (this._stateMachine.StateNames == null) this._stateMachine.StateNames = new string[1]{"Idle"};
                this._stateMachine.StateNames[0] = "Idle";

                ToggleStates = this.DrawArray("List", ref this._stateMachine.States,
                    true, ToggleStates,
                    false, ref ToggleStateEntries,
                    DrawBlock: ((state, i) =>
                    {
                        if (this._stateMachine.StateNames.Length != this._stateMachine.States.Length + 1)
                            Array.Resize(ref this._stateMachine.StateNames, this._stateMachine.States.Length + 1);

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

                            this._stateMachine.StateNames[i + 1] = state.StateName;
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

                if (ToggleTransitionStates.Length != this._stateMachine.States.Length)
                {
                    Array.Resize(ref ToggleTransitionStates, this._stateMachine.States.Length);
                    Array.Resize(ref ToggleTransitionEntries, this._stateMachine.States.Length);
                }

                {
                    bool[] entryToggles = ToggleIdleTransitionStates;
                    if (entryToggles == null)
                        entryToggles = new bool[0];
                    this.DrawTransitions("Idle", "Idle",
                        ref ToggleIdleTransitions,
                        ref this._stateMachine.IdleState.Transitions,
                        ref ToggleIdleTransitionStates);
                    ToggleIdleTransitionStates = entryToggles;
                }

                for (int iState = 0; iState < this._stateMachine.States.Length; iState++)
                {
                    State state = this._stateMachine.States[iState];

                    if (state == null)
                    {
                        EditorGUILayout.LabelField("No state");
                        continue;
                    }

                    {
                        bool[] entryToggles = ToggleTransitionEntries[iState];
                        if (entryToggles == null)
                            entryToggles = new bool[0];
                        this.DrawTransitions(
                            iState.ToString() + ") " + state.StateName,
                            state.StateName,
                            ref ToggleTransitionStates[iState],
                            ref state.Transitions, ref entryToggles
                        );
                        ToggleTransitionEntries[iState] = entryToggles;
                    }
                }
                EditorGUI.indentLevel--;
            }

            Undo.RecordObject(this._stateMachine, string.Format("Edit {0}", this._stateMachine.name));
        }

        private void DrawTransitions(string label, string stateName,
            ref bool toggle, ref StateTransition[] elements, ref bool[] entryToggles)
        {
            bool[] allEntryToggles = entryToggles;
            toggle = this.DrawArray(
                label,
                ref elements,
                true, toggle,
                false, ref allEntryToggles,
                GetFieldName: ((transition, i) =>
                {
                    if (transition == null) return "Null transition????";

                    State stateDest = transition.GetStateDestination(this._stateMachine);
                    return stateName + " -> " +
                           (stateDest == null ? "Nil" : stateDest.StateName);
                }),
                DrawBlock: (transition, i) =>
                {
                    EditorGUI.indentLevel++;

                    this.DrawTransition(ref transition, transition == null ? i.ToString() : stateName, ref allEntryToggles[i]);

                    EditorGUI.indentLevel--;
                    return transition;
                }
            );
            entryToggles = allEntryToggles;
        }

        private void DrawTransition(ref StateTransition transition, string stateName, ref bool toggled)
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

                transition.StateDestination = EditorGUILayout.Popup(transition.StateDestination + 1,
                    this._stateMachine.StateNames) - 1;

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
