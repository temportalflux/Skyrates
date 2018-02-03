using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameState))]
public class GameStateEditor : Editor
{

    private GameState instance;
    private GameState.EditorSettings editor;
    private GameState.Data data;

    void OnEnable()
    {
        this.instance = (GameState) this.target;
    }

    public override void OnInspectorGUI()
    {
        this.data = this.instance.data;
        this.editor = this.instance.editor;

        // GameState should NEVER be editable (server delivers it)

        this.editor.toggleClientsBlock = EditorGUILayout.Foldout(this.editor.toggleClientsBlock,
            "Clients: " + this.data.clients.Length);
        if (this.editor.toggleClientsBlock)
        {
            EditorGUI.indentLevel++;
            if (this.data.clients.Length == 0)
            {
                EditorGUILayout.LabelField("No clients here :(((");
            }
            else
            {
                for (int iClient = 0; iClient < this.data.clients.Length; iClient++)
                {

                    GameState.Data.Client client = this.data.clients[iClient];

                    bool toggle = this.editor.toggleClientBlocks[iClient];
                    toggle = EditorGUILayout.Foldout(toggle,
                        "ID:" + client.clientID +
                        // indicate if the local client is this block
                        (NetworkComponent.Session.ClientID == client.clientID ? " LOCAL" : "")
                    );
                    this.editor.toggleClientBlocks[iClient] = toggle;

                    if (toggle)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.LabelField("No data yet ;)");

                        EditorGUI.indentLevel--;
                    }

                }
            }
            EditorGUI.indentLevel--;
        }

        this.instance.editor = this.editor;
        EditorUtility.SetDirty(this.instance);
    }

}
