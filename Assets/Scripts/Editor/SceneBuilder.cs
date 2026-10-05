using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AstroArc.Editor
{
    public static class SceneBuilder
    {
        public static void BuildMainScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Camera
            GameObject camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            camObj.transform.position = new Vector3(0, 0, -10);
            cam.orthographic = true;
            cam.orthographicSize = 7.0f;
            cam.backgroundColor = new Color(0.01f, 0.03f, 0.06f, 1.0f);
            cam.clearFlags = CameraClearFlags.SolidColor;

            // 2. Light
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = Color.white;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

            // 3. Game Director
            GameObject directorObj = new GameObject("DefenseDirector");
            directorObj.AddComponent<DefenseDirector>();

            // Save Scene
            string sceneDir = "Assets/Scenes";
            if (!Directory.Exists(sceneDir))
            {
                Directory.CreateDirectory(sceneDir);
            }
            string scenePath = $"{sceneDir}/MainDefense.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[SCENE BUILDER] Main scene generated and saved at {scenePath}");

            // Configure Build Settings
            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
            EditorBuildSettings.scenes = scenes;
        }

        public static void BuildStandaloneWindows()
        {
            BuildMainScene();
            string buildDir = "Builds/Windows";
            if (!Directory.Exists(buildDir))
            {
                Directory.CreateDirectory(buildDir);
            }

            BuildPlayerOptions opts = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/MainDefense.unity" },
                locationPathName = $"{buildDir}/AstroArc_Defense.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(opts);
            Debug.Log($"[UNITY BUILD REPORT] Result: {report.summary.result} - Size: {report.summary.totalSize} bytes");
        }
    }
}
