using System;
using System.Collections.Generic;
using System.Linq;
using dotbim;
using Rhino.Geometry;
using Mesh = Rhino.Geometry.Mesh;

namespace dotbimGH
{
    public static class Tools
    {
        private static Mesh CreateRhinoMeshFromBimMesh(dotbim.Mesh bimMesh)
        {
            Mesh mesh = new Mesh();
            for (int i = 0; i < bimMesh.Coordinates.Count; i+=3)
            {
                mesh.Vertices.Add(bimMesh.Coordinates[i], bimMesh.Coordinates[i+1], bimMesh.Coordinates[i+2]);
            }
            
            for (int i = 0; i < bimMesh.Indices.Count; i+=3)
            {
                mesh.Faces.AddFace(bimMesh.Indices[i], bimMesh.Indices[i+1], bimMesh.Indices[i+2]);
            }

            mesh.Normals.ComputeNormals();
            mesh.Compact();

            return mesh;
        }

        private static System.Drawing.Color CreateColorFromBimColor(Color bimColor)
        {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(bimColor.A, bimColor.R, bimColor.G, bimColor.B);
            return color;
        }

        private static List<double> GetBimVerticesCoordinatesFromRhinoMesh(Mesh rhinoMesh)
        {
            List<double> verticesCoordinates = new List<double>();

            foreach (var rhinoMeshVertex in rhinoMesh.Vertices)
            {
                verticesCoordinates.AddRange(new List<double>
                {
                    rhinoMeshVertex.X,
                    rhinoMeshVertex.Y,
                    rhinoMeshVertex.Z
                });
            }

            return verticesCoordinates;
        }

        private static Color GetBimColorFromDrawingColor(System.Drawing.Color drawingColor)
        {
            Color color = new Color
            {
                R = drawingColor.R,
                G = drawingColor.G,
                B = drawingColor.B,
                A = drawingColor.A
            };

            return color;
        }

        private static List<int> GetBimFacesIdsFromRhinoMesh(Mesh rhinoMesh)
        {
            List<int> facesIds = new List<int>();

            for (int i = 0; i < rhinoMesh.Faces.Count; i++)
            {
                var rhinoMeshFace = rhinoMesh.Faces[i];
                if (rhinoMeshFace.C != rhinoMeshFace.D)
                {
                    throw new ArgumentException("Face index: " + i + " is not triangular. Triangulate mesh.");
                }
                facesIds.AddRange(new List<int>
                {
                    rhinoMeshFace.A, rhinoMeshFace.B, rhinoMeshFace.C
                });
            }

            return facesIds;
        }

        private static dotbim.Mesh CreateBimMeshFromRhinoMesh(Mesh mesh, int meshId)
        {
            dotbim.Mesh bimMesh = new dotbim.Mesh
            {
                MeshId = meshId,
                Coordinates = GetBimVerticesCoordinatesFromRhinoMesh(mesh),
                Indices = GetBimFacesIdsFromRhinoMesh(mesh)
            };

            return bimMesh;
        }

        public static File CreateFile(List<BimElementSet> bimElementSets, Dictionary<string, string> info)
        {
            List<dotbim.Mesh> meshes = new List<dotbim.Mesh>();
            List<Element> elements = new List<Element>();

            int currentMeshId = 0;
            foreach (var bimElementSet in bimElementSets)
            {
                meshes.Add(CreateBimMeshFromRhinoMesh(bimElementSet.Mesh, currentMeshId));
                for (int i = 0; i < bimElementSet.InsertPlanes.Count; i++)
                {
                    Element element = new Element
                    {
                        MeshId = currentMeshId,
                        Vector = ConvertInsertPlaneToVector(bimElementSet.InsertPlanes[i]),
                        Rotation = ConvertInsertPlaneToRotation(bimElementSet.InsertPlanes[i]),
                        Color = GetBimColorFromDrawingColor(bimElementSet.Colors[i]),
                        Guid = bimElementSet.Guids[i],
                        Info = bimElementSet.Infos[i],
                        Type = bimElementSet.Types[i]
                    };
                    
                    elements.Add(element);
                }

                currentMeshId += 1;
            }
            
            File file = new File
            {
                SchemaVersion = "1.0.0",
                Meshes = meshes,
                Elements = elements,
                Info = info
            };

            return file;
        }

        private static Vector ConvertInsertPlaneToVector(Plane plane)
        {
            var p1 = Plane.WorldXY.Origin;
            var p2 = plane.Origin;
            
            return new Vector{X = p2.X - p1.X, Y = p2.Y - p1.Y, Z = p2.Z - p1.Z};
        }

        private static Rotation ConvertInsertPlaneToRotation(Plane plane)
        {
            Quaternion quaternion = new Quaternion();
            quaternion.SetRotation(Plane.WorldXY, plane);

            return new Rotation{Qx = quaternion.B, Qy = quaternion.C, Qz = quaternion.D, Qw = quaternion.A};
        }

        private static void IsQuaternionAccepted(Quaternion quaternion)
        {
            double tolerance = 0.001;
            if (quaternion.A-tolerance <= 0 && quaternion.B-tolerance <= 0 && quaternion.C-tolerance <= 0 && quaternion.D-tolerance <= 0)
            {
                throw new ArgumentException("Quaternion with all values as 0 are not supported.");
            }
        }

        public static List<Mesh> ConvertBimMeshesAndElementsIntoRhinoMeshes(List<dotbim.Mesh> bimMeshes, List<Element> bimElements)
        {
            List<Mesh> meshes = new List<Mesh>();

            foreach (var bimElement in bimElements)
            {
                var bimMeshReferenced = bimMeshes.First(t => t.MeshId == bimElement.MeshId);
                Mesh meshReferenced = CreateRhinoMeshFromBimMesh(bimMeshReferenced);
                
                Transform moveTransform = Transform.Translation(bimElement.Vector.X, bimElement.Vector.Y, bimElement.Vector.Z);
                Quaternion quaternion = new Quaternion(bimElement.Rotation.Qw, bimElement.Rotation.Qx, bimElement.Rotation.Qy, bimElement.Rotation.Qz);
                IsQuaternionAccepted(quaternion);
                quaternion.GetRotation(out var insertPlane);
                Transform rotationTransform = Transform.PlaneToPlane(Plane.WorldXY, insertPlane);
                meshReferenced.Transform(rotationTransform);
                meshReferenced.Transform(moveTransform);
                meshReferenced.VertexColors.CreateMonotoneMesh(CreateColorFromBimColor(bimElement.Color));
                meshes.Add(meshReferenced);
            }

            return meshes;
        }
    }
}