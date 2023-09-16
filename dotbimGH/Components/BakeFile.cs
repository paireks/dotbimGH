using Grasshopper.Kernel;
using System;


namespace dotbimGH.Components
{
    public class BakeFile : GH_Component
    {
        public BakeFile()
            : base("Bake File From Path", "Bake File From Path", "Bakes file from given path", "dotbim", "Bake")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path to .bim file", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Bake", "Bake", "Bake File", GH_ParamAccess.item, false);
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

                foreach (var geo in rhinoGeometries)
                {
                    if (geo != null)
                    {
                        int id = rhinoGeometries.IndexOf(geo);
                        var minfo = model.Elements[id].Info;
                        var attributes = new Rhino.DocObjects.ObjectAttributes();

                        string mguid = model.Elements[id].Guid;
                        string mtype = model.Elements[id].Type;

                        attributes.SetUserString("Guid", mguid);
                        attributes.SetUserString("Type", mtype);
                        attributes.ObjectId = Guid.Parse(mguid);
                        foreach (var kvp in minfo)
                        {
                            attributes.SetUserString(kvp.Key, kvp.Value);
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
                return Properties.Resources.BakeFile;
            }
        }

        public override bool IsBakeCapable => base.IsBakeCapable;

        public override Guid ComponentGuid
        {
            get { return new Guid("13B4AAF1-9074-48B3-9677-4099F70B6FDE"); }
        }
    }

}