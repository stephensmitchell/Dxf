// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// The contents of this file are automatically generated by a tool, and should not be directly modified.

using System;
using System.Collections.Generic;
using System.Linq;

namespace IxMilia.Dxf.Entities
{

    /// <summary>
    /// DxfLwPolyline class
    /// </summary>
    public partial class DxfLwPolyline : DxfEntity
    {
        public override DxfEntityType EntityType { get { return DxfEntityType.LwPolyline; } }
        protected override DxfAcadVersion MinVersion { get { return DxfAcadVersion.R14; } }

        public int VertexCount { get; set; }
        public int Flags { get; set; }
        public double ConstantWidth { get; set; }
        public double Thickness { get; set; }
        private List<double> _vertexCoordinateX { get; set; }
        private List<double> _vertexCoordinateY { get; set; }
        private List<int> _vertexIdentifier { get; set; }
        private List<double> _startingWidth { get; set; }
        private List<double> _endingWidth { get; set; }
        private List<double> _bulge { get; set; }
        public DxfVector ExtrusionDirection { get; set; }

        // Flags flags

        public bool IsClosed
        {
            get { return DxfHelpers.GetFlag(Flags, 1); }
            set
            {
                var flags = Flags;
                DxfHelpers.SetFlag(value, ref flags, 1);
                Flags = flags;
            }
        }

        public bool IsPLineGen
        {
            get { return DxfHelpers.GetFlag(Flags, 128); }
            set
            {
                var flags = Flags;
                DxfHelpers.SetFlag(value, ref flags, 128);
                Flags = flags;
            }
        }

        public DxfLwPolyline()
            : base()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.VertexCount = 0;
            this.Flags = 0;
            this.ConstantWidth = 0.0;
            this.Thickness = 0.0;
            this._vertexCoordinateX = new List<double>();
            this._vertexCoordinateY = new List<double>();
            this._vertexIdentifier = new List<int>();
            this._startingWidth = new List<double>();
            this._endingWidth = new List<double>();
            this._bulge = new List<double>();
            this.ExtrusionDirection = DxfVector.ZAxis;
        }

        protected override void AddValuePairs(List<DxfCodePair> pairs, DxfAcadVersion version, bool outputHandles)
        {
            base.AddValuePairs(pairs, version, outputHandles);
            pairs.Add(new DxfCodePair(100, "AcDbPolyline"));
            pairs.Add(new DxfCodePair(90, Vertices.Count));
            pairs.Add(new DxfCodePair(70, (short)(this.Flags)));
            if (this.ConstantWidth != 0.0)
            {
                pairs.Add(new DxfCodePair(43, (this.ConstantWidth)));
            }

            if (Elevation != 0.0)
            {
                pairs.Add(new DxfCodePair(38, Elevation));
            }
            if (this.Thickness != 0.0)
            {
                pairs.Add(new DxfCodePair(39, (this.Thickness)));
            }

            foreach (var item in Vertices)
            {
                pairs.Add(new DxfCodePair(10, item.Location.X));
                pairs.Add(new DxfCodePair(20, item.Location.Y));
                if (version >= DxfAcadVersion.R2013)
                {
                    pairs.Add(new DxfCodePair(91, item.Identifier));
                }
                if (item.StartingWidth != 0.0)
                {
                    pairs.Add(new DxfCodePair(40, item.StartingWidth));
                }
                if (item.EndingWidth != 0.0)
                {
                    pairs.Add(new DxfCodePair(41, item.EndingWidth));
                }
                if (item.Bulge != 0.0)
                {
                    pairs.Add(new DxfCodePair(42, item.Bulge));
                }
            }

            if (this.ExtrusionDirection != DxfVector.ZAxis)
            {
                pairs.Add(new DxfCodePair(210, ExtrusionDirection?.X ?? default(double)));
                pairs.Add(new DxfCodePair(220, ExtrusionDirection?.Y ?? default(double)));
                pairs.Add(new DxfCodePair(230, ExtrusionDirection?.Z ?? default(double)));
            }

        }

        internal override bool TrySetPair(DxfCodePair pair)
        {
            switch (pair.Code)
            {
                case 10:
                    this._vertexCoordinateX.Add((pair.DoubleValue));
                    break;
                case 20:
                    this._vertexCoordinateY.Add((pair.DoubleValue));
                    break;
                case 39:
                    this.Thickness = (pair.DoubleValue);
                    break;
                case 40:
                    this._startingWidth.Add((pair.DoubleValue));
                    break;
                case 41:
                    this._endingWidth.Add((pair.DoubleValue));
                    break;
                case 42:
                    this._bulge.Add((pair.DoubleValue));
                    break;
                case 43:
                    this.ConstantWidth = (pair.DoubleValue);
                    break;
                case 70:
                    this.Flags = (int)(pair.ShortValue);
                    break;
                case 90:
                    this.VertexCount = (pair.IntegerValue);
                    break;
                case 91:
                    this._vertexIdentifier.Add((pair.IntegerValue));
                    break;
                case 210:
                    this.ExtrusionDirection.X = pair.DoubleValue;
                    break;
                case 220:
                    this.ExtrusionDirection.Y = pair.DoubleValue;
                    break;
                case 230:
                    this.ExtrusionDirection.Z = pair.DoubleValue;
                    break;
                default:
                    return base.TrySetPair(pair);
            }

            return true;
        }
    }

}
