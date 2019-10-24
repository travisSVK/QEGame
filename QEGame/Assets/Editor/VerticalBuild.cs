using System.Diagnostics;
using UnityEditor;

public class VerticalBuild
{
    [MenuItem("Builds/Build and Vertical Player")]
    public static void BuildGame()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] {
            "Assets/Scenes/StartScene0.unity",
            "Assets/Scenes/FirstScene.unity",
            "Assets/Scenes/GameScene.unity",
            "Assets/Scenes/Room01b.unity",
            "Assets/Scenes/Room02b.unity",
            "Assets/Scenes/Room03b.unity",
            "Assets/Scenes/Room04b.unity",
            "Assets/Scenes/EndScene.unity",
        };

        BuildPipeline.BuildPlayer(levels, path + "/VerticalPlayer.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        Process proc = new Process();
        proc.StartInfo.FileName = path + "/VerticalPlayer.exe";
        proc.Start();
    }
}
