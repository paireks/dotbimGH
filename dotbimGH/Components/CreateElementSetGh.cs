using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace dotbimGH.Components
{
    public class CreateElementsGh : GH_Component
    {
        public CreateElementsGh()
            : base("Create Elements Set", "Create Elements Set", "Create Elements Set", "dotbim", "Create File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh object", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Insert Planes", "Insert Planes", "Planes where mesh will be inserted", GH_ParamAccess.list);
            pManager.AddTextParameter("Guids", "Guids", "Guids for elements, should match Insert Planes", GH_ParamAccess.list);
            pManager.AddTextParameter("Types", "Types", "Element types, should match Insert Planes", GH_ParamAccess.list);
            pManager.AddColourParameter("Colors", "Colors", "Colors for elements, should match Insert Planes", GH_ParamAccess.list);
            pManager.AddGenericParameter("Info", "Info", "Information about elements, should match Insert Planes", GH_ParamAccess.list);
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
            List<string> guids = new List<string>();
            List<string> types = new List<string>();
            List<Color> colors = new List<Color>();
            List<Dictionary<string, string>> info = new List<Dictionary<string, string>>();

            DA.GetData(0, ref mesh);
            DA.GetDataList(1, insertPlanes);
            DA.GetDataList(2, guids);
            DA.GetDataList(3, types);
            DA.GetDataList(4, colors);
            DA.GetDataList(5, info);

            BimElementSet bimElementSet = new BimElementSet(mesh, insertPlanes, guids, types, colors, info);
                
            DA.SetData(0, bimElementSet);
            DA.SetDataList(1, bimElementSet.PreviewMeshes);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.CreateElementsSet;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("c308d338-1e9e-4c7f-823f-5bb7249cb1dd"); }
        }
    }
}