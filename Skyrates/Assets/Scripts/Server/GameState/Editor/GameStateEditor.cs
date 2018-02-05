using System;
using System.Collections;
using System.Collections.Generic;
using Skyrates.Common.Network;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameState))]
public class GameStateEditor : Editor
{

    private GameState instance;
    private GameStateData _gameStateData;

    // The entire clients block
    private static bool toggleBlockClients = true;
    // each client's block
    private static bool[] toggleClientBlocks = new bool[0];

    void OnEnable()
    {
        this.instance = (GameState) this.target;
    }

    public override void OnInspectorGUI()
    {
        this._gameStateData = this.instance.data;

        // GameState should NEVER be editable (server delivers it)

        toggleBlockClients = EditorGUILayout.Foldout(toggleBlockClients,
            "Clients: " + this._gameStateData.clients.Count);
        if (toggleBlockClients)
        {
            EditorGUI.indentLevel++;
            if (this._gameStateData.clients.Count == 0)
            {
                EditorGUILayout.LabelField("No clients here :(((");
            }
            else
            {
                if (toggleClientBlocks.Length != this._gameStateData.clients.Count)
                    Array.Resize(ref toggleClientBlocks, this._gameStateData.clients.Count);

                for (int iClient = 0; iClient < this._gameStateData.clients.Count; iClient++)
                {

                    GameStateData.Client client = this._gameStateData.clients[iClient];

                    bool toggle = toggleClientBlocks[iClient];
                    toggle = EditorGUILayout.Foldout(toggle,
                        "ID:" + client.clientID +
                        // indicate if the local client is this block
                        (NetworkComponent.GetSession.ClientID == client.clientID ? " LOCAL" : "")
                    );
                    toggleClientBlocks[iClient] = toggle;

                    if (toggle)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.LabelField("No _gameStateData yet ;)");

                        EditorGUI.indentLevel--;
                    }

                }
            }
            EditorGUI.indentLevel--;
        }

        EditorUtility.SetDirty(this.instance);
    }

}
