using System;
using dotbim;
using Grasshopper.Kernel;

namespace dotbimGH.Components
{
    public class DeconstructBimElementGh : GH_Component
    {
        public DeconstructBimElementGh()
            : base("Deconstruct Element", "Deconstruct Element", "Deconstructs Element", "dotbim", "Read File")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Element", "Element", "Element read", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Guid", "Guid", "Guid deconstructed", GH_ParamAccess.item);
            pManager.AddTextParameter("Type", "Type", "Element type deconstructed", GH_ParamAccess.item);
            pManager.AddGenericParameter("Info", "Info", "Info deconstructed about element", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Element bimElement = null;
            DA.GetData(0, ref bimElement);

            DA.SetData(0, bimElement.Guid);
            DA.SetData(1, bimElement.Type);
            DA.SetData(2, bimElement.Info);
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
            get { return new Guid("2566a95e-70be-4d77-9491-ed4d5eede973"); }
        }
    }

}