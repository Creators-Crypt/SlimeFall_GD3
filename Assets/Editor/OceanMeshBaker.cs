using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Replaces a monstrous ProBuilder ocean plane with a lightweight, standalone mesh asset.
///
/// Usage: select the Ocean GameObject (in the scene OR in Prefab Mode), then
/// Tools > Ocean > Bake Ocean Mesh (Replace ProBuilder)
/// </summary>
public static class OceanMeshBaker
{
    // Grid resolution (quads per side). 128 -> 16,641 verts. 64 -> 4,225. 256 -> 66k (don't).
    const int   Resolution = 128;
    const string OutputPath = "Assets/_Underwater_Level/Ocean_Plane.asset";

    [MenuItem("Tools/Ocean/Bake Ocean Mesh (Replace ProBuilder)")]
    static void Bake()
    {
        var go = Selection.activeGameObject;
        if (go == null) { EditorUtility.DisplayDialog("Ocean Baker", "Select the Ocean GameObject first.", "OK"); return; }

        // ---- Work out the plane extents from the existing mesh so it stays the same size ----
        var mf = go.GetComponent<MeshFilter>();
        float sizeX = 1436f, sizeZ = 1446f; // fallback: matches the current ocean
        if (mf != null && mf.sharedMesh != null)
        {
            var b = mf.sharedMesh.bounds;
            sizeX = b.size.x; sizeZ = b.size.z;
        }

        // ---- Build a flat XZ grid ----
        var mesh = BuildPlane(sizeX, sizeZ, Resolution);
        mesh.name = "Ocean_Plane";

        var existing = AssetDatabase.LoadAssetAtPath<Mesh>(OutputPath);
        if (existing != null)
        {
            EditorUtility.CopySerialized(mesh, existing);
            mesh = existing;
        }
        else
        {
            AssetDatabase.CreateAsset(mesh, OutputPath);
        }
        AssetDatabase.SaveAssets();

        // ---- Strip ProBuilder + junk physics, assign the new mesh ----
        Undo.RegisterFullObjectHierarchyUndo(go, "Bake Ocean Mesh");

        foreach (var c in go.GetComponents<Component>())
        {
            if (c == null) continue;
            var t = c.GetType().FullName;
            if (t == "UnityEngine.ProBuilder.ProBuilderMesh" ||
                t == "UnityEngine.ProBuilder.PolyShape" ||
                t == "UnityEngine.ProBuilder.BezierShape" ||
                t == "UnityEngine.ProBuilder.Shapes.ProBuilderShape")
                Undo.DestroyObjectImmediate(c);
        }
        // Rigidbody must go before the collider it depends on? No—other way round; collider first is safe.
        var col = go.GetComponent<MeshCollider>(); if (col) Undo.DestroyObjectImmediate(col);
        var rb  = go.GetComponent<Rigidbody>();    if (rb)  Undo.DestroyObjectImmediate(rb);

        if (mf == null) mf = Undo.AddComponent<MeshFilter>(go);
        mf.sharedMesh = mesh;

        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.shadowCastingMode = ShadowCastingMode.Off; // water doesn't need to cast shadows
        }

        EditorUtility.SetDirty(go);
        if (PrefabUtility.IsPartOfPrefabInstance(go))
            PrefabUtility.RecordPrefabInstancePropertyModifications(mf);

        Debug.Log($"[Ocean Baker] Baked {mesh.vertexCount:N0} verts / {mesh.triangles.Length / 3:N0} tris " +
                  $"({sizeX:F0} x {sizeZ:F0}) -> {OutputPath}. ProBuilder/collider/rigidbody removed from '{go.name}'.");
    }

    static Mesh BuildPlane(float sizeX, float sizeZ, int res)
    {
        int vpr = res + 1;
        var verts = new Vector3[vpr * vpr];
        var uvs   = new Vector2[vpr * vpr];
        var norms = new Vector3[vpr * vpr];
        var tans  = new Vector4[vpr * vpr];
        var tris  = new int[res * res * 6];

        for (int z = 0, i = 0; z < vpr; z++)
        for (int x = 0; x < vpr; x++, i++)
        {
            float u = (float)x / res, v = (float)z / res;
            verts[i] = new Vector3((u - 0.5f) * sizeX, 0f, (v - 0.5f) * sizeZ);
            uvs[i]   = new Vector2(u, v);
            norms[i] = Vector3.up;
            tans[i]  = new Vector4(1, 0, 0, -1);
        }

        for (int z = 0, t = 0; z < res; z++)
        for (int x = 0; x < res; x++)
        {
            int i = z * vpr + x;
            tris[t++] = i;       tris[t++] = i + vpr;     tris[t++] = i + 1;
            tris[t++] = i + 1;   tris[t++] = i + vpr;     tris[t++] = i + vpr + 1;
        }

        var m = new Mesh { indexFormat = IndexFormat.UInt32 };
        m.SetVertices(verts);
        m.SetUVs(0, uvs);
        m.SetNormals(norms);
        m.SetTangents(tans);
        m.SetTriangles(tris, 0);
        // Give the bounds vertical padding so wave displacement never gets frustum-culled at the edges.
        var b = m.bounds; b.Expand(new Vector3(0, 20f, 0)); m.bounds = b;
        m.UploadMeshData(markNoLongerReadable: true);
        return m;
    }
}
