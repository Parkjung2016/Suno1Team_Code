using UnityEditor;
using UnityEngine;

public class SelectStartUpSceneLoader : Editor
{
    static readonly string menuPath = "Scene loader/Enable Scene Load";

    [MenuItem("Scene loader/Enable Scene Load")]
    private static void SceneLoader()
    {
        var checkPlag = GetChecked();
        Menu.SetChecked(menuPath, !checkPlag);
    }

    public static bool GetChecked() => Menu.GetChecked(menuPath);
}