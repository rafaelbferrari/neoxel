////--------------------------------
//
// Voxels for Unity
//  Version: 1.05.2
//
// © 2014-16 by Ronny Burkersroda
//
//--------------------------------


using UnityEngine;
using UnityEditor;


namespace Voxels
{

    // Editor extension for Voxel Mesh component
    [CustomEditor(typeof(Mesh))]
    public class VoxelMeshEditor : Editor
    {

        // Show and process inspector
        public override void OnInspectorGUI()
        {
            Mesh voxelMesh = (Mesh)target;

            // Flag to enable or disable processing
            bool process = EditorGUILayout.Toggle("Enabled", voxelMesh.process);
            if (voxelMesh.process != process)
            {
                Undo.RecordObject(voxelMesh, "Processing Change");
                voxelMesh.process = process;
            }

            // Add title to bar
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.x += EditorGUIUtility.currentViewWidth * 0.5f;
            rect.y -= rect.height;
            EditorGUI.LabelField(rect, Information.Title);

            // Object selection for a mesh to use as a voxel
            UnityEngine.Mesh mesh = (UnityEngine.Mesh)EditorGUILayout.ObjectField("Voxel Mesh", voxelMesh.mesh, typeof(UnityEngine.Mesh), true);
            if (voxelMesh.mesh != mesh)
            {
                Undo.RecordObject(voxelMesh, "Voxel Mesh Change");
                voxelMesh.mesh = mesh;
            }

            // Sizing factor for the voxel mesh
            EditorGUILayout.BeginHorizontal();
            float sizeFactor = EditorGUILayout.FloatField("Size Factor", voxelMesh.sizeFactor);
            if (GUILayout.Button("Reset"))
            {
                sizeFactor = 1;
            }
            if (voxelMesh.sizeFactor != sizeFactor)
            {
                Undo.RecordObject(voxelMesh, "Size Factor Change");
                voxelMesh.sizeFactor = sizeFactor;
            }
            EditorGUILayout.EndHorizontal();

            // Flag to make new containers static
            bool staticContainers = EditorGUILayout.Toggle("Static Containers", voxelMesh.staticContainers);
            if (voxelMesh.staticContainers != staticContainers)
            {
                Undo.RecordObject(voxelMesh, "Static Containers Change");
                voxelMesh.staticContainers = staticContainers;
            }

            // Flag to merge meshes with equal materials
            EditorGUILayout.BeginHorizontal();
            bool mergeMeshes = EditorGUILayout.Toggle("Merge Meshes", voxelMesh.mergeMeshes);
            if (voxelMesh.mergeMeshes != mergeMeshes)
            {
                Undo.RecordObject(voxelMesh, "Meshes Merging Change");
                voxelMesh.mergeMeshes = mergeMeshes;
            }

            // Flag to merge only meshes with opaque materials
            EditorGUI.BeginDisabledGroup(!mergeMeshes);
            bool opaqueOnly = EditorGUILayout.ToggleLeft("Opaque Only", voxelMesh.opaqueOnly);
            if (voxelMesh.opaqueOnly != opaqueOnly)
            {
                Undo.RecordObject(voxelMesh, "Only Opaque Mesh Merging Change");
                voxelMesh.opaqueOnly = opaqueOnly;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            // Elements to fill a texture
            EditorGUILayout.BeginHorizontal();
            bool mainTextureTarget = EditorGUILayout.ToggleLeft("Main Texture", voxelMesh.mainTextureTarget, GUILayout.MaxWidth(128));
            if (voxelMesh.mainTextureTarget != mainTextureTarget)
            {
                Undo.RecordObject(voxelMesh, "Main Texture Target Flag Change");
                voxelMesh.mainTextureTarget = mainTextureTarget;
            }
            bool emissiveTextureTarget = EditorGUILayout.ToggleLeft("Emission Texture", voxelMesh.emissiveTextureTarget, GUILayout.MaxWidth(128));
            if (voxelMesh.emissiveTextureTarget != emissiveTextureTarget)
            {
                Undo.RecordObject(voxelMesh, "Emissive Texture Target Flag Change");
                voxelMesh.emissiveTextureTarget = emissiveTextureTarget;
            }

            // Flag to transfer material to vertex color
            bool vertexColors = EditorGUILayout.ToggleLeft("Vertex Colors", voxelMesh.vertexColors, GUILayout.MaxWidth(128));
            if (voxelMesh.vertexColors != vertexColors)
            {
                Undo.RecordObject(voxelMesh, "Vertex Color Flag Change");
                voxelMesh.vertexColors = vertexColors;
            }
            EditorGUILayout.EndHorizontal();

            // Name of the main target container
            string targetName = EditorGUILayout.TextField("Target Name", voxelMesh.targetName);
            if (voxelMesh.targetName != targetName)
            {
                Undo.RecordObject(voxelMesh, "Target Object Name Change");
                voxelMesh.targetName = targetName;
            }

        }

    }

}