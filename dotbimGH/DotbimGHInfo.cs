using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace dotbimGH
{
    public class DotbimGhInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "dotbimGH";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                return null;
            }
        }
        public override string Description
        {
            get
            {
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("d111a1b1-a77b-413c-a995-fa6be23fc05b");
            }
        }

        public override string AuthorName
        {
            get
            {
                return "Wojciech Radaczyński";
            }
        }
        public override string AuthorContact
        {
            get
            {
                return "w.radaczynski@gmail.com";
            }
        }

        public override string Version
        {
            get
            {
                return "1.0.0";
            }
        }
    }
}