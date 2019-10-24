﻿using System.Diagnostics;
using UnityEditor;

public class SpectatorBuild
{
    [MenuItem("Builds/Build and Run Spectator")]
    public static void BuildGame()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] {
            "Assets/Scenes/Spectator.unity",
            "Assets/Scenes/Room01a.unity",
            "Assets/Scenes/Room01b.unity",
            "Assets/Scenes/Room02a.unity",
            "Assets/Scenes/Room02b.unity",
            "Assets/Scenes/Room03a.unity",
            "Assets/Scenes/Room03b.unity",
            "Assets/Scenes/Room04a.unity",
            "Assets/Scenes/Room04b.unity"
        };

        BuildPipeline.BuildPlayer(levels, path + "/Spectator.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        Process proc = new Process();
        proc.StartInfo.FileName = path + "/Spectator.exe";
        proc.Start();
    }
}
