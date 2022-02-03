using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;

namespace dotbimGH.Components
{
    public class DeconstructInfoGh : GH_Component
    {
        public DeconstructInfoGh()
            : base("Deconstruct Info", "Deconstruct Info", "Deconstructs Info", "dotbim", "Read File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Info", "Info", "Info read", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Key", "Key", "Keys deconstructed", GH_ParamAccess.list);
            pManager.AddTextParameter("Value", "Value", "Values deconstructed", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Dictionary<string, string> info = null;
            DA.GetData(0, ref info);

            DA.SetDataList(0, info.Keys.ToList());
            DA.SetDataList(1, info.Values.ToList());
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
            get { return new Guid("bc56445d-d724-41ac-b508-2c240f1a6502"); }
        }
    }

}