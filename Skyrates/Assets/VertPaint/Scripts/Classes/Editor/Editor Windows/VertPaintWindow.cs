using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace VertPaint
{
    /// <summary>
    /// VertPaint is a small vertex color painting utility that allows you
    /// to paint vertex colors onto your meshes. In combination with a good
    /// set of vertex color blending shaders you can achieve fantastic looking
    /// results like height-based blending of various materials, etc...      
    /// <para> </para>
    /// The complete source code is provided and is open for you to read, 
    /// understand, modify, extend or whatever other use you have for it.
    /// Feel free to integrate it and adapt it to your projects as you wish.
    /// You can for instance derive from this class, override stuff, subscribe 
    /// methods to its public events and react to stuff happening in the <see cref="VertPaintWindow"/>.
    /// It's all there, easily accessible, clean, modular and well formatted. I hope you enjoy it!
    /// </summary>
    public class VertPaintWindow : EditorWindow
    {
        #region Constants

        /// <summary>
        /// The current VertPaint version.
        /// </summary>
        public const float version = 1.0f;

        // Various default values for the VertPaint settings:
        const bool defaultEnabled = true;
        const bool defaultTogglePreview = false;
        const bool defaultHideTransformHandle = true;

        const float defaultRadius = 1.5f;
        const float defaultMaxRadius = 10.0f;
        const float defaultDelay = 0.05f;
        const float defaultOpacity = 0.25f;
        const float defaultAlpha = 0.2f;

        const BrushStyle defaultBrushStyle = BrushStyle.Disc;

        const KeyCode defaultPaintKey = KeyCode.Mouse0;
        const KeyCode defaultPreviewKey = KeyCode.C;
        const KeyCode defaultModifyRadiusKey = KeyCode.X;

        // Constant strings and paths that are used often throughout this document:
        const string trueString = "true";
        const string falseString = "false";
        const string defaultMeshOutputDirectory = "Assets/VertPaint/Output/Meshes/";
        const string defaultAutosaveDirectory = "Assets/VertPaint/Templates/Autosave/";
        const string helpText = "VertPaint is a simple vertex color painting utility that ships with a set of simple example vertex color blending shaders, a shader prep utility to prepare your texture maps for your materials and some example scenes.\n\nTo paint vertex colors on your currently selected mesh, press the specified paint input key (default is the left mouse button). The vertices inside the brush area will be colored according to the specified falloff, opacity, color and delay value. Hover your mouse cursor over each setting’s label for more information about what each setting does.\n\nTo erase the painted vertex colors, paint with the shift key down. You can also start from scratch by clicking on \"Discard\". Changing the brush radius can be achieved interactively inside the scene view by keeping the \"Modify radius key\" down and dragging the mouse cursor; once happy, let go of the key and the sampled value will be your new brush radius.\n\nYou can also preview the vertex colors by pressing and holding the \"Preview vertex colors\" key.\n\nIf you’re happy with your result, click on \"Apply\" and the painted vertex colors will be automatically saved out to a mesh asset on disk (which is then highlighted inside the project view for you).\n\nSince you can only work on one mesh simultaneously, the controls for selecting GameObjects are slightly different than the standard Unity editor controls: to change the mesh you’re currently working on, select the new one in the scene view whilst holding down control (command on Mac). Note that by reselecting (or also by closing the VertPaint window), the changes you've made to the vertex colors will be automatically discarded.\nWhenever you come up with a result you like, hit apply! It doesn't cost you anything. You can always clean up later with the \"Clean up\" button.";

        static readonly Color defaultColor = Color.red;
        static readonly AnimationCurve defaultFalloff = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

        // XmlWriterSettings and XmlReaderSettings for saving/loading VertPaint templates.
        static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { Indent = true, CloseOutput = true };
        static readonly XmlReaderSettings xmlReaderSettings = new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true };

        // GUIContent labels and tooltips for the VertPaint window:
        readonly GUIContent enabledGUIContent = new GUIContent("Enabled", "This toggle enables and disables the brush. A disabled brush won't appear in the scene view and you'll not be able to paint any vertex colors with it.");
        readonly GUIContent togglePreviewGUIContent = new GUIContent("Toggle preview", "Should previewing the vertex colors be triggered by holding down the preview key or toggling on/off?");
        readonly GUIContent hideTransformHandleGUIContent = new GUIContent("Hide transform handle", "Hide/unhide the transform handle in the scene view.");
        readonly GUIContent helpGUIContent = new GUIContent("Help", "Click here to learn how to use VertPaint.");
        readonly GUIContent radiusGUIContent = new GUIContent("Radius", "The radius is defined in meters and affects the size of the paint brush.");
        readonly GUIContent delayGUIContent = new GUIContent("Delay", "The minimum delay in seconds between paint strokes.");
        readonly GUIContent opacityGUIContent = new GUIContent("Opacity", "The opacity value controls the maximum intensity of the painted colors. Maximum opacity will result in fully opaque (full alpha) colors, whereas a value of 50% would halve the intensity of the colors (thus making them blander).");
        readonly GUIContent falloffGUIContent = new GUIContent("Falloff", "The falloff determines how the painted vertex color's opacity behaves in relation to the distance to the brush center. The painted color's alpha is multiplied by the evaluated falloff value. On the curve, t=0 represents the center of the brush and t=1 its outermost edge.");
        readonly GUIContent colorGUIContent = new GUIContent("Color", "The vertex color to paint with the brush.");
        readonly GUIContent styleGUIContent = new GUIContent("Style", "The style defines the esthetic appearance of the brush.");
        readonly GUIContent alphaGUIContent = new GUIContent("Alpha", "This is the transparency of the brush object and does not affect the final vertex colors. It's only there to help you see better whilst painting.");
        readonly GUIContent meshOutputDirectoryGUIContent = new GUIContent("Mesh output directory: ", "This is the directory where VertPaint will store the vertex colored mesh assets.");
        readonly GUIContent paintKeyGUIContent = new GUIContent("Paint", "This is the key used for painting.");
        readonly GUIContent modifyRadiusKeyGUIContent = new GUIContent("Modify radius", "Keep this key pressed and drag the mouse away from and to the center of the brush to adjust its radius.");
        readonly GUIContent previewKeyGUIContent = new GUIContent("Preview vertex colors", "Press this key (or hold it, depending on the setting in the top right corner of the window) to preview the painted vertex colors on the mesh.");
        readonly GUIContent removeFavButtonGUIContent = new GUIContent("-", "Remove the last (bottom-most) entry from the list.");
        readonly GUIContent clearFavsButtonGUIContent = new GUIContent("C", "Clear the list of favorite templates.");
        readonly GUIContent fillGUIContent = new GUIContent("Fill", "Fill all vertex colors with the current brush color. Hold down shift to fill with clear black (0,0,0,0).");
        readonly GUIContent invertGUIContent = new GUIContent("Invert", "Invert all vertex colors.");
        readonly GUIContent discardGUIContent = new GUIContent("Discard", "Discard the changes made to the vertex colors.");
        readonly GUIContent applyGUIContent = new GUIContent("Apply", "Apply the painted vertex colors by saving them out to an asset on disk (inside the specified mesh output directory).");
        readonly GUIContent resetGUIContent = new GUIContent("Reset", "Revert all VertPaint settings back to their default values, except for the favorite templates list (which you can clear with the C button if you want).");
        readonly GUIContent cleanMeshOutputDirectoryGUIContent = new GUIContent("Clean up", "Clean up the mesh output directory by moving unreferenced and unneeded mesh assets in it into a sub-folder called \"_old\".");

        #endregion

        #region Events

        /// <summary>
        /// The <see cref="Painted"/> event is raised on each paint stroke and 
        /// the passed <see cref="PaintStrokeEventArgs"/> argument contains information 
        /// about the affected mesh as well as the paint stroke itself.<para> </para>
        /// To access the paint stroke related brush settings, cast the "object sender"
        /// to the <see cref="VertPaintWindow"/> type and access its public properties (such as <see cref="VertPaintWindow.Radius"/>, <see cref="VertPaintWindow.Opacity"/>, etc...).
        /// </summary>
        public event EventHandler<PaintStrokeEventArgs> Painted;

        /// <summary>
        /// The <see cref="Erased"/> event is raised every time the user erases vertex colors with the brush.<para> </para>
        /// The passed <see cref="PaintStrokeEventArgs"/> argument contains information about the affected mesh as well as the erase stroke itself.
        /// </summary>
        public event EventHandler<PaintStrokeEventArgs> Erased;

        /// <summary>
        /// The <see cref="PreviewStateChanged"/> event is raised whenever the vertex color preview shader has been enabled/disabled.
        /// </summary>
        public event EventHandler<PreviewStateChangedEventArgs> PreviewStateChanged;

        /// <summary>
        /// This event is raised every time a template has been saved.
        /// </summary>
        public event EventHandler<TemplateSavedEventArgs> TemplateSaved;

        #endregion

        #region Paths

        /// <summary>
        /// The full path to the VertPaint autosave file 
        /// (this depends on the <see cref="AutosaveDirectory"/>).
        /// </summary>
        public string AutosaveFilePath
        {
            get { return autosaveDirectory + "VertPaint Autosave.xml"; }
        }

        /// <summary>
        /// The full path to the file that contains  
        /// the list of favorite VertPaint templates.
        /// </summary>
        public string FavoriteTemplatesFilePath
        {
            get { return autosaveDirectory + "VertPaint Favorite Templates.xml"; }
        }

        #endregion

        #region Foldout states

        [SerializeField]
        bool showHelp;

        [SerializeField]
        bool showBrushSettings = true;

        [SerializeField]
        bool showKeyBindings = true;

        [SerializeField]
        bool showTemplates = true;

        #endregion

        #region Main toggles

        [SerializeField]
        bool enabled = defaultEnabled;
        /// <summary>
        /// This toggle enables and disables the brush.<para> </para> 
        /// A disabled brush won't appear in the scene view and you'll not be able to paint any vertex colors with it.
        /// </summary>
        public bool Enabled { get { return enabled; } }

        [SerializeField]
        bool hideTransformHandle = defaultHideTransformHandle;
        /// <summary>
        /// Hide/unhide the transform handle inside the scene view.
        /// </summary>
        public bool HideTransformHandle { get { return hideTransformHandle; } }

        [SerializeField]
        bool togglePreview = defaultTogglePreview;
        /// <summary>
        /// Should previewing the vertex colors be triggered by holding down the preview key or toggling it on/off?
        /// </summary>
        public bool TogglePreview { get { return togglePreview; } }

        #endregion

        #region Brush settings

        /// <summary>
        /// The last time the user painted with this <see cref="VertPaintWindow"/> instance.
        /// </summary>
        public DateTime LastPaintTime { get; private set; }

        [SerializeField]
        float radius = defaultRadius;
        /// <summary>
        /// The radius is defined in meters and affects the size of the paint brush.
        /// </summary>
        public float Radius { get { return radius; } }

        [SerializeField]
        float maxRadius = defaultMaxRadius;
        /// <summary>
        /// The maximum brush radius size (in meters).
        /// </summary>
        public float MaxRadius { get { return maxRadius; } }

        [SerializeField]
        float delay = defaultDelay;
        /// <summary>
        /// The minimum delay in seconds between paint strokes.
        /// </summary>
        public float Delay { get { return delay; } }

        [SerializeField]
        float opacity = defaultOpacity;
        /// <summary>
        /// The opacity value controls the maximum intensity of the painted colors.<para> </para> 
        /// Maximum opacity will result in fully opaque (full alpha) colors, 
        /// whereas a value of 50% would halve the intensity of the colors 
        /// (thus making each paint stroke blander).
        /// </summary>
        public float Opacity { get { return opacity; } }

        [SerializeField]
        AnimationCurve falloff = defaultFalloff;
        /// <summary>
        /// The falloff curve controls the opacity 
        /// of the painted vertex colors in relation to 
        /// their distance to the brush's center.<para> </para> 
        /// t = 0 represents the center of the brush.<para> </para> 
        /// t = 1 translates to the outer edge of the brush area.<para> </para>
        /// By default, this is a linear falloff that circularly fades out the opacity the further away you go from the brush's center.
        /// </summary>
        public AnimationCurve Falloff { get { return falloff; } }

        [SerializeField]
        Color color = Color.red;
        /// <summary>
        /// The vertex color to paint.
        /// </summary>
        public Color Color { get { return color; } }

        [SerializeField]
        BrushStyle style = defaultBrushStyle;
        /// <summary>
        /// The esthetic appearance of the brush inside the scene view.
        /// </summary>
        public BrushStyle Style { get { return style; } }

        [SerializeField]
        float alpha = defaultAlpha;
        /// <summary>
        /// The alpha of the used brush (0 = fully transparent, 1 = fully opaque). This only affects the appearance of the brush!
        /// </summary>
        public float Alpha { get { return alpha; } }

        [SerializeField]
        string meshOutputDirectory = defaultMeshOutputDirectory;
        /// <summary>
        /// The project-local path to the directory where VertPaint should deposit the applied vertex color mesh assets.
        /// </summary>
        public string MeshOutputDirectory { get { return meshOutputDirectory; } }

        #endregion

        #region Key bindings

        [SerializeField]
        KeyCode paintKey = defaultPaintKey;
        /// <summary>
        /// The <see cref="KeyCode"/> for painting vertex colors. Hold shift pressed whilst painting to erase.
        /// </summary>
        public KeyCode PaintKey { get { return paintKey; } }

        [SerializeField]
        KeyCode modifyRadiusKey = defaultModifyRadiusKey;
        /// <summary>
        /// The <see cref="KeyCode"/> for modifying the VertPaint brush radius.
        /// </summary>
        public KeyCode ModifyRadiusKey { get { return modifyRadiusKey; } }

        [SerializeField]
        KeyCode previewKey = defaultPreviewKey;
        /// <summary>
        /// The <see cref="KeyCode"/> for enabling/disabling the vertex colors preview shader.
        /// </summary>
        public KeyCode PreviewKey { get { return previewKey; } }

        #endregion

        #region Templates

        [SerializeField]
        string autosaveDirectory = defaultAutosaveDirectory;
        /// <summary>
        /// The project-local path to the directory where VertPaint will deposit its autosave and list of favorite templates.
        /// </summary>
        public string AutosaveDirectory { get { return autosaveDirectory; } }

        #endregion

        #region Private 

        Material previewMaterial;
        Material sphereBrushMaterial;

        [SerializeField]
        List<string> favoriteTemplates = new List<string>(10);

        Tool lastTool;

        Action brushModeAction;
        Action changeSelectionAction;

        bool holdingPaintKey;
        bool holdingPreviewKey;
        bool previewingVertexColors;
        bool selectionChanged;
        bool tempCollider;

        // ScrollView position vectors 
        // (for the scrollbars in the VertPaint window).
        Vector2 mainScrollPosition;
        Vector2 favoritesListScrollPosition;

        // The location where the radius sampling procedure started.
        Vector2 radiusSamplingMousePos;
        RaycastHit radiusSamplingRaycastHit;

        // The sphere brush model's transform.
        Transform sphereBrush;

        // The various cached components 
        // of the currently selected mesh:
        Transform selectedTransform;
        MeshFilter selectedMeshFilter;
        MeshRenderer selectedMeshRenderer;
        MeshCollider selectedMeshCollider;

        // GUIContent icons:
        GUIContent addFavButtonGUIContent = null;
        GUIContent templateSlotGUIContent = null;
        GUIContent saveGUIContent = null;

        #endregion

        //-------------------------------------------------------------------------------------

        /// <summary>
        /// Opens a VertPaint window.
        /// </summary>
        [MenuItem("Window/VertPaint %#v")]
        public static void Open()
        {
            // Get an existing open window or, if there is none, make a new one.
            var window = GetWindow(typeof(VertPaintWindow), true) as VertPaintWindow;
            if (window != null)
            {
                window.Show();
            }

            // Configure the window's size and title.
            window.titleContent = new GUIContent("VertPaint");
            window.minSize = new Vector2(387, 34);
            window.maxSize = new Vector2(750, 563);
        }

        /// <summary>
        /// Reverts all VertPaint settings to their default values.
        /// </summary>
        public void Reset()
        {
            // Revert foldouts:
            showHelp = false;
            showBrushSettings = showKeyBindings = showTemplates = true;

            // Revert brush settings:
            enabled = defaultEnabled;
            togglePreview = defaultTogglePreview;
            hideTransformHandle = defaultHideTransformHandle;
            radius = defaultRadius;
            maxRadius = defaultMaxRadius;
            delay = defaultDelay;
            opacity = defaultOpacity;
            falloff = defaultFalloff;
            color = defaultColor;
            style = defaultBrushStyle;
            alpha = defaultAlpha;
            meshOutputDirectory = defaultMeshOutputDirectory;

            // Revert key bindings:
            paintKey = defaultPaintKey;
            modifyRadiusKey = defaultModifyRadiusKey;
            previewKey = defaultPreviewKey;

            // Revert template settings:
            autosaveDirectory = defaultAutosaveDirectory;

            Repaint();
        }

        void OnEnable()
        {
            lastTool = Tools.current;
            LastPaintTime = DateTime.Now;

            GatherMeshComponents();

            #region Inspector initialization

            CollapseMeshRendererAndCollider();

            #endregion

            #region GUIContent initialization

            if (saveGUIContent == null)
            {
                saveGUIContent = new GUIContent(EditorIcons.SaveIcon, "Save the current VertPaint configuration out to a template file.");
            }

            if (templateSlotGUIContent == null)
            {
                templateSlotGUIContent = new GUIContent(EditorIcons.TemplateSlot, "Load a VertPaint template file by dragging it into this field. Only valid template files with the correct format can be loaded! Loading will overwrite the current settings.");
            }

            if (addFavButtonGUIContent == null)
            {
                addFavButtonGUIContent = new GUIContent(EditorIcons.AddIcon, "Add a VertPaint template to your favourites list below by dragging and dropping it into this field, or by clicking here and selecting one manually.");
            }

            #endregion

            #region Preview material initialization

            if (previewMaterial == null)
            {
                previewMaterial = new Material(Shader.Find("VertPaint/Unlit Vertex Colors"))
                {
                    renderQueue = 100000,
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            #endregion

            #region Sphere brush initialization

            if (sphereBrushMaterial == null)
            {
                sphereBrushMaterial = new Material(Shader.Find("Standard"));
                sphereBrushMaterial.hideFlags = HideFlags.HideAndDontSave;

                sphereBrushMaterial.SetFloat("_Metallic", 0.0f);
                sphereBrushMaterial.SetFloat("_Glossiness", 0.0f);
                sphereBrushMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                sphereBrushMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                sphereBrushMaterial.SetInt("_ZWrite", 0);
                sphereBrushMaterial.DisableKeyword("_ALPHATEST_ON");
                sphereBrushMaterial.DisableKeyword("_ALPHABLEND_ON");
                sphereBrushMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                sphereBrushMaterial.renderQueue = 3000;
            }

            if (sphereBrush == null)
            {
                sphereBrush = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                sphereBrush.gameObject.hideFlags = HideFlags.HideAndDontSave;

                var collider = sphereBrush.GetComponent<Collider>();
                if (collider != null)
                {
                    DestroyImmediate(collider);
                }

                var renderer = sphereBrush.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterials = new Material[] { sphereBrushMaterial };
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                sphereBrush.gameObject.SetActive(false);
            }

            #endregion

            #region Actions initialization

            // Default brush mode is paint.
            if (brushModeAction == null)
            {
                brushModeAction = BrushMode_Paint;
            }

            if (changeSelectionAction == null)
            {
                changeSelectionAction = () => selectionChanged = true;
            }

            #endregion

            #region I/O initialization

            // Load the autosave directory path from the EditorPrefs.
            autosaveDirectory = EditorPrefs.GetString("vp_autosave_dir", defaultAutosaveDirectory);

            // Load up the last autosave (or revert settings to their
            // default values if there's no autosave file available).
            if (!Load(AutosaveFilePath))
            {
                Reset();
            }

            LoadFavoriteTemplates();

            #endregion

            #region Event and delegate subscriptions

            if (SceneView.onSceneGUIDelegate != null)
            {
                SceneView.onSceneGUIDelegate -= OnSceneGUI;
            }
            SceneView.onSceneGUIDelegate += OnSceneGUI;

            Selection.selectionChanged += changeSelectionAction;
            PreviewStateChanged += VertPaintWindow_PreviewStateChanged;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            #endregion
        }

        void OnDisable()
        {
            // Set the brush mode action to null in order to 
            // avoid its accidental invocation from inside 
            // OnSceneGUI after the EditorWindow's closure.
            brushModeAction = null;

            // Remove the sphere brush object in the scene
            // as it's not needed when VertPaint isn't open.
            if (sphereBrush != null)
            {
                DestroyImmediate(sphereBrush.gameObject);
            }

            // Destroy any assets created in OnEnable to clean up.
            if (previewMaterial != null)
            {
                DestroyImmediate(previewMaterial);
            }

            if (sphereBrushMaterial != null)
            {
                DestroyImmediate(sphereBrushMaterial);
            }

            // Avoid leaving the vertex colors preview 
            // shader on the mesh when leaving VertPaint.
            if (previewingVertexColors)
            {
                OnPreviewStateChanged(new PreviewStateChangedEventArgs(false));
            }

            // If the MeshCollider was temporary, destroy it.
            // VertPaint shouldn't leave any noticeable traces!
            if (tempCollider && selectedMeshCollider != null)
            {
                DestroyImmediate(selectedMeshCollider);
            }

            // Autosave on close.
            Save(AutosaveFilePath);
            SaveFavoriteTemplates();

            // Set the tool back to what it was before opening VertPaint.
            Tools.current = lastTool;

            // Save the autosave directory path to the editor prefs 
            // (otherwise we lose that information between VertPaint sessions).
            EditorPrefs.SetString("vp_autosave_dir", autosaveDirectory);

            // Discard the painted vertex colors 
            // (if you want to keep them, apply them!).
            DiscardPaintedVertexColors();

            // Unsubscribe events (clean up before leaving).
            PreviewStateChanged -= VertPaintWindow_PreviewStateChanged;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            Selection.selectionChanged -= changeSelectionAction;

            // Clean up.
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        void OnUndoRedoPerformed()
        {
            if (selectedTransform != null)
            {
                Selection.activeTransform = selectedTransform;
            }

            if (selectedMeshRenderer != null && selectedMeshRenderer.additionalVertexStreams != null)
            {
                selectedMeshRenderer.additionalVertexStreams.colors = selectedMeshRenderer.additionalVertexStreams.colors;
            }

            Focus();
            Repaint();
            SceneView.RepaintAll();
        }

        void OnGUI()
        {
            Undo.RecordObject(this, "change VertPaint settings");

            EditorGUIUtility.labelWidth = 70.0f;
            GUILayout.Space(6.0f);

            EditorGUILayout.BeginVertical(GUILayout.MaxHeight(556.0f));
            {
                mainScrollPosition = GUILayout.BeginScrollView(mainScrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
                {
                    if (EditorApplication.isPlaying)
                    {
                        EditorGUILayout.HelpBox("Warning:\n\nCan't paint vertex colors when in play mode.\n\nExit play mode first and then hold down control (command on Mac) and left-mouse click on the desired mesh to start painting vertex colors.\n\nCheck out the help section or Documentation.pdf for more information.", MessageType.Warning);
                    }
                    else if (selectedTransform == null || selectedMeshFilter == null || selectedMeshRenderer == null || selectedMeshCollider == null)
                    {
                        EditorGUILayout.HelpBox("Warning:\n\nNo mesh selected.\n\nHold down control (command on Mac) and left-mouse click on the desired mesh to start painting vertex colors.\n\nCheck out the help section or Documentation.pdf for more information.", MessageType.Warning);
                    }
                    else
                    {
                        GUILayout.Space(1.0f);

                        #region Main settings

                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.color = enabled ? Color.green : Color.red;
                            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false), GUILayout.Width(50.0f));
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    enabled = EditorGUILayout.Toggle(GUIContent.none, enabled, GUILayout.Width(15.0f));
                                    EditorGUILayout.LabelField(enabledGUIContent, GUILayout.Width(50.0f), GUILayout.ExpandWidth(false));
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();
                            GUI.color = Color.white;

                            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false), GUILayout.Width(55.0f));
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    togglePreview = EditorGUILayout.Toggle(GUIContent.none, togglePreview, GUILayout.Width(15.0f));
                                    EditorGUILayout.LabelField(togglePreviewGUIContent, GUILayout.Width(95.0f), GUILayout.ExpandWidth(false));
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false), GUILayout.Width(55.0f));
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUI.BeginChangeCheck();
                                    hideTransformHandle = EditorGUILayout.Toggle(GUIContent.none, hideTransformHandle, GUILayout.Width(15.0f));
                                    EditorGUILayout.LabelField(hideTransformHandleGUIContent, GUILayout.Width(132.0f), GUILayout.ExpandWidth(false));
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        SceneView.RepaintAll();
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        #endregion

                        GUILayout.Space(1.0f);

                        #region Help section

                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                            if (GUILayout.Button(helpGUIContent, VertPaintGUI.BoldLabelButtonStyle))
                            {
                                showHelp = !showHelp;
                            }
                            GUI.color = Color.white;

                            if (showHelp)
                            {
                                EditorGUILayout.HelpBox(helpText, MessageType.None);
                            }
                        }
                        EditorGUILayout.EndVertical();

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Brush settings

                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                            if (GUILayout.Button("Brush Settings", VertPaintGUI.BoldLabelButtonStyle))
                            {
                                showBrushSettings = !showBrushSettings;
                            }
                            GUI.color = Color.white;

                            if (showBrushSettings)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    radius = EditorGUILayout.Slider(radiusGUIContent, radius, 0.01f, maxRadius);
                                    maxRadius = EditorGUILayout.FloatField(GUIContent.none, maxRadius, GUILayout.Width(30.0f));
                                }
                                EditorGUILayout.EndHorizontal();

                                delay = EditorGUILayout.Slider(delayGUIContent, delay, 0.01f, 1.0f);
                                opacity = EditorGUILayout.Slider(opacityGUIContent, opacity, 0.0f, 1.0f);

                                EditorGUILayout.BeginHorizontal();
                                {
                                    falloff = EditorGUILayout.CurveField(falloffGUIContent, falloff, Color.green, new Rect(0.0f, 0.0f, 1.0f, 1.0f));

                                    if (GUILayout.Button("→", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        falloff = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
                                    }
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button("↘", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        falloff = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
                                    }
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button("↗", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        falloff = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
                                    }
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button("rnd", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        var keyframes = new Keyframe[Random.Range(4, 16)];
                                        for (int i = keyframes.Length - 1; i >= 0; i--)
                                        {
                                            float t = i == 1 ? 1.0f : i == 0 ? 0.0f : Random.value;
                                            keyframes[i] = new Keyframe(t, Random.value, Random.value, Random.value);
                                        }
                                        falloff = new AnimationCurve(keyframes);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    color = EditorGUILayout.ColorField(colorGUIContent, color, true, true, false, null);

                                    GUI.color = Color.red;
                                    if (GUILayout.Button(string.Empty, GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        color = Color.red;
                                    }
                                    GUI.color = Color.green;
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button(string.Empty, GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        color = Color.green;
                                    }
                                    GUI.color = Color.blue;
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button(string.Empty, GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        color = Color.blue;
                                    }
                                    GUI.color = Color.white;
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button(string.Empty, GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        color = Color.white;
                                    }
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button("1-x", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        var c = color;
                                        color = new Color(1.0f - c.r, 1.0f - c.g, 1.0f - c.b);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                GUILayout.Space(1.25f);

                                EditorGUILayout.BeginHorizontal();
                                {
                                    float fw = EditorGUIUtility.fieldWidth;
                                    EditorGUIUtility.fieldWidth = 65.0f;
                                    style = (BrushStyle)EditorGUILayout.EnumPopup(styleGUIContent, style);
                                    EditorGUIUtility.fieldWidth = fw;

                                    if (style != BrushStyle.None && style != BrushStyle.Circle)
                                    {
                                        GUILayout.Space(3.0f);
                                        EditorGUIUtility.labelWidth = 50.0f;
                                        alpha = EditorGUILayout.Slider(alphaGUIContent, alpha, 0.01f, 1.0f);
                                    }
                                    EditorGUIUtility.labelWidth = 70.0f;
                                }
                                EditorGUILayout.EndHorizontal();

                                GUILayout.Space(1.0f);
                            }
                        }
                        EditorGUILayout.EndVertical();

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Key bindings

                        float lw = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 150.0f;
                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                            if (GUILayout.Button("Key Bindings", VertPaintGUI.BoldLabelButtonStyle))
                            {
                                showKeyBindings = !showKeyBindings;
                            }
                            GUI.color = Color.white;

                            if (showKeyBindings)
                            {
                                paintKey = (KeyCode)EditorGUILayout.EnumPopup(paintKeyGUIContent, paintKey);
                                modifyRadiusKey = (KeyCode)EditorGUILayout.EnumPopup(modifyRadiusKeyGUIContent, modifyRadiusKey);
                                previewKey = (KeyCode)EditorGUILayout.EnumPopup(previewKeyGUIContent, previewKey);
                            }
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUIUtility.labelWidth = lw;

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Templates

                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                            if (GUILayout.Button("Templates", VertPaintGUI.BoldLabelButtonStyle))
                            {
                                showTemplates = !showTemplates;
                            }
                            GUI.color = Color.white;

                            if (showTemplates)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.BeginVertical(GUILayout.Width(80.0f));
                                    {
                                        // Draw button for saving out the current configuration to a template file.
                                        if (GUILayout.Button(saveGUIContent, GUILayout.Width(85.0f), GUILayout.Height(85.0f), GUILayout.ExpandWidth(false)))
                                        {
                                            var path = EditorUtility.SaveFilePanel("Save VertPaint brush template", "Assets/VertPaint/Templates", "<insert_template_name_here>", "xml");
                                            if (!string.IsNullOrEmpty(path))
                                            {
                                                Save(path);
                                            }
                                        }

                                        // Create drag 'n' drop field rect for loading VertPaint templates.
                                        Rect dropArea = GUILayoutUtility.GetRect(83.0f, 83.0f, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                                        dropArea.x += 5.0f; dropArea.y += 5.0f;

                                        // Clicking inside the drag 'n' drop area should show
                                        // the file picker dialog (for manual template selection).
                                        Action click = () =>
                                        {
                                            string templatePath = EditorUtility.OpenFilePanel("Select VertPaint template to load", "Assets/VertPaint/Templates", "xml");
                                            if (!string.IsNullOrEmpty(templatePath))
                                            {
                                                Load(templatePath.Substring(Application.dataPath.Length - 6));
                                            }
                                        };

                                        // Draw the drag 'n' drop field 
                                        // and get the dropped template object.
                                        var droppedObj = VertPaintGUI.DragAndDropArea(
                                            dropArea,
                                            leftClick: click,
                                            rightClick: click,
                                            validityCheck: IsXml,
                                            guiContent: templateSlotGUIContent,
                                            dragAndDropVisualMode: DragAndDropVisualMode.Link
                                        );

                                        Repaint();

                                        if (droppedObj != null)
                                        {
                                            Load(AssetDatabase.GetAssetPath(droppedObj));
                                            Repaint();
                                        }
                                    }
                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.BeginVertical();
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            EditorGUILayout.LabelField("Favorite templates");

                                            // Build the rect for a drag 'n' drop field that
                                            // is used to add VertPaint templates to the favorites list.
                                            Rect dropArea = GUILayoutUtility.GetRect(25.5f, 16.75f, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                                            dropArea.x -= 1.5f; dropArea.y += 3.5f;

                                            // Clicking instead of dragging & dropping into the plus icon  
                                            // opens the file picker dialog, with which you can manually 
                                            // select the template file to add to the favorites list.
                                            Action click = () =>
                                            {
                                                string templatePath = EditorUtility.OpenFilePanel("Select favorite VertPaint template", "Assets/VertPaint/Templates", "xml");
                                                if (!string.IsNullOrEmpty(templatePath))
                                                {
                                                    favoriteTemplates.Add(templatePath.Substring(Application.dataPath.Length - 6));
                                                    SaveFavoriteTemplates();
                                                }
                                            };

                                            // Draw the drag 'n' drop field and 
                                            // get the dropped template object.
                                            var droppedObj = VertPaintGUI.DragAndDropArea(
                                                dropArea,
                                                leftClick: click,
                                                rightClick: click,
                                                validityCheck: IsXml,
                                                guiContent: addFavButtonGUIContent,
                                                dragAndDropVisualMode: DragAndDropVisualMode.Copy,
                                                controlID: GUIUtility.GetControlID(FocusType.Passive)
                                            );

                                            Repaint();

                                            // Add the dropped template to the favorites list.
                                            if (droppedObj != null)
                                            {
                                                favoriteTemplates.Add(AssetDatabase.GetAssetPath(droppedObj));
                                                SaveFavoriteTemplates();
                                                Repaint();
                                            }

                                            GUI.enabled = favoriteTemplates.Count > 0;

                                            if (GUILayout.Button(removeFavButtonGUIContent, GUILayout.Width(25.0f)))
                                            {
                                                favoriteTemplates.RemoveAt(favoriteTemplates.Count - 1);
                                                SaveFavoriteTemplates();
                                            }

                                            if (GUILayout.Button(clearFavsButtonGUIContent, GUILayout.Width(25.0f)))
                                            {
                                                favoriteTemplates.Clear();
                                                SaveFavoriteTemplates();
                                            }

                                            GUI.enabled = true;
                                        }
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginVertical("Box", GUILayout.MaxHeight(160.0f));
                                        {
                                            favoritesListScrollPosition = GUILayout.BeginScrollView(favoritesListScrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
                                            {
                                                for (int i = 0; i < favoriteTemplates.Count; i++)
                                                {
                                                    EditorGUILayout.BeginHorizontal();
                                                    {
                                                        if (GUILayout.Button("...", GUILayout.Width(30.0f)))
                                                        {
                                                            var templatePath = EditorUtility.OpenFilePanel("Reassign favorite VertPaint template", "Assets/VertPaint/Templates", "xml");
                                                            if (!string.IsNullOrEmpty(templatePath))
                                                            {
                                                                favoriteTemplates[i] = templatePath.Substring(Application.dataPath.Length - 6);
                                                                SaveFavoriteTemplates();
                                                            }
                                                        }

                                                        if (GUILayout.Button(Path.GetFileNameWithoutExtension(favoriteTemplates[i]), GUILayout.Width(position.width - 224.0f)))
                                                        {
                                                            if (!Load(favoriteTemplates[i]))
                                                            {
                                                                favoriteTemplates.RemoveAt(i);
                                                                SaveFavoriteTemplates();
                                                            }
                                                        }

                                                        if (GUILayout.Button("-", GUILayout.Width(30.0f)))
                                                        {
                                                            favoriteTemplates.RemoveAt(i);
                                                            SaveFavoriteTemplates();
                                                        }
                                                    }
                                                    EditorGUILayout.EndHorizontal();
                                                }
                                            }
                                            EditorGUILayout.EndScrollView();
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("Autosave directory: ", GUILayout.Width(120.0f));
                                    if (GUILayout.Button(autosaveDirectory, EditorStyles.textField, GUILayout.Height(16.0f)))
                                    {
                                        var path = EditorUtility.OpenFolderPanel("Select VertPaint autosave directory", defaultAutosaveDirectory, string.Empty);
                                        if (!string.IsNullOrEmpty(path))
                                        {
                                            autosaveDirectory = path.Substring(Application.dataPath.Length - 6) + '/';
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                GUILayout.Space(1.0f);
                            }
                        }
                        EditorGUILayout.EndVertical();

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Mesh output directory

                        EditorGUILayout.BeginVertical("Box");
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(meshOutputDirectoryGUIContent, GUILayout.Width(138.0f));
                                if (GUILayout.Button(new GUIContent(meshOutputDirectory, "This is the directory where VertPaint will store the vertex colored mesh assets (after clicking on \"Apply\")."), EditorStyles.textField, GUILayout.Height(16.0f)))
                                {
                                    var path = EditorUtility.OpenFolderPanel("Select mesh output folder", "Assets/", string.Empty);
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        meshOutputDirectory = path.Substring(Application.dataPath.Length - 6) + '/';
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Quick-action buttons

                        EditorGUILayout.BeginHorizontal();
                        {
                            // Fill
                            if (GUILayout.Button(fillGUIContent, GUILayout.Height(20.0f)))
                            {
                                InitializeAdditionalVertexStreams();

                                if (selectedMeshRenderer != null)
                                {
                                    // Allow the user to undo the fill action.
                                    Undo.RecordObject(selectedMeshRenderer.additionalVertexStreams, "fill " + color.ToString());

                                    // Fill all vertex colors with the current brush colors 
                                    // or hold down shift whilst clicking on "Fill" to fill with black.
                                    var colors = selectedMeshRenderer.additionalVertexStreams.colors.Select(c => Event.current.modifiers == EventModifiers.Shift ? Color.clear : color).ToArray();
                                    selectedMeshRenderer.additionalVertexStreams.colors = colors;
                                    EditorUtility.SetDirty(selectedMeshRenderer.additionalVertexStreams);

                                    Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                                }
                            }

                            // Invert
                            if (GUILayout.Button(invertGUIContent, GUILayout.Height(20.0f)))
                            {
                                InitializeAdditionalVertexStreams();

                                if (selectedMeshRenderer != null)
                                {
                                    Undo.RecordObject(selectedMeshRenderer.additionalVertexStreams, "invert vertex colors");

                                    // Invert all vertex colors.
                                    var colors = selectedMeshRenderer.additionalVertexStreams.colors.Select(c => new Color(1.0f - c.r, 1.0f - c.g, 1.0f - c.b)).ToArray();
                                    selectedMeshRenderer.additionalVertexStreams.colors = colors;
                                    EditorUtility.SetDirty(selectedMeshRenderer.additionalVertexStreams);

                                    Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                                }
                            }

                            // Discard
                            if (GUILayout.Button(discardGUIContent, GUILayout.Height(20.0f)))
                            {
                                DiscardPaintedVertexColors();
                            }

                            // Apply
                            if (GUILayout.Button(applyGUIContent, GUILayout.Height(20.0f)))
                            {
                                ApplyPaintedVertexColors();
                            }

                            // Clean up
                            if (GUILayout.Button(cleanMeshOutputDirectoryGUIContent, GUILayout.Height(20.0f)))
                            {
                                CleanMeshOutputDirectory();
                            }

                            // Reset
                            if (GUILayout.Button(resetGUIContent, GUILayout.Height(20.0f)))
                            {
                                Reset();
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        #endregion

                        GUILayout.Space(2.0f);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(3.0f);
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (EditorApplication.isPlaying || !enabled)
                return;

            // Invoke the current brush mode.
            if (brushModeAction != null)
                brushModeAction.Invoke();

            // Hide or show the transform handle according to the setting.
            Tools.current = hideTransformHandle ? Tool.None : lastTool;

            // Keep selected no more than one object at a time.
            if (Selection.transforms.Length > 1)
            {
                Transform t = Selection.activeTransform;
                Selection.activeTransform = null;
                Selection.activeTransform = t;
            }

            // On selection changed:
            if (selectionChanged)
            {
                selectionChanged = false;

                // Remove the vertex color preview shader from the deselected mesh.
                if (selectedMeshRenderer != null && selectedMeshRenderer.sharedMaterials.Contains(previewMaterial))
                {
                    var matList = selectedMeshRenderer.sharedMaterials.ToList();
                    matList.Remove(previewMaterial);
                    selectedMeshRenderer.sharedMaterials = matList.ToArray();
                }

                if (tempCollider && selectedMeshCollider != null)
                {
                    DestroyImmediate(selectedMeshCollider);
                    selectedMeshCollider = null;
                    tempCollider = false;
                }

                DiscardPaintedVertexColors();

                GatherMeshComponents();

                // Add the vertex color preview shader to the freshly selected object (if needed).
                if (previewingVertexColors && selectedMeshRenderer != null && !selectedMeshRenderer.sharedMaterials.Contains(previewMaterial))
                {
                    var matList = selectedMeshRenderer.sharedMaterials.ToList();
                    if (!matList.Contains(previewMaterial))
                    {
                        matList.Add(previewMaterial);
                    }
                    selectedMeshRenderer.sharedMaterials = matList.ToArray();
                }

                // Collapse the MeshCollider/MeshRenderer component 
                // to hide the green wireframe overlay on the mesh.
                CollapseMeshRendererAndCollider();

                Repaint();
            }
        }

        /// <summary>
        /// Collapses the inspected <see cref="MeshRenderer"/> and <see cref="MeshCollider"/> components.
        /// </summary>
        void CollapseMeshRendererAndCollider()
        {
            if (selectedMeshCollider != null && selectedMeshRenderer != null)
            {
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(selectedMeshCollider, true);
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(selectedMeshRenderer, true);

                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(selectedMeshCollider, false);
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(selectedMeshRenderer, false);
            }
        }

        /// <summary>
        /// Assigns the selected mesh to the <see cref="selectedTransform"/> variable 
        /// and caches its relevant components, such as <see cref="selectedMeshFilter"/>, 
        /// <see cref="selectedMeshRenderer"/>, etc...
        /// </summary>
        void GatherMeshComponents()
        {
            selectedTransform = Selection.activeTransform;
            if (selectedTransform != null)
            {
                selectedMeshFilter = selectedTransform.GetComponent<MeshFilter>();
                selectedMeshRenderer = selectedTransform.GetComponent<MeshRenderer>();
                selectedMeshCollider = selectedTransform.GetComponent<MeshCollider>();
                if (selectedMeshCollider == null && selectedMeshFilter != null)
                {
                    tempCollider = true;
                    selectedMeshCollider = selectedMeshFilter.gameObject.AddComponent<MeshCollider>();
                }
                else tempCollider = false;
            }
        }

        #region Brush

        void DrawBrush(Vector3 position, Vector3 normal, Color color, BrushStyle style)
        {
            Handles.color = color;

            if (sphereBrush != null)
            {
                sphereBrush.gameObject.SetActive(style == BrushStyle.Sphere);
            }

            switch (style)
            {
                case BrushStyle.Circle:
                    Handles.DrawWireDisc(position, normal, radius);
                    HandleUtility.Repaint();
                    break;
                case BrushStyle.Disc:
                    Handles.DrawSolidDisc(position, normal, radius);
                    HandleUtility.Repaint();
                    break;
                case BrushStyle.Sphere:
                    sphereBrush.position = position;
                    sphereBrush.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
                    sphereBrushMaterial.color = color;
                    sphereBrushMaterial.SetColor("_EmissionColor", color);

                    if (SceneView.lastActiveSceneView != null)
                        SceneView.lastActiveSceneView.Repaint();

                    break;
            }
        }

        void BrushMode_Paint()
        {
            RaycastHit hit = default(RaycastHit);
            if (selectedMeshCollider != null && selectedMeshCollider.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit, 15000.0f))
            {
                if (style != BrushStyle.None)
                {
                    DrawBrush(hit.point, hit.normal, new Color(color.r, color.g, color.b, style == BrushStyle.Circle ? 1.0f : alpha), style);
                }

                if (Event.current.isKey)
                {
                    if (Event.current.keyCode == paintKey)
                    {
                        PaintVertexColors(hit);
                        Event.current.Use();
                    }
                }
                else if (Event.current.isMouse)
                {
                    if ((Event.current.button == 0 && paintKey == KeyCode.Mouse0) ||
                        (Event.current.button == 1 && paintKey == KeyCode.Mouse1) ||
                        (Event.current.button == 2 && paintKey == KeyCode.Mouse2))
                    {
                        if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                        {
                            if (Event.current.modifiers != EventModifiers.Control && Event.current.modifiers != EventModifiers.Command)
                            {
                                PaintVertexColors(hit);
                                Event.current.Use();
                            }
                        }
                    }
                }
            }

            if (Event.current.isKey)
            {
                // Handle the vertex color previewing based on the
                // specified input key and the user settings (hold or toggle).
                if (Event.current.keyCode == previewKey)
                {
                    if (!holdingPreviewKey && !togglePreview && Event.current.type == EventType.KeyDown)
                    {
                        holdingPreviewKey = true;
                        OnPreviewStateChanged(new PreviewStateChangedEventArgs(true));
                    }

                    if (Event.current.type == EventType.KeyUp)
                    {
                        holdingPreviewKey = false;
                        OnPreviewStateChanged(new PreviewStateChangedEventArgs(!previewingVertexColors));
                    }

                    Event.current.Use();
                }

                // Switch to radius sampling mode once
                // the "modify radius" key has been pressed.
                if (Event.current.keyCode == modifyRadiusKey)
                {
                    if (Event.current.type == EventType.KeyDown)
                    {
                        radiusSamplingMousePos = Event.current.mousePosition;
                        radiusSamplingRaycastHit = hit;
                        brushModeAction = BrushMode_ModifyRadius;
                    }

                    Event.current.Use();
                }
            }
            else if (Event.current.isMouse)
            {
                if ((Event.current.button == 0 && previewKey == KeyCode.Mouse0) ||
                    (Event.current.button == 1 && previewKey == KeyCode.Mouse1) ||
                    (Event.current.button == 2 && previewKey == KeyCode.Mouse2))
                {
                    if (!holdingPreviewKey && !togglePreview && Event.current.type == EventType.MouseDown)
                    {
                        holdingPreviewKey = true;
                        OnPreviewStateChanged(new PreviewStateChangedEventArgs(true));
                    }

                    if (Event.current.type == EventType.MouseUp)
                    {
                        holdingPreviewKey = false;
                        OnPreviewStateChanged(new PreviewStateChangedEventArgs(!previewingVertexColors));
                    }

                    Event.current.Use();
                }

                if ((Event.current.button == 0 && modifyRadiusKey == KeyCode.Mouse0) ||
                    (Event.current.button == 1 && modifyRadiusKey == KeyCode.Mouse1) ||
                    (Event.current.button == 2 && modifyRadiusKey == KeyCode.Mouse2))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        radiusSamplingRaycastHit = hit;
                        radiusSamplingMousePos = Event.current.mousePosition;
                        brushModeAction = BrushMode_ModifyRadius;
                    }

                    Event.current.Use();
                }
            }

            // Unless we're trying to change our selection, 
            // go for a passive focus type to avoid accidental deselection whilst painting.
            if (Event.current.modifiers != EventModifiers.Control && Event.current.modifiers != EventModifiers.Command)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
        }

        void BrushMode_ModifyRadius()
        {
            // Adjust the radius by dragging the mouse away from
            // or towards the location where we started sampling.
            radius = Vector2.Distance(radiusSamplingMousePos, Event.current.mousePosition) * .025f;

            // Make the brush flicker whilst sampling the new radius.
            float brushAlpha = Mathf.Clamp(Mathf.Sin(Time.realtimeSinceStartup * 15.0f), 0.0f, style == BrushStyle.Circle ? 1.0f : alpha);
            DrawBrush(radiusSamplingRaycastHit.point, radiusSamplingRaycastHit.normal, new Color(color.r, color.g, color.b, brushAlpha), style);

            // Repaint the VertPaint window 
            // to keep the slider synchronized.
            Repaint();

            // Exit the sampling mode if the user lets go of the key (or mouse button).
            if (Event.current.isKey && Event.current.keyCode == modifyRadiusKey)
            {
                if (Event.current.type == EventType.KeyUp)
                {
                    brushModeAction = BrushMode_Paint;
                }
                Event.current.Use();
            }
            else if (Event.current.isMouse)
            {
                if ((Event.current.button == 0 && modifyRadiusKey == KeyCode.Mouse0) ||
                    (Event.current.button == 1 && modifyRadiusKey == KeyCode.Mouse1) ||
                    (Event.current.button == 2 && modifyRadiusKey == KeyCode.Mouse2))
                {
                    if (Event.current.type == EventType.MouseUp)
                    {
                        brushModeAction = BrushMode_Paint;
                    }
                    Event.current.Use();
                }
            }
        }

        #endregion

        #region Vertex color operations

        /// <summary>
        /// Checks if the <see cref="selectedMeshRenderer"/>'s additionalVertexStreams property is null.
        /// If it is, the instantiated <see cref="selectedMeshFilter"/>'s sharedMesh is assigned to it.<para> </para>
        /// Once the additionalVertexStreams' existance can be guaranteed, its colors property is checked.
        /// If there's no vertex color data stored in it, a new <see cref="Color"/>[] array is created and assigned to it.
        /// </summary>
        void InitializeAdditionalVertexStreams()
        {
            if (selectedMeshRenderer != null && selectedMeshFilter != null)
            {
                if (selectedMeshRenderer.additionalVertexStreams == null)
                {
                    selectedMeshRenderer.additionalVertexStreams = Instantiate(selectedMeshFilter.sharedMesh);
                }

                var colors = selectedMeshRenderer.additionalVertexStreams.colors;
                if (colors == null || colors.Length == 0)
                {
                    colors = new Color[selectedMeshFilter.sharedMesh.vertices.Length];
                }

                selectedMeshRenderer.additionalVertexStreams.colors = colors;
            }
        }

        /// <summary>
        /// Paints the specified vertex color to the vertices inside the brush's area.
        /// </summary>
        /// <param name="brushHit">The brush's RaycastHit where the vertex color should be painted.</param>
        void PaintVertexColors(RaycastHit brushHit)
        {
            if (selectedMeshRenderer == null)
            {
                Debug.LogError("VertPaint: The specified MeshRenderer is null; couldn't paint vertex colors.");
                return;
            }

            // Respect the specified delay between paint strokes value.
            if (LastPaintTime.AddSeconds(delay) > DateTime.Now)
                return;

            InitializeAdditionalVertexStreams();

            // Add the paint stroke to the undo stack.
            Undo.RecordObject(selectedMeshRenderer.additionalVertexStreams, "paint vertex colors");

            var colors = selectedMeshRenderer.additionalVertexStreams.colors;
            for (int i = colors.Length - 1; i >= 0; i--)
            {
                Vector3 currentVertex = selectedMeshRenderer.additionalVertexStreams.vertices[i];

                // Calculate the distance between the center of the brush and the current vertex.
                // If it's greater than the brush radius, it means that the vert is outside of the brush area and can safely be ignored.
                float distance = Vector3.Distance(brushHit.point, brushHit.transform.TransformPoint(currentVertex));
                if (distance > radius)
                {
                    continue;
                }

                // Apply the vertex colors based on falloff and opacity.
                float t = opacity * color.a * falloff.Evaluate(distance / radius);
                colors[i] = Color.Lerp(colors[i], Event.current.modifiers == EventModifiers.Shift ? Color.clear : color, t);
            }

            selectedMeshRenderer.additionalVertexStreams.colors = colors;
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());


            // Erasing is basically like painting black color, so check against that
            // and raise the correct event based on the painted vertex color.
            if (color.r + color.g + color.b < float.Epsilon)
            {
                OnErased(new PaintStrokeEventArgs(brushHit, DateTime.Now));
            }
            else
            {
                OnPainted(new PaintStrokeEventArgs(brushHit, DateTime.Now));
            }

            LastPaintTime = DateTime.Now;
        }

        /// <summary>
        /// Discards the painted vertex colors.
        /// </summary>
        void DiscardPaintedVertexColors()
        {
            if (selectedMeshRenderer == null)
            {
                return;
            }

            selectedMeshRenderer.additionalVertexStreams = null;

            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.Repaint();
            }
        }

        /// <summary>
        /// Applies the painted vertex colors to the selected mesh by 
        /// writing them to an asset on disk (located inside the specified <see cref="meshOutputDirectory"/>).
        /// </summary>
        void ApplyPaintedVertexColors()
        {
            if (selectedMeshFilter != null && selectedMeshRenderer != null && selectedMeshRenderer.additionalVertexStreams != null)
            {
                var mesh = Instantiate(selectedMeshRenderer.additionalVertexStreams);
                mesh.name = selectedMeshRenderer.name;

                StringBuilder path = new StringBuilder(meshOutputDirectory).Append(SceneManager.GetActiveScene().name).Append('/').Append(selectedMeshRenderer.name).Append(".asset");
                string dir = Path.GetDirectoryName(path.ToString());
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                }
                AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path.ToString()));

                selectedMeshFilter.sharedMesh = mesh;
                EditorGUIUtility.PingObject(mesh);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion

        #region Event dispatchers

        protected virtual void OnPainted(PaintStrokeEventArgs eventArgs)
        {
            if (Painted != null)
            {
                Painted.Invoke(this, eventArgs);
            }
        }

        protected virtual void OnErased(PaintStrokeEventArgs eventArgs)
        {
            if (Erased != null)
            {
                Erased.Invoke(this, eventArgs);
            }
        }

        protected virtual void OnPreviewStateChanged(PreviewStateChangedEventArgs eventArgs)
        {
            if (PreviewStateChanged != null)
            {
                PreviewStateChanged.Invoke(this, eventArgs);
            }
        }

        protected virtual void OnTemplateSaved(TemplateSavedEventArgs eventArgs)
        {
            if (TemplateSaved != null)
            {
                TemplateSaved.Invoke(this, eventArgs);
            }
        }

        #endregion

        #region Default event subscribers

        void VertPaintWindow_PreviewStateChanged(object sender, PreviewStateChangedEventArgs e)
        {
            previewingVertexColors = e.Previewing;

            if (previewingVertexColors)
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            if (selectedMeshRenderer != null)
            {
                var matList = selectedMeshRenderer.sharedMaterials.ToList();
                if (previewingVertexColors && !matList.Contains(previewMaterial))
                {
                    matList.Add(previewMaterial);
                }
                else if (matList.Contains(previewMaterial))
                {
                    matList.Remove(previewMaterial);
                }
                selectedMeshRenderer.sharedMaterials = matList.ToArray();
            }
        }

        #endregion

        #region I/O

        /// <summary>
        /// Checks whether the specified object's file extension is ".xml" or not.
        /// </summary>
        /// <returns>True if the <paramref name="obj"/> is an xml file, false if otherwise.</returns>
        /// <param name="obj">The object to check.</param>
        bool IsXml(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return string.CompareOrdinal(Path.GetExtension(path), ".xml") == 0;
        }

        /// <summary>
        /// Cleans the mesh output directory from unused mesh assets.<para> </para>
        /// Meshes that are never referenced in any <see cref="MeshFilter"/> in the 
        /// scene are considered unused and are thus moved into a folder named "_old" 
        /// (since deleting them would be too intrusive).
        /// </summary>
        void CleanMeshOutputDirectory()
        {
            // Build the final output directory path based on the specified
            // mesh output directory and the currently active scene's name.
            // The result is a sub-folder of the mesh output directory named after the currently active scene.
            StringBuilder dir = new StringBuilder(meshOutputDirectory).Append(SceneManager.GetActiveScene().name).Append('/');

            // If the output directory doesn't even exist 
            // it means that there aren't any meshes whatsoever.
            if (!Directory.Exists(dir.ToString()))
            {
                return;
            }

            // Get all mesh asset paths from the output directory.
            string[] meshPaths = Directory.GetFiles(dir.ToString(), "*.asset");
            if (meshPaths == null || meshPaths.Length == 0)
            {
                return;
            }

            // Create a new dictionary to store the meshes 
            // from the output folder along with their paths.
            Dictionary<Mesh, string> meshes = new Dictionary<Mesh, string>(meshPaths.Length);

            // Populate the dictionary with valid KeyValuePairs.
            for (int i = meshPaths.Length - 1; i >= 0; i--)
            {
                var mesh = AssetDatabase.LoadAssetAtPath(meshPaths[i], typeof(Mesh)) as Mesh;
                if (mesh != null)
                {
                    meshes.Add(mesh, meshPaths[i]);
                }
            }

            // Gather all unusued mesh paths.
            // It's: all mesh paths minus the ones currently
            // in use, thanks to LINQ's Enumerable.Except method.
            IEnumerable<string> unusedMeshPaths = meshPaths.Except(FindObjectsOfType<MeshFilter>()
                .Where(meshFilter => meshes.ContainsKey(meshFilter.sharedMesh))
                .Select(meshFilter => meshes[meshFilter.sharedMesh])
                .Distinct());

            // Safety-check the target "_old" folder 
            // and move the unused mesh assets into it.
            if (unusedMeshPaths.Any())
            {
                string destinationDirectory = dir.Append("_old/").ToString();
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                }

                foreach (string path in unusedMeshPaths)
                {
                    AssetDatabase.MoveAsset(path, AssetDatabase.GenerateUniqueAssetPath(destinationDirectory + Path.GetFileName(path)));
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Saves the current VertPaint configuration out to a template file at the specified path.
        /// </summary>
        /// <param name="filePath">The full file path (including the .xml extension) to where the template file should be stored.</param>
        /// <returns>True if the operation was successful, false if the saving procedure failed in some way.</returns>
        public bool Save(string filePath)
        {
            // Null or empty file paths should be ignored (and avoided altogether).
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("VertPaint: Couldn't save template because the specified file path string is either null or empty.");
                return false;
            }

            try
            {
                // Create a new, well formatted xml document 
                // with all the VertPaint settings in it.
                using (XmlWriter writer = XmlWriter.Create(filePath, xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("vertPaintTemplate");
                    writer.WriteAttributeString("version", version.ToString());
                    {
                        writer.WriteElementString("enabled", enabled ? trueString : falseString);
                        writer.WriteElementString("togglePreview", togglePreview ? trueString : falseString);
                        writer.WriteElementString("hideTransformHandle", hideTransformHandle ? trueString : falseString);

                        writer.WriteStartElement("brushSettings");
                        {
                            writer.WriteElementString("radius", radius.ToString());
                            writer.WriteElementString("maxRadius", maxRadius.ToString());
                            writer.WriteElementString("delay", delay.ToString());
                            writer.WriteElementString("opacity", opacity.ToString());
                            writer.WriteStartElement("falloff");
                            {
                                for (int i = 0; i < falloff.keys.Length; i++)
                                {
                                    var key = falloff.keys[i];
                                    writer.WriteStartElement("key");
                                    {
                                        writer.WriteElementString("time", key.time.ToString());
                                        writer.WriteElementString("value", key.value.ToString());
                                        writer.WriteElementString("inTangent", key.inTangent.ToString());
                                        writer.WriteElementString("outTangent", key.outTangent.ToString());
                                    }
                                    writer.WriteEndElement();
                                }
                            }
                            writer.WriteEndElement();
                            writer.WriteStartElement("color");
                            {
                                writer.WriteElementString("r", color.r.ToString());
                                writer.WriteElementString("g", color.g.ToString());
                                writer.WriteElementString("b", color.b.ToString());
                                writer.WriteElementString("a", color.a.ToString());
                            }
                            writer.WriteEndElement();
                            writer.WriteElementString("style", ((int)style).ToString());
                            writer.WriteElementString("alpha", alpha.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("keyBindings");
                        {
                            writer.WriteElementString("paint", paintKey.ToString());
                            writer.WriteElementString("preview", previewKey.ToString());
                            writer.WriteElementString("modifyRadius", modifyRadiusKey.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("templates");
                        {
                            writer.WriteElementString("autosaveDirectory", autosaveDirectory);
                        }
                        writer.WriteEndElement();

                        writer.WriteElementString("meshOutputDirectory", meshOutputDirectory);

                        writer.WriteStartElement("foldouts");
                        {
                            writer.WriteElementString("help", showHelp ? trueString : falseString);
                            writer.WriteElementString("brushSettings", showBrushSettings ? trueString : falseString);
                            writer.WriteElementString("keyBindings", showKeyBindings ? trueString : falseString);
                            writer.WriteElementString("templates", showTemplates ? trueString : falseString);
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                // Raise the TemplateSaved event with all the related arguments.
                OnTemplateSaved(new TemplateSavedEventArgs(DateTime.Now, filePath, string.CompareOrdinal(filePath, AutosaveFilePath) == 0));

                // Refresh the asset database to make the new 
                // template appear in the project view instantly.
                AssetDatabase.Refresh();

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("VertPaint: Template saving procedure failed. Returning false... Error message: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Loads up a VertPaint template file (overwriting the current settings).
        /// </summary>
        /// <param name="filePath">The full file path (including the .xml extension) to the template file.</param>
        /// <returns>True if the template was loaded successfully, false if the loading procedure failed in some way.</returns>
        public bool Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                if (string.CompareOrdinal(filePath, AutosaveFilePath) != 0)
                {
                    Debug.LogError("VertPaint: The specified template file path is either null/empty or doesn't point to an existing file. Loading procedure aborted; returning false...");
                }
                return false;
            }

            using (XmlReader reader = XmlReader.Create(filePath, xmlReaderSettings))
            {
                try
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            string baseLocalName = reader.LocalName;

                            if (string.CompareOrdinal(baseLocalName, "enabled") == 0)
                                enabled = string.CompareOrdinal(reader.ReadString(), trueString) == 0;

                            if (string.CompareOrdinal(baseLocalName, "togglePreview") == 0)
                                togglePreview = string.CompareOrdinal(reader.ReadString(), trueString) == 0;

                            if (string.CompareOrdinal(baseLocalName, "hideTransformHandle") == 0)
                                hideTransformHandle = string.CompareOrdinal(reader.ReadString(), trueString) == 0;
                            
                            if (string.CompareOrdinal(baseLocalName, "brushSettings") == 0)
                            {
                                using (XmlReader brushSettingsReader = reader.ReadSubtree())
                                {
                                    // Prepare a list of keyframes for parsing the falloff curve.
                                    var falloffKeys = new List<Keyframe>(5);

                                    while (brushSettingsReader.Read())
                                    {
                                        if (brushSettingsReader.IsStartElement())
                                        {
                                            string brushSettingLocalName = brushSettingsReader.LocalName;

                                            if (string.CompareOrdinal(brushSettingLocalName, "radius") == 0)
                                                radius = float.Parse(brushSettingsReader.ReadString());

                                            if (string.CompareOrdinal(brushSettingLocalName, "maxRadius") == 0)
                                                maxRadius = float.Parse(brushSettingsReader.ReadString());

                                            if (string.CompareOrdinal(brushSettingLocalName, "delay") == 0)
                                                delay = float.Parse(brushSettingsReader.ReadString());

                                            if (string.CompareOrdinal(brushSettingLocalName, "opacity") == 0)
                                                opacity = float.Parse(brushSettingsReader.ReadString());

                                            if (string.CompareOrdinal(brushSettingLocalName, "key") == 0)
                                            {
                                                using (XmlReader falloffReader = brushSettingsReader.ReadSubtree())
                                                {
                                                    var keyframe = new Keyframe();
                                                    while (falloffReader.Read())
                                                    {
                                                        if (falloffReader.IsStartElement())
                                                        {
                                                            string keyElementLocalName = falloffReader.LocalName;

                                                            if (string.CompareOrdinal(keyElementLocalName, "time") == 0)
                                                                keyframe.time = float.Parse(falloffReader.ReadString());

                                                            if (string.CompareOrdinal(keyElementLocalName, "value") == 0)
                                                                keyframe.value = float.Parse(falloffReader.ReadString());

                                                            if (string.CompareOrdinal(keyElementLocalName, "inTangent") == 0)
                                                                keyframe.inTangent = float.Parse(falloffReader.ReadString());

                                                            if (string.CompareOrdinal(keyElementLocalName, "outTangent") == 0)
                                                                keyframe.outTangent = float.Parse(falloffReader.ReadString());
                                                        }
                                                    }
                                                    falloffKeys.Add(keyframe);
                                                }
                                            }

                                            if (string.CompareOrdinal(brushSettingLocalName, "r") == 0)
                                                color.r = float.Parse(brushSettingsReader.ReadString());

                                            if (string.CompareOrdinal(brushSettingLocalName, "g") == 0)
                                                color.g = float.Parse(brushSettingsReader.ReadString());

                                            if (string.CompareOrdinal(brushSettingLocalName, "b") == 0)
                                                color.b = float.Parse(brushSettingsReader.ReadString());

                                            if (string.CompareOrdinal(brushSettingLocalName, "a") == 0)
                                                color.a = float.Parse(brushSettingsReader.ReadString());

                                            if (string.CompareOrdinal(brushSettingLocalName, "style") == 0)
                                                style = (BrushStyle)int.Parse(brushSettingsReader.ReadString());

                                            if (string.CompareOrdinal(brushSettingLocalName, "alpha") == 0)
                                                alpha = float.Parse(brushSettingsReader.ReadString());
                                        }
                                    }

                                    // Apply the parsed falloff curve.
                                    falloff = new AnimationCurve(falloffKeys.ToArray());
                                }
                            }

                            if (string.CompareOrdinal(baseLocalName, "keyBindings") == 0)
                            {
                                using (XmlReader keyBindingsReader = reader.ReadSubtree())
                                {
                                    while (keyBindingsReader.Read())
                                    {
                                        if (keyBindingsReader.IsStartElement())
                                        {
                                            string keyBindingLocalName = keyBindingsReader.LocalName;

                                            if (string.CompareOrdinal(keyBindingLocalName, "paint") == 0)
                                                paintKey = (KeyCode)Enum.Parse(typeof(KeyCode), keyBindingsReader.ReadString());

                                            if (string.CompareOrdinal(keyBindingLocalName, "preview") == 0)
                                                previewKey = (KeyCode)Enum.Parse(typeof(KeyCode), keyBindingsReader.ReadString());

                                            if (string.CompareOrdinal(keyBindingLocalName, "modifyRadius") == 0)
                                                modifyRadiusKey = (KeyCode)Enum.Parse(typeof(KeyCode), keyBindingsReader.ReadString());
                                        }
                                    }
                                }
                            }

                            if (string.CompareOrdinal(baseLocalName, "templates") == 0)
                            {
                                using (XmlReader templatesReader = reader.ReadSubtree())
                                {
                                    while (templatesReader.Read())
                                    {
                                        if (templatesReader.IsStartElement())
                                        {
                                            string templatesSettingLocalName = templatesReader.LocalName;

                                            if (string.CompareOrdinal(templatesSettingLocalName, "autosaveDirectory") == 0)
                                                autosaveDirectory = templatesReader.ReadString();
                                        }
                                    }
                                }
                            }

                            if (string.CompareOrdinal(baseLocalName, "meshOutputDirectory") == 0)
                                meshOutputDirectory = reader.ReadString();

                            if (string.CompareOrdinal(baseLocalName, "foldouts") == 0)
                            {
                                using (XmlReader foldoutsReader = reader.ReadSubtree())
                                {
                                    while (foldoutsReader.Read())
                                    {
                                        if (foldoutsReader.IsStartElement())
                                        {
                                            string foldoutLocalName = foldoutsReader.LocalName;

                                            if (string.CompareOrdinal(foldoutLocalName, "help") == 0)
                                                showHelp = bool.Parse(foldoutsReader.ReadString());

                                            if (string.CompareOrdinal(foldoutLocalName, "brushSettings") == 0)
                                                showBrushSettings = bool.Parse(foldoutsReader.ReadString());

                                            if (string.CompareOrdinal(foldoutLocalName, "keyBindings") == 0)
                                                showKeyBindings = bool.Parse(foldoutsReader.ReadString());

                                            if (string.CompareOrdinal(foldoutLocalName, "templates") == 0)
                                                showTemplates = bool.Parse(foldoutsReader.ReadString());
                                        }
                                    }
                                }
                            }


                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError("VertPaint: Template loading procedure failed; returning false... Error message: " + e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Saves the favorite VertPaint templates list.
        /// </summary>
        /// <returns>True if the saving operation was successful; false if the procedure failed in some way.</returns>
        public bool SaveFavoriteTemplates()
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(autosaveDirectory + "VertPaint Favorite Templates.xml", xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("favoriteVertPaintTemplates");
                    writer.WriteAttributeString("version", version.ToString());
                    {
                        for (int i = 0; i < favoriteTemplates.Count; i++)
                        {
                            string path = favoriteTemplates[i];
                            writer.WriteStartElement("template");
                            {
                                writer.WriteElementString("guid", AssetDatabase.AssetPathToGUID(path));
                                writer.WriteElementString("path", path);
                            }
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                AssetDatabase.Refresh();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("VertPaint: Couldn't save favorite templates list. Error message: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Loads the favorite templates list from the last known favorites list autosave location.
        /// </summary>
        /// <returns>True if the favorite templates list was loaded successfully, false if the loading procedure failed in some way.</returns>
        public bool LoadFavoriteTemplates()
        {
            string path = autosaveDirectory + "VertPaint Favorite Templates.xml";
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("VertPaint: there was an error loading the favorite templates list. Perhaps check the involved file paths...");
                return false;
            }

            using (XmlReader reader = XmlReader.Create(path, xmlReaderSettings))
            {
                try
                {
                    favoriteTemplates.Clear();

                    while (reader.Read())
                    {
                        if (reader.IsStartElement() && string.CompareOrdinal(reader.LocalName, "template") == 0)
                        {
                            using (XmlReader templateReader = reader.ReadSubtree())
                            {
                                while (templateReader.Read())
                                {
                                    if (templateReader.IsStartElement())
                                    {
                                        string templatePath = string.Empty;

                                        if (string.CompareOrdinal(templateReader.LocalName, "path") == 0)
                                            templatePath = templateReader.ReadString();

                                        if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
                                        {
                                            favoriteTemplates.Add(templatePath);
                                            break;
                                        }

                                        if (string.CompareOrdinal(templateReader.LocalName, "guid") == 0)
                                            templatePath = AssetDatabase.GUIDToAssetPath(templateReader.ReadString());

                                        if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
                                        {
                                            favoriteTemplates.Add(templatePath);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError("VertPaint: Couldn't load the list of favorite templates; returning false... Error message: " + e.Message);
                    return false;
                }
            }
        }

        #endregion
    }
}

// Copyright (C) Raphael Beck, 2017 | www.glitchedpolygons.com