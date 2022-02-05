using System;
using System.Collections.Generic;
using dotbim;
using Grasshopper.Kernel;

namespace dotbimGH.Components
{
    public class CreateBimFileGh : GH_Component
    {
        public CreateBimFileGh()
            : base("Create File", "Create File", "Creates .bim file", "dotbim", "Create File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements Sets", "Elements Sets", "Elements Sets to place in file", GH_ParamAccess.list);
            pManager.AddGenericParameter("Info", "Info", "Information about file", GH_ParamAccess.item);
            pManager.AddTextParameter("Path", "Path", "Path to file, should end up with .bim", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<BimElementSet> bimElementSets = new List<BimElementSet>();
            Dictionary<string, string> info = null;
            string path = string.Empty;
            
            DA.GetDataList(0, bimElementSets);
            DA.GetData(1, ref info);
            DA.GetData(2, ref path);
            
            File file = Tools.CreateFile(bimElementSets, info);
            file.Save(path);
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
            get { return new Guid("2e9723cf-a0e5-4ec5-b229-e6ab673032a5"); }
        }
    }
}