using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace dotbimGH.Components
{
    public class CreateGuidGh : GH_Component
    {
        public CreateGuidGh()
            : base("Create Guid", "Create Guid", "Create Guid", "dotbim", "Create File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Count", "Count", "Number of Guids to generate", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Guid", "Guid", "Guid created", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int count = 0;

            DA.GetData(0, ref count);

            List<string> guids = new List<string>();

            for (int i = 0; i < count; i++)
            {
                guids.Add(Guid.NewGuid().ToString());
            }

            DA.SetDataList(0, guids);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.CreateGuid;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("b1d9ed12-626c-4a78-a5a1-aed4c7a0e910"); }
        }
    }
}