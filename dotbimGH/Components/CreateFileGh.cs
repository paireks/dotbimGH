using dotbim;
using dotbimGH.Interfaces;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;

namespace dotbimGH.Components
{
    public class CreateFileGh : GH_Component
    {
        public CreateFileGh()
            : base("Create File", "Create File", "Creates .bim file", "dotbim", "Create File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements Sets or Elements", "Elements Sets or Elements", "Elements Sets and Elements to place in file", GH_ParamAccess.list);
            pManager.AddGenericParameter("Info", "Info", "Information about file", GH_ParamAccess.item);
            pManager.AddTextParameter("Path", "Path", "Path to file, should end up with .bim", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IElementSetConvertable> elementSetConvertables = new List<IElementSetConvertable>();
            Dictionary<string, string> info = null;
            string path = string.Empty;

            DA.GetDataList(0, elementSetConvertables);
            DA.GetData(1, ref info);
            DA.GetData(2, ref path);

            File file = Tools.CreateFile(elementSetConvertables, info);
            file.Save(path);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.CreateFile;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ea80e5c1-79f5-463d-bd9c-6d315577c746"); }
        }
    }
}