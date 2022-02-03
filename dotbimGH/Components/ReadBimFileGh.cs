using System;
using dotbim;
using Grasshopper.Kernel;

namespace dotbimGH.Components
{
    public class ReadBimFileGh : GH_Component
    {
        public ReadBimFileGh()
            : base("Read BIM File", "Read BIM File", "Reads BIM File", "dotbim", "Read File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path to .bim file", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("BIM File", "BIM File", "BIM File read", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = string.Empty;
            DA.GetData(0, ref path);

            DA.SetData(0, File.Read(path));
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
            get { return new Guid("b8675443-c974-4685-a072-14052fb30852"); }
        }
    }

}