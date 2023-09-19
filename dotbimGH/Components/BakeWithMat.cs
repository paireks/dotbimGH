using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace dotbimGH.Components
{
    public class BakeWithMat : GH_Component
    {

        private Dictionary<(Color, string), Rhino.Render.RenderMaterial> materialCache = new Dictionary<(Color, string), Rhino.Render.RenderMaterial>();
        private Dictionary<(Color, string), int> layerCache = new Dictionary<(Color, string), int>();
        private bool createLayer = false;

        public BakeWithMat()
            : base("Bake With Material", "Bake With Material", "Separate meshes by color and Assign Material", "dotbim", "Bake")
        {
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendSeparator(menu);
            ToolStripMenuItem setTrace = Menu_AppendItem(menu, "Create Layers", CreateLayerMenu, true, createLayer);
        }

        public void CreateLayerMenu(Object sender, EventArgs e)
        {
            createLayer = !createLayer;
            ExpireSolution(true);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh from Element Geometry", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Name", "Bim name used to create separate materials and layers/sublayers", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("Bake", "Bake", "Add Meshes with material to RhinoDoc", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            string bimName = null;
            bool bake = false;

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref bimName);
            DA.GetData(2, ref bake);

            List<Color> colors = new List<Color>();

            Dictionary<Color, List<int>> colorIndices = new Dictionary<Color, List<int>>();
            var doc = Rhino.RhinoDoc.ActiveDoc;


            if (string.IsNullOrEmpty(bimName))
            {
                //bimName = "BimLayer";
                return;
            }

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

            foreach (var kvp in colorIndices)
            {
                Color color = kvp.Key;
                List<int> indices = kvp.Value;
                Mesh mergedMesh = new Mesh();
                Rhino.Render.RenderMaterial mat;
                int layerIndex = -1;

                // Create a composite key using both color and bimName
                var compositeKey = (color, bimName);

                // Check if the material is already cached
                if (materialCache.ContainsKey(compositeKey))
                {
                    mat = materialCache[compositeKey];
                }
                else
                {
                    mat = CreateMat(color, bimName);
                    materialCache[compositeKey] = mat;
                }

                if (bake)
                {
                    doc.RenderMaterials.Add(mat);

                    // Check if the layer is already cached
                    if (createLayer)
                    {
                        if (layerCache.ContainsKey(compositeKey))
                        {
                            layerIndex = layerCache[compositeKey];
                        }
                        else
                        {
                            // Find the parent layer "bimLayer" or create it if it doesn't exist
                            Rhino.DocObjects.Layer parentLayer = doc.Layers.FindName(bimName);
                            int parentLayerIndex;

                            if (parentLayer != null)
                            {
                                parentLayerIndex = parentLayer.Index;
                            }
                            else
                            {
                                parentLayerIndex = doc.Layers.Add(bimName, System.Drawing.Color.Black);
                            }

                            // Create a child layer under the parent layer
                            string layerName = $"{bimName}_{mat.Name}";
                            int childLayerIndex = doc.Layers.Add(layerName, color);
                            doc.Layers[childLayerIndex].ParentLayerId = doc.Layers[parentLayerIndex].Id;

                            // Cache the child layer index
                            layerCache[compositeKey] = childLayerIndex;

                            layerIndex = childLayerIndex;
                        }
                    }
                    foreach (int i in indices)
                    {
                        var meshF = MeshFacetoMesh(mesh.Faces[i], mesh);
                        meshF.VertexColors.CreateMonotoneMesh(color);
                        mergedMesh.Append(meshF);
                    }

                    Bake(mergedMesh, mat, doc, layerIndex, createLayer);
                }
            }
        }

        // this can be used to create group names
        string FindUniqueParentLayerName(Rhino.RhinoDoc doc, string baseName)
        {
            int suffix = 1;
            string uniqueName = baseName;

            while (doc.Layers.FindName(uniqueName) != null)
            {
                uniqueName = $"{baseName}_{suffix:D2}";
                suffix++;
            }

            return uniqueName;
        }

        void Bake(Mesh mesh, Rhino.Render.RenderMaterial mat, Rhino.RhinoDoc doc, int layerIndex, bool createLay)
        {
            GH_Document gh_doc = Grasshopper.Instances.ActiveCanvas.Document;
            GH_BakeUtility bakeUtility = new GH_BakeUtility(gh_doc);
            var attributes = new Rhino.DocObjects.ObjectAttributes();
            try
            {
                if (createLay)
                {
                    attributes.RenderMaterial = mat;
                    attributes.LayerIndex = layerIndex;
                }

                else
                {
                    attributes.RenderMaterial = mat;
                }

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

        Rhino.Render.RenderMaterial CreateMat(Color color, string bimName)
        {
            double transp = (double)color.A / 255;

            var doc = Rhino.RhinoDoc.ActiveDoc;
            Rhino.DocObjects.Material material = Rhino.DocObjects.Material.DefaultMaterial;

            material.Name = $"{bimName}_{"bimMat"}";
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

        public override bool Read(GH_IReader reader)
        {
            if (reader.ItemExists("CreateLayer"))
                createLayer = reader.GetBoolean("CreateLayer");

            return base.Read(reader);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("CreateLayer", createLayer);
            return base.Write(writer);
        }
    }
}
