/*
 
Copyright 2020 Martin Jonasson
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections.Generic;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// Strips all shaders from the URP from build, none of these seem to be required when not using post processing.
/// Also makes any textures used by the URP post processing 32x32 to make them as small as possible.
/// This saves around 3-4 mb's for my test project. Your mileage may vary.
/// </summary>
/*
internal class URPPostProcessStripper : IPreprocessShaders, IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var packagesPath = Application.dataPath.Replace("/Assets", "") + "/Library/PackageCache/";
        Recurse(Directory.GetDirectories(packagesPath, "com.unity.render-pipelines.*"));
    }

    static void Recurse(string[] directories)
    {
        foreach (var directory in directories)
        {
            if (IsIgnoredPath(directory)) continue;

            // Debug.Log($"directory: {directory}");
            // Debug.Log($"Modifying entries in: {directory}");

            StripFiles(Directory.GetFiles(directory, "*.png.meta"));
            StripFiles(Directory.GetFiles(directory, "*.tga.meta"));

            Recurse(Directory.GetDirectories(directory));
        }
    }

    static void StripFiles(string[] files)
    {
        foreach (var filename in files)
        {
            if (IsIgnoredPath(filename)) continue;

            // Debug.Log($"Modifying entry: {filename}");
            var lines = File.ReadAllLines(filename);
            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace("maxTextureSize: 2048", "maxTextureSize: 32");
            }

            File.WriteAllLines(filename, lines);
        }
    }

    static bool IsIgnoredPath(string path)
    {
        return path.Contains("editor") || path.Contains("Editor");
    }

    public void OnProcessShader(
        Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderCompilerData)
    {
        Debug.Log(shader.name);
        if (!shader.name.Contains("Universal Render Pipeline")) return;
        shaderCompilerData.Clear();
    }
}
*/