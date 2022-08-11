using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveFunctionCollabseMaster))]
public class InspectorWave : Editor
{
    public override void OnInspectorGUI()
    {
        WaveFunctionCollabseMaster waveMap = (WaveFunctionCollabseMaster)target;

        if( DrawDefaultInspector())
        {
            if(waveMap.autoUpdate)
            {
                waveMap.GenerateMap();
            }
        }
        if(GUILayout.Button("Generate"))
        {
            waveMap.GenerateMap();
        }
    }
}
