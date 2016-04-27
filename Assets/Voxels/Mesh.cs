//--------------------------------
//
// Voxels for Unity
//  Version: 1.05.2
//
// © 2014-16 by Ronny Burkersroda
//
//--------------------------------


using UnityEngine;
using System.Collections.Generic;


namespace Voxels
{

    // Class to convert incoming voxel data to a list of meshes
    public class Mesh : Processor
    {
        // Mesh for one voxel and sizing factor
        public UnityEngine.Mesh mesh;
        public float sizeFactor = 1;
        // Flag to transfer material to vertex colors
        public bool vertexColors = false;
        // Flag to create texture and use it as main one in the materials
        public bool mainTextureTarget = false;
        // Flag to create texture and use it as emission one in the materials
        public bool emissiveTextureTarget = false;
        // Flag to create static game objects
        public bool staticContainers = true;
        // Flag to merge containers with comparable material to one mesh
        public bool mergeMeshes = true;
        // Flag to merge only opaque containers
        public bool opaqueOnly = true;
        // Material to use in combination with voxel texture
        public Material textureMaterialTemplate = null;
        // Name of the target container to create
        public string targetName = "Voxel Mesh";

        // Combination of containers for given materials
        Dictionary<Material, GameObject> groups;
        IEnumerator<KeyValuePair<Material, GameObject>> currentGroup;

        // Array of mesh filters and processing flags
        MeshFilter[] meshFilters;
        bool[] processedMeshes;
        // Container for current material
        GameObject materialContainer;
        // Array of vertex colors
        Color[] colors;
        Color lastColor;

        // Target container
        GameObject mainContainer;

        // Transformation vectors
        Vector3 offset;
        Vector3 scaling;
        Vector3 globalScaling;

        // Current position
        int groupNumber = 0;

        // Mesh counters
        int meshCount = 0;
        int currentMesh;

        // Voxel texture input / processor
        public Texture2D.Process voxelTexture2D;
        Vector2[] textureCoordinates;

        // Voxels iterator
        Storage.Iterator iterator;

        // Build voxel object
        public override float Build(Storage voxels, Bounds bounds)
        {
            // Check for given array
            if (voxels != null)
            {
                GameObject subContainer;
                int width = voxels.Width;
                int height = voxels.Height;
                int depth = voxels.Depth;
                int sides = voxels.FacesCount;
                int x, y, z;

                // Check for non-empty array
                if (width * height * depth * sides > 0)
                {
                    if (mainContainer == null)
                    {
                        // Check if texture is required
                        if (mainTextureTarget || emissiveTextureTarget)
                        {
                            // Create voxel texture, if required
                            if (voxelTexture2D == null)
                            {
                                voxelTexture2D = new Texture2D.Process();
                            }

                            // Build texture
                            if (voxelTexture2D != null && voxelTexture2D.CurrentProgress < 1)
                            {
                                return voxelTexture2D.Build(voxels, bounds) * 0.5f;
                            }
                        }
                        else
                        {
                            voxelTexture2D = null;
                        }

                        // Get iterator
                        iterator = voxels.GetIterator();

                        // Create empty game object
                        mainContainer = new GameObject(targetName);
                        if (mainContainer != null)
                        {
                            // Hide new container
                            mainContainer.hideFlags |= HideFlags.HideAndDontSave;

                            // Create empty list to store groups to
                            groups = new Dictionary<Material, GameObject>();

                            // Copy position from source object
                            mainContainer.transform.position = gameObject.transform.position;

                            // Copy static flag
                            mainContainer.isStatic = staticContainers;

                            // Calculate total scaling for one block
                            globalScaling = new Vector3(2.0f * bounds.extents.x / (float)width, 2.0f * bounds.extents.y / (float)height, 2.0f * bounds.extents.z / (float)depth);

                            // Check for given mesh
                            if (mesh != null)
                            {
                                // Calculate offset and scaling for one voxel mesh
                                offset = -mesh.bounds.center;
                                scaling.x = 0.5f / mesh.bounds.extents.x;
                                scaling.y = 0.5f / mesh.bounds.extents.y;
                                scaling.z = 0.5f / mesh.bounds.extents.z;
                                offset.x *= scaling.x;
                                offset.y *= scaling.y;
                                offset.z *= scaling.z;
                                scaling.x *= globalScaling.x;
                                scaling.y *= globalScaling.y;
                                scaling.z *= globalScaling.z;
                            }
                            else
                            {
                                // Unset translation und scaling
                                offset = Vector3.zero;
                                scaling = Vector3.one;
                            }

                            // Add offset for half voxel
                            offset += new Vector3(0.5f * globalScaling.x, 0.5f * globalScaling.y, 0.5f * globalScaling.z);

                            // Move to match position of the original object
                            offset += bounds.center - gameObject.transform.position - bounds.extents;
                        }
                    }

                    // Check for main container and voxel iterator
                    if (mainContainer != null && iterator != null)
                    {
                        // Process voxels in steps
                        for (int number = 0; number < 10; ++number)
                        {
                            // Retrieve material for current coordinate
                            int iteratorIndex = iterator.Number;
                            Color color;
                            Material material = iterator.GetNextMaterial(out color, out x, out y, out z);

                            // Check for valid voxel
                            if (material != null)
                            {
                                // Check for existing material groups
                                if (groups != null)
                                {
                                    // Check for new group
                                    if (!groups.TryGetValue(material, out subContainer) || (subContainer == null))
                                    {
                                        // Create empty game object
                                        subContainer = new GameObject(material.name == null || material.name.Length == 0 ? (groups.Count + 1).ToString() : material.name);
                                        if (subContainer != null)
                                        {
                                            // Attach it to this main object
                                            subContainer.transform.parent = mainContainer.transform;

                                            // Unset local transformation
                                            subContainer.transform.localPosition = Vector3.zero;
                                            subContainer.transform.localScale = Vector3.one;
                                            subContainer.transform.localRotation = Quaternion.identity;

                                            // Copy static flag
                                            subContainer.isStatic = staticContainers;

                                            // Set layer number by transparency property
                                            if (material.HasProperty("_Color"))
                                            {
                                                if (material.color.a < 1)
                                                {
                                                    subContainer.layer = 1;
                                                }
                                                else
                                                {
                                                    subContainer.layer = 0;
                                                }
                                            }

                                            try
                                            {
                                                // Add group to list
                                                groups.Add(material, subContainer);
                                            }
                                            catch(System.Exception exception)
                                            {
                                                Debug.Log(exception.Message);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Unset container for first material
                                    subContainer = null;
                                }

                                // Calculate current voxel position
                                Vector3 currentPosition = new Vector3((float)x * globalScaling.x + offset.x, (float)y * globalScaling.y + offset.y, (float)z * globalScaling.z + offset.z);

                                // material container as parent for the current voxel
                                GameObject parent = subContainer;

                                // Create empty game object
                                subContainer = new GameObject(x.ToString() + ", " + y.ToString() + ", " + z.ToString());
                                if (subContainer != null)
                                {
                                    if (parent != null)
                                    {
                                        // Attach it to material container
                                        subContainer.transform.parent = parent.transform;
                                    }
                                    else
                                    {
                                        // Attach it to main object
                                        subContainer.transform.parent = mainContainer.transform;
                                    }

                                    // Set transformation to position and scale current cell
                                    subContainer.transform.localPosition = currentPosition;
                                    subContainer.transform.localScale = scaling * sizeFactor;
                                    subContainer.transform.localRotation = Quaternion.identity;

                                    // Copy static flag
                                    subContainer.isStatic = staticContainers;

                                    // Set layer number by transparency property
                                    if (material.HasProperty("_Color"))
                                    {
                                        if (material.color.a < 1)
                                        {
                                            subContainer.layer = 1;
                                        }
                                        else
                                        {
                                            subContainer.layer = 0;
                                        }
                                    }

                                    // Check for valid mesh
                                    if (mesh != null)
                                    {
                                        // Add mesh filter
                                        MeshFilter meshFilter = subContainer.AddComponent<MeshFilter>();
                                        if (meshFilter != null)
                                        {
                                            // Apply specified mesh as shared one for the sub container
                                            meshFilter.sharedMesh = mesh;

                                            // Check for vertex color utilization
                                            if (vertexColors)
                                            {
                                                // Create new array, if size does not match
                                                if (colors == null || colors.Length != mesh.vertexCount)
                                                {
                                                    colors = new Color[mesh.vertexCount];
                                                    lastColor = colors[0];
                                                }

                                                // Fill and apply new vertex colors array
                                                if (colors != null)
                                                {
                                                    if (lastColor != color)
                                                    {
                                                        for (int index = 0; index < colors.Length; ++index)
                                                        {
                                                            colors[index] = color;
                                                        }
                                                        lastColor = color;
                                                    }

                                                    meshFilter.mesh.colors = colors;
                                                }
                                            }

                                            // Check for existing voxel map
                                            if (voxelTexture2D != null)
                                            {
                                                // Retrieve texture coordinate for current voxel
                                                Vector2 textureCoordinate = voxelTexture2D.GetTextureCoordinate(voxels, iteratorIndex);
                                                if (!float.IsNaN(textureCoordinate.x))
                                                {
                                                    // Create new array, if size does not match
                                                    if (textureCoordinates == null || textureCoordinates.Length != mesh.vertexCount)
                                                    {
                                                        textureCoordinates = new Vector2[mesh.vertexCount];
                                                    }

                                                    // Fill and apply new UV coordinates array
                                                    if (textureCoordinates != null)
                                                    {
                                                        for (int index = 0; index < textureCoordinates.Length; ++index)
                                                        {
                                                            textureCoordinates[index] = textureCoordinate;
                                                        }

                                                        meshFilter.mesh.uv = textureCoordinates;
                                                    }

                                                    //Debug.Log(textureCoordinate);

                                                    // Apply texture to material
                                                    if (mainTextureTarget)
                                                    {
                                                        material.SetTexture("_MainTex", voxelTexture2D.Texture);
                                                    }
                                                    if (emissiveTextureTarget)
                                                    {
                                                        material.SetTexture("_EmissionMap", voxelTexture2D.Texture);
                                                    }
                                                }
                                            }

                                            // Add mesh renderer
                                            MeshRenderer meshRenderer = subContainer.AddComponent<MeshRenderer>();
                                            if (meshRenderer != null)
                                            {
                                                // Hide object
                                                meshRenderer.enabled = false;

                                                // Set material to renderer
                                                if (material != null)
                                                {
                                                    meshRenderer.material = material;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                iterator = null;
                                break;
                            }
                        }

                        // Return current progress when building has not been finished
                        if (iterator != null)
                        {
                            return ((float)iterator.Number / (float)(voxels.Count + 1) * (mergeMeshes ? 0.5f : 1.0f)) * ((voxelTexture2D != null) ? 0.5f : 1.0f) + ((voxelTexture2D != null) ? 0.5f : 0.0f);
                        }
                    }
                }
            }

            // Check for groups of materials
            if (groups != null && groups.Count > 0)
            {
                GameObject meshContainer;

                // Initialize group enumerator
                if (currentGroup == null)
                {
                    currentGroup = groups.GetEnumerator();
                    if (currentGroup != null)
                    {
                        if (!currentGroup.MoveNext())
                        {
                            currentGroup = null;
                        }
                    }
                }

                // Check for mesh baking
                if (mergeMeshes)
                {
                    int count, index;
                    int vertexCount, indexCount;

                    // Process collected material groups
                    if (currentGroup != null)
                    {
                        // Check if semi-transparent meshes should be merged or if current group is opaque
                        if (!opaqueOnly || (currentGroup.Current.Key.color.a <= 0 || currentGroup.Current.Key.color.a >= 1))
                        {
                            if (materialContainer == null)
                            {
                                // Create empty game object for the material
                                materialContainer = new GameObject(currentGroup.Current.Value.name);
                                if (materialContainer != null)
                                {
                                    // Attach it to this main object
                                    materialContainer.transform.parent = mainContainer.transform;

                                    // Unset relative transformation
                                    materialContainer.transform.localPosition = Vector3.zero;
                                    materialContainer.transform.localScale = Vector3.one;
                                    materialContainer.transform.localRotation = Quaternion.identity;

                                    // Copy static flag
                                    materialContainer.isStatic = staticContainers;

                                    // Get meshes of the current group and create array to store flags of processed meshes 
                                    meshFilters = currentGroup.Current.Value.GetComponentsInChildren<MeshFilter>();
                                    processedMeshes = new bool[meshFilters.Length];

                                    // Initialize processing flags and count meshes to merge
                                    for (meshCount = 0, index = 0; index < meshFilters.Length; ++index)
                                    {
                                        if (meshFilters[index].gameObject != currentGroup.Current.Value)
                                        {
                                            processedMeshes[index] = false;

                                            ++meshCount;
                                        }
                                        else
                                        {
                                            processedMeshes[index] = true;
                                        }
                                    }

                                    if (meshCount == 0)
                                    {
                                        materialContainer = null;
                                    }

                                    currentMesh = 0;
                                }
                            }
                        }
                        else
                        {
                            materialContainer = null;
                        }

                        if (materialContainer != null)
                        {
                            // Count number of meshes and total vertices and indices count for current target
                            for (vertexCount = 0, indexCount = 0, count = 0, index = 0; index < meshFilters.Length; ++index)
                            {
                                // Check if mesh has not already been processed
                                if (!processedMeshes[index])
                                {
                                    // Check for vertex and index count limit
                                    if (vertexCount + meshFilters[index].sharedMesh.vertexCount < 65536 && indexCount + meshFilters[index].sharedMesh.triangles.Length < 65536)
                                    {
                                        vertexCount += meshFilters[index].sharedMesh.vertexCount;
                                        indexCount += meshFilters[index].sharedMesh.triangles.Length;

                                        ++count;
                                    }
                                }
                            }

                            // Create array to store meshes to merge to
                            CombineInstance[] subMeshes = new CombineInstance[count];

                            // Process meshes of the current group
                            for (vertexCount = 0, indexCount = 0, count = 0, index = 0; index < meshFilters.Length; ++index)
                            {
                                // Check if mesh is not already processed
                                if (!processedMeshes[index])
                                {
                                    // Check for vertex and index limit
                                    if (vertexCount + meshFilters[index].sharedMesh.vertexCount < 65536 && indexCount + meshFilters[index].sharedMesh.triangles.Length < 65536)
                                    {
                                        // Increase vertices and indices counts for current target mesh
                                        vertexCount += meshFilters[index].sharedMesh.vertexCount;
                                        indexCount += meshFilters[index].sharedMesh.triangles.Length;

                                        // Store mesh instance and calculate transformation relative to parent object
                                        subMeshes[count].mesh = meshFilters[index].sharedMesh;
                                        subMeshes[count].transform = currentGroup.Current.Value.transform.worldToLocalMatrix * meshFilters[index].transform.localToWorldMatrix;

                                        // Set flag to skip mesh at next iteration
                                        processedMeshes[index] = true;

                                        // Increase sub meshes count for current merge target
                                        ++count;
                                    }
                                }
                            }

                            // Create object for current mesh to merge
                            meshContainer = new GameObject("Part");
                            if (meshContainer != null)
                            {
                                // Attach it to this material object
                                meshContainer.transform.parent = materialContainer.transform;

                                // Unset relative transformation
                                meshContainer.transform.localPosition = Vector3.zero;
                                meshContainer.transform.localScale = Vector3.one;
                                meshContainer.transform.localRotation = Quaternion.identity;

                                // Copy static flag
                                meshContainer.isStatic = staticContainers;

                                // Add mesh filter
                                MeshFilter meshFilter = meshContainer.GetComponent<MeshFilter>();
                                if (meshFilter == null)
                                {
                                    meshFilter = meshContainer.AddComponent<MeshFilter>();
                                }

                                if (meshFilter != null)
                                {
                                    // Create empty mesh object
                                    UnityEngine.Mesh mesh = new UnityEngine.Mesh();
                                    if (mesh != null)
                                    {
                                        // Merge all collected meshes into new one
                                        mesh.CombineMeshes(subMeshes, true, true);

                                        // Set mesh to filter
                                        meshFilter.mesh = mesh;

                                        // Add mesh renderer
                                        MeshRenderer meshRenderer = meshContainer.GetComponent<MeshRenderer>();
                                        if (meshRenderer == null)
                                        {
                                            meshRenderer = meshContainer.AddComponent<MeshRenderer>();
                                        }

                                        // Set material
                                        if (meshRenderer != null)
                                        {
                                            meshRenderer.material = currentGroup.Current.Key;
                                            //meshRenderer.material = groups[group].material;

                                            meshRenderer.enabled = false;
                                        }
                                    }
                                }
                            }

                            // Decrease number of remaining objects
                            currentMesh += count;
                            if (currentMesh >= meshCount)
                            {
                                // Unset objects for current group
                                materialContainer = null;

                                // Remove original game object
                                DestroyImmediate(currentGroup.Current.Value);
                                //DestroyImmediate(groups[group].gameObject);
                            }
                        }
                    }

                    // Increase number of the current group, if it has been finished
                    if (materialContainer == null)
                    {
                        if (currentGroup.MoveNext())
                        {
                            ++groupNumber;
                        }
                        else
                        {
                            currentGroup = null;
                        }
                    }

                    // Return current progress when building has not been finished
                    if (currentGroup != null)
                    {
                        return (((float)groupNumber + ((float)currentMesh / (float)(meshCount + 1))) / (float)groups.Count * 0.5f + 0.5f) * ((voxelTexture2D != null) ? 0.5f : 1.0f) + ((voxelTexture2D != null) ? 0.5f : 0.0f);
                    }
                }

                // Clear groups list
                groups.Clear();
                groups = null;
            }

            // Reset current processing data
            currentGroup = null;
            groupNumber = 0;
            meshFilters = null;
            processedMeshes = null;
            colors = null;
            voxelTexture2D = null;

            if (mainContainer != null)
            {
                // Show new main container and enable its renderers
                mainContainer.hideFlags &= ~HideFlags.HideAndDontSave;
                ShowRenderer(mainContainer);

                //StaticBatchingUtility.Combine(mainContainer);

#if UNITY_EDITOR

                // Add object creation undo operation
                if (!Application.isPlaying)
                {
                    UnityEditor.Undo.RegisterCreatedObjectUndo(mainContainer, "\"" + targetName + "\" Creation");
                }

#endif

                mainContainer = null;
            }

            return 1;
        }

        // Enable renderer component of the given object and its children
        void ShowRenderer(GameObject container, bool enabled = true)
        {
            int child;

            if (container.GetComponent<Renderer>() != null)
            {
                container.GetComponent<Renderer>().enabled = enabled;
            }

            for (child = 0; child < container.transform.childCount; ++child)
            {
                ShowRenderer(container.transform.GetChild(child).gameObject, enabled);
            }
        }

    }

}