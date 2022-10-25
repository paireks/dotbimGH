using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Color = System.Drawing.Color;
using Mesh = Rhino.Geometry.Mesh;

namespace dotbimGH.Components
{
    public class CreateElementGh : GH_Component
    {
        public CreateElementGh()
            : base("Create Element", "Create Element", "Creates Element", "dotbim", "Create File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh object", GH_ParamAccess.item);
            pManager.AddTextParameter("Guid", "Guid", "Guid for element", GH_ParamAccess.item);
            pManager.AddTextParameter("Type", "Type", "Element type", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "Color", "Color for element", GH_ParamAccess.item);
            pManager.AddGenericParameter("Info", "Info", "Information about element", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Element", "Element", "Element created", GH_ParamAccess.item);
            pManager.AddMeshParameter("Preview", "Preview", "Preview of element", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            string guid = string.Empty;
            string type = string.Empty;
            Color color = Color.Empty;
            Dictionary<string, string> info = new Dictionary<string, string>();

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref guid);
            DA.GetData(2, ref type);
            DA.GetData(3, ref color);
            DA.GetData(4, ref info);

            BimElement bimElement = new BimElement(mesh, guid, type, color, info);

            DA.SetData(0, bimElement);
            DA.SetData(1, bimElement.PreviewMesh);
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
            get { return new Guid("25b35908-122d-44b6-a6f1-cffc0b0f0249"); }
        }
    }
}