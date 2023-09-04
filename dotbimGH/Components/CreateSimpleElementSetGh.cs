using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace dotbimGH.Components
{
    public class CreateSimpleElementSetGh : GH_Component
    {
        public CreateSimpleElementSetGh() : base("Create Simple Elements Set", "Create Simple Elements Set", "Create Simple Elements Set", "dotbim", "Create Elements")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh object", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Insert Planes", "Insert Planes", "Planes where mesh will be inserted", GH_ParamAccess.list);
            pManager.AddTextParameter("Type", "Type", "Element type, which will be applied to all these elements", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "Color", "Color for elements", GH_ParamAccess.item);
            pManager.AddGenericParameter("Info", "Info", "Information, which will be applied to all these elements", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Elements Set", "Elements Set", "Elements Set created", GH_ParamAccess.item);
            pManager.AddMeshParameter("Preview", "Preview", "Preview of elements", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            List<Plane> insertPlanes = new List<Plane>();
            string type = null;
            Color color = Color.Empty;
            Dictionary<string, string> info = null;

            DA.GetData(0, ref mesh);
            DA.GetDataList(1, insertPlanes);
            DA.GetData(2, ref type);
            DA.GetData(3, ref color);
            DA.GetData(4, ref info);

            BimElementSet bimElementSet = new BimElementSet(mesh, insertPlanes, type, color, info);
                
            DA.SetData(0, bimElementSet);
            DA.SetDataList(1, bimElementSet.PreviewMeshes);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.CreateSimpleElementsSet;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("7f610515-5f97-43ae-827e-ceec7da2d60f"); }
        }
    }
}