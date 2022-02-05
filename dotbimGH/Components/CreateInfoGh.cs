using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace dotbimGH.Components
{
    public class CreateInfoGh : GH_Component
    {
        public CreateInfoGh()
            : base("Create Info", "Create Info", "Create Info", "dotbim", "Create File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Keys", "Keys", "Keys", GH_ParamAccess.list);
            pManager.AddTextParameter("Values", "Values", "Values", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Info", "Info", "Info created", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> keys = new List<string>();
            List<string> values = new List<string>();

            DA.GetDataList(0, keys);
            DA.GetDataList(1, values);

            if (keys.Count != values.Count)
            {
                throw new ArgumentException("Keys list length should match values list length");
            }

            Dictionary<string, string> info = new Dictionary<string, string>();
            for (var i = 0; i < keys.Count; i++)
            {
                info.Add(keys[i], values[i]);
            }

            DA.SetData(0, info);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.CreateInfo;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("90dfd01d-2940-46ef-9afd-adb20c3994e9"); }
        }
    }
}