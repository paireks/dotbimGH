using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace dotbimGH.Components
{
    public class BakeWithMat : GH_Component
    {

        private Dictionary<Color, Rhino.Render.RenderMaterial> materialCache = new Dictionary<Color, Rhino.Render.RenderMaterial>();

        public BakeWithMat()
            : base("Bake With Material", "Bake With Material", "Separate meshes by color and Assign Material", "dotbim", "Bake")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh from Element Geometry", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Bake", "Bake", "Add Meshes with material to RhinoDoc", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            bool bake = false;

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref bake);

            List<Color> colors = new List<Color>();

            Dictionary<Color, List<int>> colorIndices = new Dictionary<Color, List<int>>();
            var doc = Rhino.RhinoDoc.ActiveDoc;

            if (mesh == null) return;

            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                MeshFace face = mesh.Faces[i];
                Color color = mesh.VertexColors[face.A];
                colors.Add(color);

                if (!colorIndices.ContainsKey(color))
                {
                    colorIndices[color] = new List<int>();
                }
                colorIndices[color].Add(i);
            }

            if (bake)
            {
                foreach (var kvp in colorIndices)
                {
                    Color color = kvp.Key;
                    List<int> indices = kvp.Value;

                    Mesh mergedMesh = new Mesh();
                    Rhino.Render.RenderMaterial mat;

                    // Check if the material is already cached
                    if (materialCache.ContainsKey(color))
                    {
                        mat = materialCache[color];
                    }
                    else
                    {
                        mat = CreateMat(0, color); // You may need to adjust the material ID (0 in this example)
                        materialCache[color] = mat; // Cache the material for reuse
                    }

                    foreach (int i in indices)
                    {
                        var meshF = MeshFacetoMesh(mesh.Faces[i], mesh);
                        meshF.VertexColors.CreateMonotoneMesh(color);
                        mergedMesh.Append(meshF);
                    }
                    doc.RenderMaterials.Add(mat);

                    Bake(mergedMesh, mat, doc);
                }
            }
        }

        void Bake(Mesh mesh, Rhino.Render.RenderMaterial mat, Rhino.RhinoDoc doc)
        {
            GH_Document gh_doc = Grasshopper.Instances.ActiveCanvas.Document;
            GH_BakeUtility bakeUtility = new GH_BakeUtility(gh_doc);
            try
            {
                var attributes = new Rhino.DocObjects.ObjectAttributes
                {
                    RenderMaterial = mat
                };

                IGH_GeometricGoo convertedMesh = null;
                if (mesh.IsValid)
                {
                    convertedMesh = GH_Convert.ToGeometricGoo(mesh);
                }
                bakeUtility.BakeObject(convertedMesh, attributes, doc);

            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error baking the Mesh: {ex.Message}");
            }
        }

        Mesh MeshFacetoMesh(MeshFace faceToConvert, Mesh originalMesh)
        {
            Mesh convertedMesh = new Mesh();

            // Extract vertices of the MeshFace
            Point3d vertexA = originalMesh.Vertices[faceToConvert.A];
            Point3d vertexB = originalMesh.Vertices[faceToConvert.B];
            Point3d vertexC = originalMesh.Vertices[faceToConvert.C];

            // Add vertices to the new Mesh
            int indexA = convertedMesh.Vertices.Add(vertexA);
            int indexB = convertedMesh.Vertices.Add(vertexB);
            int indexC = convertedMesh.Vertices.Add(vertexC);

            // Add a new face to the Mesh using the vertex indices
            convertedMesh.Faces.AddFace(indexA, indexB, indexC);

            return convertedMesh;
        }

        Rhino.Render.RenderMaterial CreateMat(int id, Color color)
        {
            double transp = (double)color.A / 255;

            var doc = Rhino.RhinoDoc.ActiveDoc;
            Rhino.DocObjects.Material material = Rhino.DocObjects.Material.DefaultMaterial;

            material.Name = "bimMat_" + id.ToString();
            material.DiffuseColor = color;
            material.Reflectivity = Math.Abs(0.95 - transp);
            material.ReflectionColor = System.Drawing.Color.LightGray;
            material.SpecularColor = System.Drawing.Color.Gray;
            material.Transparency = Math.Round(1 - transp, 2);

            material.CommitChanges();
            var renderMaterial = Rhino.Render.RenderMaterial.FromMaterial(material, doc);
            return renderMaterial;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.bimmat;
            }
        }

        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("E456CAAD-9AAE-41CA-8FF5-05C585893447");
            }
        }

    }
}