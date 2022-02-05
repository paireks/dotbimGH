using System;
using dotbim;
using Grasshopper.Kernel;

namespace dotbimGH.Components
{
    public class ReadFileGh : GH_Component
    {
        public ReadFileGh()
            : base("Read File", "Read File", "Reads .bim file", "dotbim", "Read File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path to .bim file", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("File", "File", "File read", GH_ParamAccess.item);
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
                return Properties.Resources.ReadFile;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("b8675443-c974-4685-a072-14052fb30852"); }
        }
    }

}