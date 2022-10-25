using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Color = System.Drawing.Color;
using Mesh = Rhino.Geometry.Mesh;

namespace dotbimGH.Components
{
    public class CreateSimpleElementGh : GH_Component
    {
        public CreateSimpleElementGh()
            : base("Create Simple Element", "Create Simple Element", "Creates Simple Element", "dotbim", "Create Elements")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh object", GH_ParamAccess.item);
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
            string type = string.Empty;
            Color color = Color.Empty;
            Dictionary<string, string> info = new Dictionary<string, string>();

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref type);
            DA.GetData(2, ref color);
            DA.GetData(3, ref info);

            BimElement bimElement = new BimElement(mesh, type, color, info);

            DA.SetData(0, bimElement);
            DA.SetData(1, bimElement.PreviewMesh);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.CreateSimpleElement;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ffaf000b-824b-46f9-aa8e-2fa44b1dc011"); }
        }
    }
}