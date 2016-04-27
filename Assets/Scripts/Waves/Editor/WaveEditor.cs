using UnityEngine;
using UnityEditor;

public class WaveEditor
{
    [MenuItem("Neovoxel/Create/Wave")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<Wave>();
    }
}