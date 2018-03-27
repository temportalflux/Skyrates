﻿using UnityEngine;
using UnityEditor;

class VertexColorBlendingEditor_3Way_HeightBased : VertexColorBlendingEditorBase
{
    // The material properties:
    MaterialProperty color1, albedo1, normalmap1, mshao1, smoothness1, normalmapStrength1;
    MaterialProperty color2, albedo2, normalmap2, mshao2, smoothness2, normalmapStrength2;
    MaterialProperty color3, albedo3, normalmap3, mshao3, smoothness3, normalmapStrength3;
    MaterialProperty blend1, blend2;

    public override void OnEnable()
    {
        base.OnEnable();

        var targetObjects = serializedObject.targetObjects;
        if (targetObjects == null || targetObjects.Length == 0)
            return;

        color1 = GetMaterialProperty(targetObjects, 0);
        albedo1 = GetMaterialProperty(targetObjects, 1);
        normalmap1 = GetMaterialProperty(targetObjects, 2);
        mshao1 = GetMaterialProperty(targetObjects, 3);
        smoothness1 = GetMaterialProperty(targetObjects, 4);
        normalmapStrength1 = GetMaterialProperty(targetObjects, 5);

        color2 = GetMaterialProperty(targetObjects, 6);
        albedo2 = GetMaterialProperty(targetObjects, 7);
        normalmap2 = GetMaterialProperty(targetObjects, 8);
        mshao2 = GetMaterialProperty(targetObjects, 9);
        smoothness2 = GetMaterialProperty(targetObjects, 10);
        normalmapStrength2 = GetMaterialProperty(targetObjects, 11);

        color3 = GetMaterialProperty(targetObjects, 12);
        albedo3 = GetMaterialProperty(targetObjects, 13);
        normalmap3 = GetMaterialProperty(targetObjects, 14);
        mshao3 = GetMaterialProperty(targetObjects, 15);
        smoothness3= GetMaterialProperty(targetObjects, 16);
        normalmapStrength3 = GetMaterialProperty(targetObjects, 17);

        blend1 = GetMaterialProperty(targetObjects, 18);
        blend2 = GetMaterialProperty(targetObjects, 19);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLayerFieldsGUI(Color.white, "Background Layer", color1, albedo1, normalmap1, mshao1, smoothness1, normalmapStrength1);
        DrawLayerFieldsGUI(Color.red, "Red Layer", color2, albedo2, normalmap2, mshao2, smoothness2, normalmapStrength2);
        DrawLayerFieldsGUI(Color.green, "Green Layer", color3, albedo3, normalmap3, mshao3, smoothness3, normalmapStrength3);

        blend1.floatValue = EditorGUILayout.Slider("Red Blend", blend1.floatValue, -3, 1);
        blend2.floatValue = EditorGUILayout.Slider("Green Blend", blend2.floatValue, -3, 1);

        GUILayout.Space(5);

        DrawShaderPrepUtilButton();

        serializedObject.ApplyModifiedProperties();
    }
}

// Copyright (C) Glitched Polygons | Raphael Beck, 2017