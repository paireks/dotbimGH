using System.Collections.Generic;
using Rhino.Geometry;
using Mesh = Rhino.Geometry.Mesh;

namespace dotbimGH
{
    public class BimElementSet
    {

        public BimElementSet(Mesh mesh, List<Plane> insertPlanes, List<string> guids,
            List<string> types, List<System.Drawing.Color> colors, List<Dictionary<string, string>> infos)
        {
            Mesh = mesh;
            InsertPlanes = insertPlanes;
            Guids = guids;
            Types = types;
            Colors = colors;
            Infos = infos;
            PreviewMeshes = CreatePreviewMeshes();
        }
        
        private List<Mesh> CreatePreviewMeshes()
        {
            List<Mesh> previewMeshes = new List<Mesh>();
            
            for (int i = 0; i < InsertPlanes.Count; i++)
            {
                Mesh previewMesh = Mesh.DuplicateMesh();
                previewMesh.Transform(Transform.PlaneToPlane(Plane.WorldXY, InsertPlanes[i]));
                previewMesh.VertexColors.CreateMonotoneMesh(Colors[i]);
                previewMeshes.Add(previewMesh);
            }

            return previewMeshes;
        }
        
        public Mesh Mesh { get; }
        public List<Plane> InsertPlanes { get; }
        public List<string> Guids { get; }
        public List<string> Types { get; }
        public List<System.Drawing.Color> Colors { get; }
        public List<Dictionary<string, string>> Infos { get; }
        public List<Mesh> PreviewMeshes { get; }
    }
}