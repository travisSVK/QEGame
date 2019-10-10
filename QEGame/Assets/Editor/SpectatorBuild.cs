using System.Diagnostics;
using UnityEditor;

public class SpectatorBuild
{
    [MenuItem("Builds/Build and Run Spectator")]
    public static void BuildGame()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] { "Assets/Spectator.unity" };

        BuildPipeline.BuildPlayer(levels, path + "/Spectator.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        Process proc = new Process();
        proc.StartInfo.FileName = path + "/Spectator.exe";
        proc.Start();
    }
}
