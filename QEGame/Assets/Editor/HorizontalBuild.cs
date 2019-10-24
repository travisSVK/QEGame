using System.Diagnostics;
using UnityEditor;

public class HorizontalBuild
{
    [MenuItem("Builds/Build and Horizontal Player")]
    public static void BuildGame()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] {
            "Assets/Scenes/StartScene0.unity",
            "Assets/Scenes/FirstScene.unity",
            "Assets/Scenes/GameScene.unity",
            "Assets/Scenes/Room01a.unity",
            "Assets/Scenes/Room02a.unity",
            "Assets/Scenes/Room03a.unity",
            "Assets/Scenes/Room04a.unity",
            "Assets/Scenes/EndScene.unity",
        };

        BuildPipeline.BuildPlayer(levels, path + "/HorizontalPlayer.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        Process proc = new Process();
        proc.StartInfo.FileName = path + "/HorizontalPlayer.exe";
        proc.Start();
    }
}
