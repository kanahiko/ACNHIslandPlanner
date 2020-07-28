#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Globalization;

namespace MA_TextureAtlasserPro
{
    [System.Serializable]
    public class MA_ModelGroup
    {
        public string name = "Model";
        public List<GameObject> meshes = new List<GameObject>();
    }

    [System.Serializable]
    public class MeshGroup
    {
        public GameObject meshGameObject;
        //public Mesh mesh;
        //public string meshName;
    }

    public static class ObjExporter
    {
        public static string MeshToString(Mesh mesh, string name)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("g ").Append(name).Append("\n");

            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector2[] uv = mesh.uv;
            foreach (Vector3 v in vertices)
            {
                sb.Append(string.Format($"v {(-v.x).ToString("G", CultureInfo.CreateSpecificCulture("en-US"))}" +
                    $" {v.y.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))} " +
                    $"{v.z.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))}\n"));
            }
            sb.Append("\n");
            foreach (Vector3 v in normals)
            {
                sb.Append(string.Format($"vn {v.x.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))}" +
                    $" {v.y.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))} " +
                    $"{v.z.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))}\n"));
            }
            sb.Append("\n");
            foreach (Vector3 v in uv)
            {
                sb.Append(string.Format($"vt {v.x.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))} " +
                    $"{v.y.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))}\n"));
            }
            sb.Append("\n");
            for (int material = 0; material < mesh.subMeshCount; material++)
            {
                //sb.Append("\n");
                //sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                //sb.Append("usemap ").Append(mats[material].name).Append("\n");

                int[] triangles = mesh.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(($"f { triangles[i + 1] + 1}/{ triangles[i + 1] + 1}/{ triangles[i + 1] + 1} " +
                        $"{ triangles[i] + 1}/{ triangles[i] + 1}/{ triangles[i] + 1} " +
                        $"{triangles[i + 2] + 1}/{triangles[i + 2] + 1}/{triangles[i + 2] + 1}\n"));
                }
            }
            return sb.ToString();
        }

        public static void MeshToFile(Mesh mf, string name, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(MeshToString(mf, name));
            }
        }
    }
}
#endif