using System;

namespace dotbimGH
{
    public struct Vertex
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                if (value >= 0)
                {
                    _id = value;
                }
                else
                {
                    throw new ArgumentException("Id should be >= 0");
                }
            }
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}