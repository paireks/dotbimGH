using System;
using dotbim;
using Grasshopper.Kernel;

namespace dotbimGH.Components
{
    public class DeconstructBimFileGh : GH_Component
    {
        public DeconstructBimFileGh()
            : base("Deconstruct BIM File", "Deconstruct BIM File", "Deconstructs BIM File", "BIM", "Read File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("BIM File", "BIM File", "BIM File read", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Schema Version", "Schema Version", "Version of schema", GH_ParamAccess.item);
            pManager.AddGenericParameter("BIM Info", "BIM Info", "BIM Info deconstructed about file", GH_ParamAccess.item);
            pManager.AddGenericParameter("BIM Elements Geometry", "BIM Elements Geometry", "BIM Elements Geometry deconstructed", GH_ParamAccess.list);
            pManager.AddGenericParameter("BIM Elements Data", "BIM Elements Data", "BIM Elements Data deconstructed", GH_ParamAccess.list);
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
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("e7c90a9a-28c2-44b5-a02f-77afae97a5e3"); }
        }
    }

}