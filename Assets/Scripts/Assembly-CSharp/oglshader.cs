using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class oglshader : MonoBehaviour
{
    private static Dictionary<string, Shader> shaderCache = new Dictionary<string, Shader>();

    void Start()
    {
        CacheAllShaders();
    }

    void CacheAllShaders()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.sharedMaterials)
            {
                if (mat != null && mat.shader != null)
                {
                    string shaderName = mat.shader.name;

                    if (!shaderCache.ContainsKey(shaderName))
                    {
                        Shader translatedShader = TranslateShader(mat.shader);
                        shaderCache[shaderName] = translatedShader;
                        mat.shader = translatedShader;
                        Debug.Log("Cached & Translated Shader: " + shaderName);
                    }
                }
            }
        }
    }

    Shader TranslateShader(Shader originalShader)
    {
        string shaderName = originalShader.name;

        // If already translated, return cached version
        if (shaderCache.ContainsKey(shaderName))
            return shaderCache[shaderName];

        // Hypothetically read shader source (Unity does not expose it directly)
        string shaderCode = LoadShaderText(shaderName);
        if (string.IsNullOrEmpty(shaderCode)) return originalShader;

        // Convert HLSL (DirectX) to GLSL (OpenGL)
        shaderCode = shaderCode.Replace("SV_POSITION", "gl_Position")
                               .Replace("mul(", "matrixCompMult(")
                               .Replace("float4", "vec4")
                               .Replace("sampler2D", "uniform sampler2D");

        // Create and return a new shader (Unity does not support runtime shader creation)
        Shader newShader = new Material(originalShader).shader;
        shaderCache[shaderName] = newShader;
        return newShader;
    }

    string LoadShaderText(string shaderName)
    {
        // Try to find the shader in the project files (Assets/Shaders/)
        string shaderPath = Path.Combine(Application.dataPath, "Shaders");
        shaderPath = Path.Combine(shaderPath, shaderName + ".shader");

        if (File.Exists(shaderPath))
        {
            return File.ReadAllText(shaderPath);
        }
        else
        {
            Debug.LogWarning("Shader file not found: " + shaderPath);
            return null;
        }
    }
}
