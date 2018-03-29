using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Skyrates.AI.Composite
{

    [CustomEditor(typeof(BehaviorPipeline))]
    public class EditorBehaviorPipeline : Editor
    {

        private BehaviorPipeline mInstance;
        private ReorderableList mListBehavior;

        void OnEnable()
        {
            this.mInstance = this.target as BehaviorPipeline;
            
            this.mListBehavior = null;
        }

        void OnDisable()
        {
            this.mListBehavior = null;
        }

        public override void OnInspectorGUI()
        {
            this.DrawScriptField(this.mInstance);

            if (this.mListBehavior == null)
            {
                this.SetupList();
            }
            this.mListBehavior.DoLayoutList();

            this.serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(this.mInstance, string.Format("Edit {0}", this.mInstance.name));
            EditorUtility.SetDirty(this.mInstance);

        }

        private void SetupList()
        {
            SerializedProperty prop = this.serializedObject.FindProperty("Behaviors");
            this.SetupList(
                "Behaviors", prop,
                (Rect rect) =>
                {
                },
                out this.mListBehavior
            );
        }

        protected void SetupList(string label,
            SerializedProperty elements,
            Action<Rect> drawHeader,
            out ReorderableList list)
        {
            ReorderableList retList = new ReorderableList(
                this.serializedObject, elements,
                true, true, true, true);

            float vSpace = 2;
            float hSpace = 3;
            float hBigSpace = EditorGUIUtility.singleLineHeight * 2 / 3;

            retList.drawHeaderCallback = (Rect rect) =>
            {
                GUIContent textLabel = new GUIContent(label);
                var textDimensions = GUI.skin.label.CalcSize(textLabel);
                textDimensions.x += 5;
                EditorGUI.LabelField(rect, textLabel);

                float boxWidth = rect.width - textDimensions.x;
                rect.x += rect.width - boxWidth;
                rect.width = boxWidth;
                drawHeader(rect);
            };
            retList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.x += 10; // for any possible foldout arrows
                rect.y += vSpace;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width -= hBigSpace;
                SerializedProperty element = retList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none);

                float oldWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = hBigSpace;
                rect.x += rect.width + hSpace; rect.width = hBigSpace;
                EditorGUIUtility.labelWidth = oldWidth;
            };
            

            list = retList;
        }

    }

}
