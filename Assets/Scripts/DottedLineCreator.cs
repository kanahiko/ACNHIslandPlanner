using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class DottedLineCreator : MonoBehaviour
{
    public Vector3 size = new Vector3(4,4,1);
    public float segmentSize = 0.5f;
    public float thickness = 0.2f;

    public bool create;
    public bool save;

    public Mesh mesh;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public string fileName = "test";

    public GameObject test;

    // Start is called before the first frame update
    void Start()
    {
        if (test)
        {
            Debug.Log(test.name);
            meshFilter.mesh = test.GetComponentInChildren<MeshFilter>().sharedMesh;
            string ttt = MA_TextureAtlasserPro.ObjExporter.MeshToString(test.GetComponentInChildren<MeshFilter>().sharedMesh, test.name);
            using (StreamWriter sw = new StreamWriter(@"C:\Users\a.moskaleva\Downloads\workacnh\ACNHIslandPlanner\Assets\Bridges\Test.obj"))
            {
                sw.Write(ttt);
            }
            return;
        }
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        CreateDottedLine();
    }

    private void FixedUpdate()
    {
        if (create)
        {
            create = false;
            CreateDottedLine();
        }
        if (save)
        {
            save = false; 
            //string path = FileUtil.GetProjectRelativePath();
            #if UNITY_EDITOR
            AssetDatabase.CreateAsset(mesh, $"Assets /Borders/border_{(int)size.x}_{(int)size.y}_{(int)size.z}_.asset");
            AssetDatabase.SaveAssets();
            #endif
        }
    }
    void CreateDottedLine()
    {
        int count = Mathf.CeilToInt(size.x / segmentSize);
        Debug.Log(count);
        bool dotLine = true;
        Vector3 offset = new Vector3(0,0.0015f,0);
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();


        CreateHorizontalDottedLine(Vector3.zero, ref vertices, ref triangles);
        CreateVerticalDottedLine(Vector3.zero, ref vertices, ref triangles);
        CreateHorizontalDottedLine(new Vector3(0,0, size.y - thickness), ref vertices, ref triangles);
        CreateVerticalDottedLine(new Vector3(size.x - thickness, 0, 0), ref vertices, ref triangles);

        if (size.z > 0.1f)
        {
            CreateHorizontalDottedLine(new Vector3(0, 0, size.z - thickness), ref vertices, ref triangles);
        }

        if (mesh == null)
        {
            mesh = new Mesh();
        }
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        meshFilter.sharedMesh = mesh;
    }
    void CreateVerticalDottedLine(Vector3 additionalOffset, ref List<Vector3> vertices, ref List<int> triangles)
    {
        int count = Mathf.CeilToInt(size.y / segmentSize);
        bool dotLine = true;
        Vector3 offset = new Vector3(0, 0.0015f, 0) + additionalOffset;
        for (int i = 0; i < count; i++)
        {
            if (dotLine)
            {
                float sizeY = segmentSize;
                if (offset.z + sizeY > size.y)
                {
                    sizeY -= (offset.z + sizeY - size.y);
                }
                CreateDot(thickness, sizeY, offset, ref vertices, ref triangles);
            }
            dotLine = !dotLine;
            offset.z += segmentSize;
        }
    }
    void CreateHorizontalDottedLine(Vector3 additionalOffset, ref List<Vector3> vertices, ref List<int> triangles)
    {
        int count = Mathf.CeilToInt(size.x / segmentSize);
        bool dotLine = true;
        Vector3 offset = new Vector3(0, 0.0015f, 0) + additionalOffset;
        for (int i = 0; i < count; i++)
        {
            if (dotLine)
            {
                float sizeX = segmentSize;
                if (offset.x + sizeX > size.x)
                {
                    sizeX -= (offset.x + sizeX - size.x);
                }
                CreateDot(sizeX, thickness, offset, ref vertices, ref triangles);
            }
            dotLine = !dotLine;
            offset.x += segmentSize;

        }
    }
    void CreateDot(float sizeX, float sizeY, Vector3 offset, ref List<Vector3> vertices, ref List<int> triangles)
    {
        int indexOffest = vertices.Count;
        vertices.Add(new Vector3(0, 0, 0) + offset);
        vertices.Add(new Vector3(sizeX, 0, 0) + offset);
        vertices.Add(new Vector3(0, 0, sizeY) + offset);
        vertices.Add(new Vector3(sizeX, 0, sizeY) + offset);

        triangles.AddRange(new List<int> {0 + indexOffest, 3 + indexOffest, 1 + indexOffest, 
            0 + indexOffest, 2 + indexOffest, 3 + indexOffest });
    }
}
