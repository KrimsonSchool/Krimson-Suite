using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[ScriptedImporter(1, "krmsn")]
public class KrimsonImporter : ScriptedImporter
{
    public string meshName = "ERROR";
    public Material material;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        if(material==null)
            material = GraphicsSettings.currentRenderPipeline.defaultMaterial;
        
        string[] lines = File.ReadAllLines(ctx.assetPath);

        if (lines.Length != 0)
        {
            //find header
            var l = lines[0];
            var tokens = l.Split(' ');

            List<Color> color = new();
            switch (tokens[0])
            {
                case "img":
                {
                    int width = int.Parse(tokens[1]);
                    int height = int.Parse(tokens[2]);

                    Texture2D tex = new Texture2D(width, height);

                    //split each rgba fragment by comma

                    foreach (var lin in lines)
                    {
                        if (lin.StartsWith("#"))
                            continue;
                        //each spaced word on a line
                        var fragments = lin.Split(' ');
                        foreach (var frag in fragments)
                        {
                            //Debug.Log(frag);
                            var col = frag.Split(',');

                            if (col.Length > 3)
                            {
                                color.Add(new Color(float.Parse(col[0]), float.Parse(col[1]), float.Parse(col[2]), float.Parse(col[3])));
                            }
                        }
                    }

                    int index = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            tex.SetPixel(i, j, color[index]);
                            index++;
                        }
                    }

                    tex.Apply();
                    tex.filterMode = FilterMode.Point;

                    ctx.AddObjectToAsset("Texture", tex);
                    ctx.SetMainObject(tex);
                    break;
                }
                case "msh":
                    string name = "mesh";
                    List<Vector3> vertex = new();
                    List<Vector2> uv = new();
                    List<Vector3> normal = new();
                    List<int> indices = new();

                    Dictionary<string, int> vertexLookup = new();

                    List<Vector3> objVertex = new();
                    List<Vector2> objUv = new();
                    List<Vector3> objNormal = new();

                    foreach (var lin in lines)
                    {
                        if (lin.StartsWith("v "))
                        {
                            string[] vars = lin.Split(' ');
                            objVertex.Add(new Vector3(float.Parse(vars[1]), float.Parse(vars[2]), float.Parse(vars[3])));
                        }
                        else if (lin.StartsWith("vt "))
                        {
                            string[] vars = lin.Split(' ');
                            objUv.Add(new Vector2(float.Parse(vars[1]), float.Parse(vars[2])));
                        }
                        else if (lin.StartsWith("vn "))
                        {
                            string[] vars = lin.Split(' ');
                            objNormal.Add(new Vector3(float.Parse(vars[1]), float.Parse(vars[2]), float.Parse(vars[3])));
                        }

                        if (lin.StartsWith("f "))
                        {
                            string[] faces = lin.Split(' ');
                            foreach (string face in faces)
                            {
                                if (face == "f")
                                {
                                    continue;
                                }

                                //vertex uv normal
                                string[] vuvn_S = face.Split('/');
                                int[] vuvn = new int[vuvn_S.Length];
                                vuvn[0] = int.Parse(vuvn_S[0]);
                                vuvn[1] = int.Parse(vuvn_S[1]);
                                vuvn[2] = int.Parse(vuvn_S[2]);

                                //v uv and n index
                                string key = vuvn[0] + "_" + vuvn[1] + "_" + vuvn[2];

                                if (!vertexLookup.TryGetValue(key, out int existingIndex))
                                {
                                    int newIndex = vertex.Count;
                                    vertex.Add(objVertex[vuvn[0] - 1]);
                                    uv.Add(objUv[vuvn[1] - 1]);
                                    normal.Add(objNormal[vuvn[2] - 1]);

                                    vertexLookup.Add(key, newIndex);
                                    indices.Add(newIndex);
                                }
                                else
                                {
                                    indices.Add(existingIndex);
                                }
                            }
                        }
                    }

                    Mesh mesh = new();
                    if (meshName != "ERROR")
                        name = meshName;
                    else
                        name = mesh.name;

                    mesh.SetVertices(vertex);
                    mesh.SetUVs(0, uv);
                    mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
                    mesh.SetNormals(normal);
                    mesh.name = name;
                    meshName = name;
                    //mesh.SetTriangles(indices.ToArray(), 0);

                    GameObject gameObject = new GameObject();
                    gameObject.name = name;
                    gameObject.AddComponent<MeshFilter>().mesh = mesh;
                    gameObject.AddComponent<MeshRenderer>().material = material;

                    ctx.AddObjectToAsset("GameObject", gameObject);
                    ctx.AddObjectToAsset("Mesh", mesh);
                    ctx.SetMainObject(gameObject);
                    break;
            }
        }
    }
}

/*
Mesh mesh = new();
   List<Vector3> vertex = new();
   vertex.Add(new Vector3(0,0,0));
   vertex.Add(new Vector3(1,0,0));
   vertex.Add(new Vector3(1,1,0));
   vertex.Add(new Vector3(0,1,0));
   mesh.SetVertices(vertex);

   List<Vector2> uv = new();
   uv.Add(new Vector2(0,0));
   uv.Add(new Vector2(0,1));
   uv.Add(new Vector2(1,1));
   uv.Add(new Vector2(1,0));
   mesh.SetUVs(0, uv);

   List<int> indices = new();
   indices.Add(0);
   indices.Add(1);
   indices.Add(2);
   indices.Add(0);
   indices.Add(2);
   indices.Add(3);
   mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

   ctx.AddObjectToAsset("Mesh", mesh);
   ctx.SetMainObject(mesh);
   */


/*

//Debug.Log(lin);
   if (lin.StartsWith('o'))
   {
       name = lin.Split(' ')[1];
   }
   else if (lin.StartsWith("v "))
   {
       string[] vars = lin.Split(' ');
       Debug.Log(vars[0] + " " + vars[1] + " " + vars[2] + " " + vars[3]);
       vertex.Add(new Vector3(float.Parse(vars[1]), float.Parse(vars[2]), float.Parse(vars[3])));
   }
   else if (lin.StartsWith("vt"))
   {
       string[] vars = lin.Split(' ');
       uv.Add(new Vector2(float.Parse(vars[1]), float.Parse(vars[2])));
   }
   else

   */