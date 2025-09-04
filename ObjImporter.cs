using Raylib_cs;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

public class ObjImporter
{
    public static Mesh ObjLoad(string path)
    {
        string[] Lines = File.ReadAllLines(path);
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> TextureCords = new List<Vector2>();
        List<Faces> MeshFaces = new List<Faces>();
        //Load File
        for (int a = 0; a < Lines.Length; a++)
        {
            string[] splitLine = Lines[a].Split(' ');
            var Check = splitLine.ToList();
            Check.Remove("");
            splitLine = Check.ToArray();

            if (Lines[a].StartsWith("v "))
            {
                Vector3 vector3 = new Vector3();
                vector3.X = float.Parse(splitLine[1], CultureInfo.InvariantCulture.NumberFormat);
                vector3.Y = float.Parse(splitLine[2], CultureInfo.InvariantCulture.NumberFormat);
                vector3.Z = float.Parse(splitLine[3], CultureInfo.InvariantCulture.NumberFormat);
                vertices.Add(vector3);
            }

            if (Lines[a].StartsWith("vt "))
            {
                Vector2 vector2 = new Vector2();
                vector2.X = float.Parse(splitLine[1], CultureInfo.InvariantCulture.NumberFormat);
                vector2.Y = -float.Parse(splitLine[2], CultureInfo.InvariantCulture.NumberFormat);
                TextureCords.Add(vector2);
            }

            if (Lines[a].StartsWith("vn "))
            {
                Vector3 vector3 = new Vector3();
                vector3.X = float.Parse(splitLine[1], CultureInfo.InvariantCulture.NumberFormat);
                vector3.Y = float.Parse(splitLine[2], CultureInfo.InvariantCulture.NumberFormat);
                vector3.Z = float.Parse(splitLine[3], CultureInfo.InvariantCulture.NumberFormat);
                normals.Add(vector3);
            }

            if (Lines[a].StartsWith("f "))
            {
                Faces faces = new Faces();

                string[] SplitPoint = splitLine[1].Split('/');

                faces.V1Pos = int.Parse(SplitPoint[0]) - 1;
                if (SplitPoint[1] != "")
                {
                    faces.UV1Pos = int.Parse(SplitPoint[1]) - 1;
                }
                faces.Normal1Pos = int.Parse(SplitPoint[2]) - 1;

                SplitPoint = splitLine[2].Split('/');
                faces.V2Pos = int.Parse(SplitPoint[0]) - 1;
                if (SplitPoint[1] != "")
                {
                    faces.UV2Pos = int.Parse(SplitPoint[1]) - 1;
                }
                faces.Normal2Pos = int.Parse(SplitPoint[2]) - 1;

                SplitPoint = splitLine[3].Split('/');
                faces.V3Pos = int.Parse(SplitPoint[0]) - 1;
                if (SplitPoint[1] != "")
                {
                    faces.UV3Pos = int.Parse(SplitPoint[1]) - 1;
                }
                faces.Normal3Pos = int.Parse(SplitPoint[2]) - 1;

                MeshFaces.Add(faces);
            }
        }

        //Generate New Mesh

        Mesh mesh = new Mesh(MeshFaces.Count*3,MeshFaces.Count);
        mesh.AllocVertices();
        mesh.AllocTexCoords();
        mesh.AllocIndices();
        mesh.AllocNormals();

        Span<Vector3> NewVertices = mesh.VerticesAs<Vector3>();
        Span<Vector2> NewTextureCords = mesh.TexCoordsAs<Vector2>();
        Span<Vector3> NewNormals = mesh.NormalsAs<Vector3>();
        Span<ushort> Indices = mesh.IndicesAs<ushort>();

        for (int i = 0; i < MeshFaces.Count; i++)
        {
            NewVertices[3*i+0] = vertices[MeshFaces[i].V1Pos];
            NewVertices[3 * i + 1] = vertices[MeshFaces[i].V2Pos];
            NewVertices[3 * i + 2] = vertices[MeshFaces[i].V3Pos];

            if (TextureCords.Count != 0)
            {
                NewTextureCords[3 * i + 0] = TextureCords[MeshFaces[i].UV1Pos];
                NewTextureCords[3 * i + 1] = TextureCords[MeshFaces[i].UV2Pos];
                NewTextureCords[3 * i + 2] = TextureCords[MeshFaces[i].UV3Pos];
            }

            NewNormals[3 * i + 0] = normals[MeshFaces[i].Normal1Pos];
            NewNormals[3 * i + 1] = normals[MeshFaces[i].Normal2Pos];
            NewNormals[3 * i + 2] = normals[MeshFaces[i].Normal3Pos];

            Indices[3 * i + 0] = (ushort)(3 * i);
            Indices[3 * i + 1] = (ushort)(3 * i + 1);
            Indices[3 * i + 2] = (ushort)(3 * i + 2);
        }

        //mesh.Vertices = NewVertices.ToArray();

        //mesh.vertices = NewVertices.ToArray();
        //mesh.normals = NewNormals.ToArray();
        //mesh.uv = NewTextureCords.ToArray();
        //mesh.triangles = Indices.ToArray();
        //mesh.Optimize();
        //mesh.RecalculateNormals();

        return mesh;
    }
    public struct Faces
    {
        public Vector3 V1;
        public Vector3 V2;
        public Vector3 V3;

        public int V1Pos;
        public int V2Pos;
        public int V3Pos;

        public Vector2 UV1;
        public Vector2 UV2;
        public Vector2 UV3;

        public int UV1Pos;
        public int UV2Pos;
        public int UV3Pos;

        public Vector3 Normal1;
        public Vector3 Normal2;
        public Vector3 Normal3;

        public int Normal1Pos;
        public int Normal2Pos;
        public int Normal3Pos;
    }
}
