using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildWebGL
{
    [MenuItem("Build/WebGL")]
    public static void Build()
    {
        // Disabled compression: most static hosts transparently gzip/brotli-compress
        // every response themselves, so Unity-side compressed files (.gz/.br) get
        // double handled and the browser connection dies ("connection was closed").
        // Disabled keeps the files raw and lets the host compress on the wire.
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

        string outputPath = Environment.GetEnvironmentVariable("WEBGL_OUT_PATH");
        if (string.IsNullOrEmpty(outputPath))
            outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TileFall_WebGL_New");

        var options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Start.unity", "Assets/Scenes/MainGame.unity" },
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildResult result = report.summary.result;
        Debug.Log($"[BuildWebGL] Result: {result} | Output: {outputPath} | Total time: {report.summary.totalTime}");

        if (result != BuildResult.Succeeded)
            EditorApplication.Exit(1);
        else
            EditorApplication.Exit(0);
    }
}

public class WebGLBuildPostProcess : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
            return;

        string indexPath = Path.Combine(report.summary.outputPath, "index.html");
        if (!File.Exists(indexPath))
            return;

        string html = File.ReadAllText(indexPath);

        // Cap render resolution at 1x device pixel ratio (avoids rendering 4x pixels
        // on hiDPI laptops, which is very expensive with SSAO/post-processing).
        html = html.Replace("// config.devicePixelRatio = 1;", "config.devicePixelRatio = 1;");

        File.WriteAllText(indexPath, html);
        Debug.Log("[WebGLBuildPostProcess] index.html patched: devicePixelRatio = 1");
    }
}
