using System;
using Grasshopper.Kernel;
using Color = System.Drawing.Color;
using Mesh = Rhino.Geometry.Mesh;

namespace dotbimGH.Components
{
    public class CreateSimpleElementWithoutPropertiesGh : GH_Component
    {
        public CreateSimpleElementWithoutPropertiesGh()
            : base("Create Simple Element Without Properties", "Create Simple Element Without Properties", "Create Simple Element Without Properties", "dotbim", "Create Elements")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh object", GH_ParamAccess.item);
            pManager.AddTextParameter("Type", "Type", "Element type", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "Color", "Color for element", GH_ParamAccess.item);
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

            DA.GetData(0, ref mesh);
            DA.GetData(1, ref type);
            DA.GetData(2, ref color);

            BimElement bimElement = new BimElement(mesh, type, color);

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
            get { return new Guid("d95d2c5a-30f9-40f5-bdd9-da8200a5572a"); }
        }
    }
}