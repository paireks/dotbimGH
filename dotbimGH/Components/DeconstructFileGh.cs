using System;
using dotbim;
using Grasshopper.Kernel;

namespace dotbimGH.Components
{
    public class DeconstructFileGh : GH_Component
    {
        public DeconstructFileGh()
            : base("Deconstruct File", "Deconstruct File", "Deconstructs .bim file", "dotbim", "Read File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("File", "File", "File read", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Schema Version", "Schema Version", "Version of schema", GH_ParamAccess.item);
            pManager.AddGenericParameter("Info", "Info", "Info deconstructed about file", GH_ParamAccess.item);
            pManager.AddGenericParameter("Elements Geometry", "Elements Geometry", "Elements Geometry deconstructed", GH_ParamAccess.list);
            pManager.AddGenericParameter("Elements Data", "Elements Data", "Elements Data deconstructed", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            File file = null;
            DA.GetData(0, ref file);

            DA.SetData(0, file.SchemaVersion);
            DA.SetData(1, file.Info);
            DA.SetDataList(2, Tools.ConvertBimMeshesAndElementsIntoRhinoMeshes(file.Meshes, file.Elements));
            DA.SetDataList(3, file.Elements);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.DeconstructFile;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("e7c90a9a-28c2-44b5-a02f-77afae97a5e3"); }
        }
    }

}