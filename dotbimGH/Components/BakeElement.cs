using Grasshopper.Kernel;
using System;
using System.Linq;

namespace dotbimGH.Components
{
    public class BakeElement : GH_Component
    {
        public BakeElement()
            : base("Bake Element", "BakeElement", "BakeElement", "dotbim", "Bake BIM")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("File", "File", "File read", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("Bake", "Bake", "Bake Element", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string file = null;
            bool bake = false;

            DA.GetData(0, ref file);
            DA.GetData(1, ref bake);

            if (bake)
                Bake(file);
        }

        void Bake(string filename)
        {
            var doc = Rhino.RhinoDoc.ActiveDoc;
            try
            {
                // Use the dotbim library to open and process the BIM file
                var model = dotbim.File.Read(filename);
                var rhinoGeometries = Tools.ConvertBimMeshesAndElementsIntoRhinoMeshes(model.Meshes, model.Elements);
                var fileinfo = model.Info;
                var filekeys = fileinfo.Keys.ToList();
                var filevalues = fileinfo.Values.ToList();

                foreach (var geo in rhinoGeometries)
                {
                    if (geo != null)
                    {
                        int id = rhinoGeometries.IndexOf(geo);
                        var minfo = model.Elements[id].Info;
                        var attributes = new Rhino.DocObjects.ObjectAttributes();

                        string mguid = model.Elements[id].Guid;
                        string mmshid = model.Elements[id].MeshId.ToString();
                        string mrot = Math.Round(model.Elements[id].Rotation.Qw, 3).ToString() + ", " +
                                        Math.Round(model.Elements[id].Rotation.Qx, 3).ToString() + ", " +
                                        Math.Round(model.Elements[id].Rotation.Qy, 3).ToString() + ", " +
                                        Math.Round(model.Elements[id].Rotation.Qz, 3).ToString();
                        string mvect = Math.Round(model.Elements[id].Vector.X, 3).ToString() + ", " +
                                       Math.Round(model.Elements[id].Vector.Y, 3).ToString() + ", " +
                                       Math.Round(model.Elements[id].Vector.Z, 3).ToString();
                        string mtype = model.Elements[id].Type;
                        string mcolor = model.Elements[id].Color.A.ToString() + ", " +
                                        model.Elements[id].Color.R.ToString() + ", " +
                                        model.Elements[id].Color.G.ToString() + ", " +
                                        model.Elements[id].Color.B.ToString();

                        // assign atributes to the meshes to use in Rhino

                        foreach (var fkey in filekeys)
                        {
                            int fid = filekeys.IndexOf(fkey);
                            attributes.SetUserString("File Info: " + fkey, filevalues[fid]);
                        }

                        attributes.SetUserString("Guid", mguid);
                        attributes.SetUserString("Mesh ID", mmshid);
                        attributes.SetUserString("Rotation", mrot);
                        attributes.SetUserString("Vector", mvect);
                        attributes.SetUserString("Type", mtype);
                        attributes.SetUserString("Color", mcolor);
                        attributes.ObjectId = Guid.Parse(mguid);
                        foreach (var kvp in minfo)
                        {
                            attributes.SetUserString("Info: " + kvp.Key, kvp.Value);
                        }

                        doc.Objects.Add(geo, attributes);
                    }
                }

                doc.Views.Redraw();

            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error opening BIM file: {ex.Message}");
            }

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.bakebim;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("13B4AAF1-9074-48B3-9677-4099F70B6FDE"); }
        }
    }

}