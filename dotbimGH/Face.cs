using System;

namespace dotbimGH
{
    public struct Face
    {
        private int _id1;
        private int _id2;
        private int _id3;
        public int Id1
        {
            get => _id1;
            set
            {
                if (value >= 0)
                {
                    _id1 = value;
                }
                else
                {
                    throw new ArgumentException("Id1 should be >= 0");
                }
            }
        }
        public int Id2
        {
            get => _id2;
            set
            {
                if (value >= 0)
                {
                    _id2 = value;
                }
                else
                {
                    throw new ArgumentException("Id2 should be >= 0");
                }
            }
        }
        public int Id3
        {
            get => _id3;
            set
            {
                if (value >= 0)
                {
                    _id3 = value;
                }
                else
                {
                    throw new ArgumentException("Id3 should be >= 0");
                }
            }
        }
    }
}