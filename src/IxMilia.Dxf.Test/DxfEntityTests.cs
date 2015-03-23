﻿// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using IxMilia.Dxf.Entities;
using Xunit;

namespace IxMilia.Dxf.Test
{
    public class DxfEntityTests : AbstractDxfTests
    {
        #region Private helpers

        private static DxfEntity Entity(string entityType, string data)
        {
            var file = Section("ENTITIES", string.Format(@"
999
ill-placed comment
  0
{0}
  5
<handle>
  6
<linetype-name>
  8
<layer>
 48
3.14159
 60
1
 62
1
 67
1
{1}
", entityType, data.Trim()));
            var entity = file.Entities.Single();
            Assert.Equal("<handle>", entity.Handle);
            Assert.Equal("<linetype-name>", entity.LinetypeName);
            Assert.Equal("<layer>", entity.Layer);
            Assert.Equal(3.14159, entity.LinetypeScale);
            Assert.False(entity.IsVisible);
            Assert.True(entity.IsInPaperSpace);
            Assert.Equal(DxfColor.FromIndex(1), entity.Color);
            return entity;
        }

        private static DxfEntity EmptyEntity(string entityType)
        {
            var file = Section("ENTITIES", string.Format(@"
  0
{0}", entityType));
            var entity = file.Entities.Single();
            Assert.Null(entity.Handle);
            Assert.Equal("0", entity.Layer);
            Assert.Equal("BYLAYER", entity.LinetypeName);
            Assert.Equal(1.0, entity.LinetypeScale);
            Assert.True(entity.IsVisible);
            Assert.False(entity.IsInPaperSpace);
            Assert.Equal(DxfColor.ByLayer, entity.Color);
            return entity;
        }

        private static void EnsureFileContainsEntity(DxfEntity entity, string text)
        {
            var file = new DxfFile();
            file.Entities.Add(entity);
            var stream = new MemoryStream();
            file.Save(stream);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            var actual = new StreamReader(stream).ReadToEnd();
            Assert.True(actual.Contains(text.Trim()));
        }

        #endregion

        #region Read default value tests

        [Fact]
        public void ReadDefaultLineTest()
        {
            var line = (DxfLine)EmptyEntity("LINE");
            Assert.Equal(0.0, line.P1.X);
            Assert.Equal(0.0, line.P1.Y);
            Assert.Equal(0.0, line.P1.Z);
            Assert.Equal(0.0, line.P2.X);
            Assert.Equal(0.0, line.P2.Y);
            Assert.Equal(0.0, line.P2.Z);
            Assert.Equal(0.0, line.Thickness);
            Assert.Equal(0.0, line.ExtrusionDirection.X);
            Assert.Equal(0.0, line.ExtrusionDirection.Y);
            Assert.Equal(1.0, line.ExtrusionDirection.Z);
        }

        [Fact]
        public void ReadDefaultCircleTest()
        {
            var circle = (DxfCircle)EmptyEntity("CIRCLE");
            Assert.Equal(0.0, circle.Center.X);
            Assert.Equal(0.0, circle.Center.Y);
            Assert.Equal(0.0, circle.Center.Z);
            Assert.Equal(0.0, circle.Radius);
            Assert.Equal(0.0, circle.Normal.X);
            Assert.Equal(0.0, circle.Normal.Y);
            Assert.Equal(1.0, circle.Normal.Z);
            Assert.Equal(0.0, circle.Thickness);
        }

        [Fact]
        public void ReadDefaultArcTest()
        {
            var arc = (DxfArc)EmptyEntity("ARC");
            Assert.Equal(0.0, arc.Center.X);
            Assert.Equal(0.0, arc.Center.Y);
            Assert.Equal(0.0, arc.Center.Z);
            Assert.Equal(0.0, arc.Radius);
            Assert.Equal(0.0, arc.Normal.X);
            Assert.Equal(0.0, arc.Normal.Y);
            Assert.Equal(1.0, arc.Normal.Z);
            Assert.Equal(0.0, arc.StartAngle);
            Assert.Equal(360.0, arc.EndAngle);
            Assert.Equal(0.0, arc.Thickness);
        }

        [Fact]
        public void ReadDefaultEllipseTest()
        {
            var el = (DxfEllipse)EmptyEntity("ELLIPSE");
            Assert.Equal(0.0, el.Center.X);
            Assert.Equal(0.0, el.Center.Y);
            Assert.Equal(0.0, el.Center.Z);
            Assert.Equal(1.0, el.MajorAxis.X);
            Assert.Equal(0.0, el.MajorAxis.Y);
            Assert.Equal(0.0, el.MajorAxis.Z);
            Assert.Equal(0.0, el.Normal.X);
            Assert.Equal(0.0, el.Normal.Y);
            Assert.Equal(1.0, el.Normal.Z);
            Assert.Equal(1.0, el.MinorAxisRatio);
            Assert.Equal(0.0, el.StartParameter);
            Assert.Equal(Math.PI * 2, el.EndParameter);
        }

        [Fact]
        public void ReadDefaultTextTest()
        {
            var text = (DxfText)EmptyEntity("TEXT");
            Assert.Equal(0.0, text.Location.X);
            Assert.Equal(0.0, text.Location.Y);
            Assert.Equal(0.0, text.Location.Z);
            Assert.Equal(0.0, text.Normal.X);
            Assert.Equal(0.0, text.Normal.Y);
            Assert.Equal(1.0, text.Normal.Z);
            Assert.Equal(0.0, text.Rotation);
            Assert.Equal(1.0, text.TextHeight);
            Assert.Null(text.Value);
            Assert.Equal("STANDARD", text.TextStyleName);
            Assert.Equal(0.0, text.Thickness);
            Assert.Equal(1.0, text.RelativeXScaleFactor);
            Assert.Equal(0.0, text.ObliqueAngle);
            Assert.False(text.IsTextBackward);
            Assert.False(text.IsTextUpsideDown);
            Assert.Equal(0.0, text.SecondAlignmentPoint.X);
            Assert.Equal(0.0, text.SecondAlignmentPoint.Y);
            Assert.Equal(0.0, text.SecondAlignmentPoint.Z);
            Assert.Equal(DxfHorizontalTextJustification.Left, text.HorizontalTextJustification);
            Assert.Equal(DxfVerticalTextJustification.Baseline, text.VerticalTextJustification);
        }

        [Fact]
        public void ReadDefaultVertexTest()
        {
            var vertex = (DxfVertex)EmptyEntity("VERTEX");
            Assert.Equal(0.0, vertex.Location.X);
            Assert.Equal(0.0, vertex.Location.Y);
            Assert.Equal(0.0, vertex.Location.Z);
            Assert.Equal(0.0, vertex.StartingWidth);
            Assert.Equal(0.0, vertex.EndingWidth);
            Assert.Equal(0.0, vertex.Bulge);
            Assert.False(vertex.IsExtraCreatedByCurveFit);
            Assert.False(vertex.IsCurveFitTangentDefined);
            Assert.False(vertex.IsSplineVertexCreatedBySplineFitting);
            Assert.False(vertex.IsSplineFrameControlPoint);
            Assert.False(vertex.Is3DPolylineVertex);
            Assert.False(vertex.Is3DPolygonMesh);
            Assert.False(vertex.IsPolyfaceMeshVertex);
            Assert.Equal(0.0, vertex.CurveFitTangentDirection);
            Assert.Equal(0, vertex.PolyfaceMeshVertexIndex1);
            Assert.Equal(0, vertex.PolyfaceMeshVertexIndex2);
            Assert.Equal(0, vertex.PolyfaceMeshVertexIndex3);
            Assert.Equal(0, vertex.PolyfaceMeshVertexIndex4);
        }

        [Fact]
        public void ReadDefaultSeqendTest()
        {
            var seqend = (DxfSeqend)EmptyEntity("SEQEND");
            // nothing to verify
        }

        [Fact]
        public void ReadDefaultPolylineTest()
        {
            var poly = (DxfPolyline)EmptyEntity("POLYLINE");
            Assert.Equal(0.0, poly.Elevation);
            Assert.Equal(0.0, poly.Normal.X);
            Assert.Equal(0.0, poly.Normal.Y);
            Assert.Equal(1.0, poly.Normal.Z);
            Assert.Equal(0.0, poly.Thickness);
            Assert.Equal(0.0, poly.DefaultStartingWidth);
            Assert.Equal(0.0, poly.DefaultEndingWidth);
            Assert.Equal(0, poly.PolygonMeshMVertexCount);
            Assert.Equal(0, poly.PolygonMeshNVertexCount);
            Assert.Equal(0, poly.SmoothSurfaceMDensity);
            Assert.Equal(0, poly.SmoothSurfaceNDensity);
            Assert.Equal(DxfPolylineCurvedAndSmoothSurfaceType.None, poly.SurfaceType);
            Assert.False(poly.IsClosed);
            Assert.False(poly.CurveFitVerticiesAdded);
            Assert.False(poly.SplineFitVerticiesAdded);
            Assert.False(poly.Is3DPolyline);
            Assert.False(poly.Is3DPolygonMesh);
            Assert.False(poly.IsPolygonMeshClosedInNDirection);
            Assert.False(poly.IsPolyfaceMesh);
            Assert.False(poly.IsLinetypePatternGeneratedContinuously);
        }

        [Fact]
        public void ReadDefaultSolidTest()
        {
            var solid = (DxfSolid)EmptyEntity("SOLID");
            Assert.Equal(DxfPoint.Origin, solid.FirstCorner);
            Assert.Equal(DxfPoint.Origin, solid.SecondCorner);
            Assert.Equal(DxfPoint.Origin, solid.ThirdCorner);
            Assert.Equal(DxfPoint.Origin, solid.FourthCorner);
            Assert.Equal(0.0, solid.Thickness);
            Assert.Equal(DxfVector.ZAxis, solid.ExtrusionDirection);
        }

        #endregion

        #region Read specific value tests

        [Fact]
        public void ReadDimensionTest()
        {
            var dimension = (DxfAlignedDimension)Entity("DIMENSION", @"
  1
text
 10
330.250000
 20
1310.000000
 13
330.250000
 23
1282.000000
 14
319.750000
 24
1282.000000
 70
1
");
            Assert.Equal(new DxfPoint(330.25, 1310.0, 0.0), dimension.DefinitionPoint1);
            Assert.Equal(new DxfPoint(330.25, 1282, 0.0), dimension.DefinitionPoint2);
            Assert.Equal(new DxfPoint(319.75, 1282, 0.0), dimension.DefinitionPoint3);
            Assert.Equal("text", dimension.Text);
        }

        [Fact]
        public void ReadLineTest()
        {
            var line = (DxfLine)Entity("LINE", @"
 10
1.100000E+001
 20
2.200000E+001
 30
3.300000E+001
 11
4.400000E+001
 21
5.500000E+001
 31
6.600000E+001
 39
7.700000E+001
210
8.800000E+001
220
9.900000E+001
230
1.500000E+002
");
            Assert.Equal(11.0, line.P1.X);
            Assert.Equal(22.0, line.P1.Y);
            Assert.Equal(33.0, line.P1.Z);
            Assert.Equal(44.0, line.P2.X);
            Assert.Equal(55.0, line.P2.Y);
            Assert.Equal(66.0, line.P2.Z);
            Assert.Equal(77.0, line.Thickness);
            Assert.Equal(88.0, line.ExtrusionDirection.X);
            Assert.Equal(99.0, line.ExtrusionDirection.Y);
            Assert.Equal(150.0, line.ExtrusionDirection.Z);
        }

        [Fact]
        public void ReadCircleTest()
        {
            var circle = (DxfCircle)Entity("CIRCLE", @"
 10
1.100000E+001
 20
2.200000E+001
 30
3.300000E+001
 40
4.400000E+001
 39
3.500000E+001
210
5.500000E+001
220
6.600000E+001
230
7.700000E+001
");
            Assert.Equal(11.0, circle.Center.X);
            Assert.Equal(22.0, circle.Center.Y);
            Assert.Equal(33.0, circle.Center.Z);
            Assert.Equal(44.0, circle.Radius);
            Assert.Equal(55.0, circle.Normal.X);
            Assert.Equal(66.0, circle.Normal.Y);
            Assert.Equal(77.0, circle.Normal.Z);
            Assert.Equal(35.0, circle.Thickness);
        }

        [Fact]
        public void ReadArcTest()
        {
            var arc = (DxfArc)Entity("ARC", @"
 10
1.100000E+001
 20
2.200000E+001
 30
3.300000E+001
 40
4.400000E+001
210
5.500000E+001
220
6.600000E+001
230
7.700000E+001
 50
8.800000E+001
 51
9.900000E+001
 39
3.500000E+001
");
            Assert.Equal(11.0, arc.Center.X);
            Assert.Equal(22.0, arc.Center.Y);
            Assert.Equal(33.0, arc.Center.Z);
            Assert.Equal(44.0, arc.Radius);
            Assert.Equal(55.0, arc.Normal.X);
            Assert.Equal(66.0, arc.Normal.Y);
            Assert.Equal(77.0, arc.Normal.Z);
            Assert.Equal(88.0, arc.StartAngle);
            Assert.Equal(99.0, arc.EndAngle);
            Assert.Equal(35.0, arc.Thickness);
        }

        [Fact]
        public void ReadEllipseTest()
        {
            var el = (DxfEllipse)Entity("ELLIPSE", @"
 10
1.100000E+001
 20
2.200000E+001
 30
3.300000E+001
 11
4.400000E+001
 21
5.500000E+001
 31
6.600000E+001
210
7.700000E+001
220
8.800000E+001
230
9.900000E+001
 40
1.200000E+001
 41
0.100000E+000
 42
0.400000E+000
");
            Assert.Equal(11.0, el.Center.X);
            Assert.Equal(22.0, el.Center.Y);
            Assert.Equal(33.0, el.Center.Z);
            Assert.Equal(44.0, el.MajorAxis.X);
            Assert.Equal(55.0, el.MajorAxis.Y);
            Assert.Equal(66.0, el.MajorAxis.Z);
            Assert.Equal(77.0, el.Normal.X);
            Assert.Equal(88.0, el.Normal.Y);
            Assert.Equal(99.0, el.Normal.Z);
            Assert.Equal(12.0, el.MinorAxisRatio);
            Assert.Equal(0.1, el.StartParameter);
            Assert.Equal(0.4, el.EndParameter);
        }

        [Fact]
        public void ReadTextTest()
        {
            var text = (DxfText)Entity("TEXT", @"
  1
foo bar
  7
text style name
 10
1.100000E+001
 20
2.200000E+001
 30
3.300000E+001
 39
3.900000E+001
 40
4.400000E+001
 41
4.100000E+001
 50
5.500000E+001
 51
5.100000E+001
 71
255
 72
3
 73
1
 11
9.100000E+001
 21
9.200000E+001
 31
9.300000E+001
 210
6.600000E+001
 220
7.700000E+001
 230
8.800000E+001
");
            Assert.Equal("foo bar", text.Value);
            Assert.Equal("text style name", text.TextStyleName);
            Assert.Equal(11.0, text.Location.X);
            Assert.Equal(22.0, text.Location.Y);
            Assert.Equal(33.0, text.Location.Z);
            Assert.Equal(39.0, text.Thickness);
            Assert.Equal(41.0, text.RelativeXScaleFactor);
            Assert.Equal(44.0, text.TextHeight);
            Assert.Equal(51.0, text.ObliqueAngle);
            Assert.True(text.IsTextBackward);
            Assert.True(text.IsTextUpsideDown);
            Assert.Equal(DxfHorizontalTextJustification.Aligned, text.HorizontalTextJustification);
            Assert.Equal(DxfVerticalTextJustification.Bottom, text.VerticalTextJustification);
            Assert.Equal(91.0, text.SecondAlignmentPoint.X);
            Assert.Equal(92.0, text.SecondAlignmentPoint.Y);
            Assert.Equal(93.0, text.SecondAlignmentPoint.Z);
            Assert.Equal(55.0, text.Rotation);
            Assert.Equal(66.0, text.Normal.X);
            Assert.Equal(77.0, text.Normal.Y);
            Assert.Equal(88.0, text.Normal.Z);
        }

        [Fact]
        public void ReadVertexTest()
        {
            var vertex = (DxfVertex)Entity("VERTEX", @"
 10
1.100000E+001
 20
2.200000E+001
 30
3.300000E+001
 40
4.000000E+001
 41
4.100000E+001
 42
4.200000E+001
 50
5.000000E+001
 70
255
 71
71
 72
72
 73
73
 74
74
");
            Assert.Equal(11.0, vertex.Location.X);
            Assert.Equal(22.0, vertex.Location.Y);
            Assert.Equal(33.0, vertex.Location.Z);
            Assert.Equal(40.0, vertex.StartingWidth);
            Assert.Equal(41.0, vertex.EndingWidth);
            Assert.Equal(42.0, vertex.Bulge);
            Assert.True(vertex.IsExtraCreatedByCurveFit);
            Assert.True(vertex.IsCurveFitTangentDefined);
            Assert.True(vertex.IsSplineVertexCreatedBySplineFitting);
            Assert.True(vertex.IsSplineFrameControlPoint);
            Assert.True(vertex.Is3DPolylineVertex);
            Assert.True(vertex.Is3DPolygonMesh);
            Assert.True(vertex.IsPolyfaceMeshVertex);
            Assert.Equal(50.0, vertex.CurveFitTangentDirection);
            Assert.Equal(71, vertex.PolyfaceMeshVertexIndex1);
            Assert.Equal(72, vertex.PolyfaceMeshVertexIndex2);
            Assert.Equal(73, vertex.PolyfaceMeshVertexIndex3);
            Assert.Equal(74, vertex.PolyfaceMeshVertexIndex4);
        }

        [Fact]
        public void ReadSeqendTest()
        {
            var seqend = (DxfSeqend)Entity("SEQEND", "");
            // nothing to verify
        }

        [Fact]
        public void ReadPolylineTest()
        {
            var poly = (DxfPolyline)Entity("POLYLINE", @"
 30
1.100000E+001
 39
1.800000E+001
 40
4.000000E+001
 41
4.100000E+001
 70
255
 71
71
 72
72
 73
73
 74
74
 75
6
210
2.200000E+001
220
3.300000E+001
230
4.400000E+001
  0
VERTEX
 10
1.200000E+001
 20
2.300000E+001
 30
3.400000E+001
  0
VERTEX
 10
4.500000E+001
 20
5.600000E+001
 30
6.700000E+001
  0
SEQEND
");
            Assert.Equal(11.0, poly.Elevation);
            Assert.Equal(18.0, poly.Thickness);
            Assert.Equal(40.0, poly.DefaultStartingWidth);
            Assert.Equal(41.0, poly.DefaultEndingWidth);
            Assert.Equal(71, poly.PolygonMeshMVertexCount);
            Assert.Equal(72, poly.PolygonMeshNVertexCount);
            Assert.Equal(73, poly.SmoothSurfaceMDensity);
            Assert.Equal(74, poly.SmoothSurfaceNDensity);
            Assert.Equal(DxfPolylineCurvedAndSmoothSurfaceType.CubicBSpline, poly.SurfaceType);
            Assert.True(poly.IsClosed);
            Assert.True(poly.CurveFitVerticiesAdded);
            Assert.True(poly.SplineFitVerticiesAdded);
            Assert.True(poly.Is3DPolyline);
            Assert.True(poly.Is3DPolygonMesh);
            Assert.True(poly.IsPolygonMeshClosedInNDirection);
            Assert.True(poly.IsPolyfaceMesh);
            Assert.True(poly.IsLinetypePatternGeneratedContinuously);
            Assert.Equal(22.0, poly.Normal.X);
            Assert.Equal(33.0, poly.Normal.Y);
            Assert.Equal(44.0, poly.Normal.Z);
            Assert.Equal(2, poly.Vertices.Count);
            Assert.Equal(12.0, poly.Vertices[0].Location.X);
            Assert.Equal(23.0, poly.Vertices[0].Location.Y);
            Assert.Equal(34.0, poly.Vertices[0].Location.Z);
            Assert.Equal(45.0, poly.Vertices[1].Location.X);
            Assert.Equal(56.0, poly.Vertices[1].Location.Y);
            Assert.Equal(67.0, poly.Vertices[1].Location.Z);
        }

        public void ReadSolidTest()
        {
            var solid = (DxfSolid)Entity("SOLID", @"
 10
1
 20
2
 30
3
 11
4
 21
5
 31
6
 12
7
 22
8
 32
9
 13
10
 23
11
 33
12
 39
13
210
14
220
15
230
16
");
            Assert.Equal(new DxfPoint(1, 2, 3), solid.FirstCorner);
            Assert.Equal(new DxfPoint(4, 5, 6), solid.SecondCorner);
            Assert.Equal(new DxfPoint(7, 8, 9), solid.ThirdCorner);
            Assert.Equal(new DxfPoint(10, 11, 12), solid.FourthCorner);
            Assert.Equal(13.0, solid.Thickness);
            Assert.Equal(new DxfVector(14, 15, 16), solid.ExtrusionDirection);
        }

        #endregion

        #region Write default value tests

        [Fact]
        public void WriteDefaultLineTest()
        {
            EnsureFileContainsEntity(new DxfLine(), @"
  0
LINE
  5

  8
0
100
AcDbLine
 10
0.0000000000000000E+000
 20
0.0000000000000000E+000
 30
0.0000000000000000E+000
 11
0.0000000000000000E+000
 21
0.0000000000000000E+000
 31
0.0000000000000000E+000
  0
");
        }

        [Fact]
        public void WriteDefaultCircleTest()
        {
            EnsureFileContainsEntity(new DxfCircle(), @"
  0
CIRCLE
  5

  8
0
100
AcDbCircle
 10
0.0000000000000000E+000
 20
0.0000000000000000E+000
 30
0.0000000000000000E+000
 40
0.0000000000000000E+000
  0
");
        }

        [Fact]
        public void WriteDefaultArcTest()
        {
            EnsureFileContainsEntity(new DxfArc(), @"
  0
ARC
  5

  8
0
100
AcDbCircle
 10
0.0000000000000000E+000
 20
0.0000000000000000E+000
 30
0.0000000000000000E+000
 40
0.0000000000000000E+000
100
AcDbArc
 50
0.0000000000000000E+000
 51
3.6000000000000000E+002
  0
");
        }

        [Fact]
        public void WriteDefaultEllipseTest()
        {
            EnsureFileContainsEntity(new DxfEllipse(), @"
  0
ELLIPSE
  5

  8
0
100
AcDbEllipse
 10
0.0000000000000000E+000
 20
0.0000000000000000E+000
 30
0.0000000000000000E+000
 11
1.0000000000000000E+000
 21
0.0000000000000000E+000
 31
0.0000000000000000E+000
 40
1.0000000000000000E+000
 41
0.0000000000000000E+000
 42
6.2831853071795862E+000
  0
");
        }

        [Fact]
        public void WriteDefaultTextTest()
        {
            EnsureFileContainsEntity(new DxfText(), @"
  0
TEXT
  5

  8
0
100
AcDbText
 10
0.0000000000000000E+000
 20
0.0000000000000000E+000
 30
0.0000000000000000E+000
 40
1.0000000000000000E+000
  1

 11
0.0000000000000000E+000
 21
0.0000000000000000E+000
 31
0.0000000000000000E+000
  0
");
        }

        [Fact]
        public void WriteDefaultPolylineTest()
        {
            EnsureFileContainsEntity(new DxfPolyline(), @"
  0
POLYLINE
  5

  8
0
100
AcDb2dPolyline
 10
0.0000000000000000E+000
 20
0.0000000000000000E+000
 30
0.0000000000000000E+000
  0
SEQEND
  5

  8
0
  0
");
        }

        public void WriteDefaultSolidTest()
        {
            EnsureFileContainsEntity(new DxfSolid(), @"
  0
SOLID
 62
0
100
AcDbTrace
 10
0.0000000000000000E+000
 20
0.0000000000000000E+000
 30
0.0000000000000000E+000
 11
0.0000000000000000E+000
 21
0.0000000000000000E+000
 31
0.0000000000000000E+000
 12
0.0000000000000000E+000
 22
0.0000000000000000E+000
 32
0.0000000000000000E+000
 13
0.0000000000000000E+000
 23
0.0000000000000000E+000
 33
0.0000000000000000E+000
");
        }

        #endregion

        #region Write specific value tests TODO

        [Fact]
        public void WriteLineTest()
        {
            EnsureFileContainsEntity(new DxfLine(new DxfPoint(1, 2, 3), new DxfPoint(4, 5, 6))
                {
                    Color = DxfColor.FromIndex(7),
                    Handle = "foo",
                    Layer = "bar",
                    Thickness = 7,
                    ExtrusionDirection = new DxfVector(8, 9, 10)
                }, @"
  0
LINE
  5
foo
  8
bar
 62
7
100
AcDbLine
 39
7.0000000000000000E+000
 10
1.0000000000000000E+000
 20
2.0000000000000000E+000
 30
3.0000000000000000E+000
 11
4.0000000000000000E+000
 21
5.0000000000000000E+000
 31
6.0000000000000000E+000
210
8.0000000000000000E+000
220
9.0000000000000000E+000
230
1.0000000000000000E+001
  0
");
        }

        [Fact]
        public void WriteDimensionTest()
        {
            EnsureFileContainsEntity(new DxfAlignedDimension()
            {
                Color = DxfColor.FromIndex(7),
                DefinitionPoint1 = new DxfPoint(330.25, 1310.0, 330.25),
                DefinitionPoint2 = new DxfPoint(330.25, 1282.0, 0.0),
                DefinitionPoint3 = new DxfPoint(319.75, 1282.0, 0.0),
                Handle = "foo",
                Layer = "bar",
                Text = "text"
            }, @"
  0
DIMENSION
  5
foo
  8
bar
 62
7
100
AcDbDimension
  2

 10
3.3025000000000000E+002
 20
1.3100000000000000E+003
 30
3.3025000000000000E+002
 11
0.0000000000000000E+000
 21
0.0000000000000000E+000
 31
0.0000000000000000E+000
 70
0
  1
text
  3

100
AcDbAlignedDimension
 12
0.0000000000000000E+000
 22
0.0000000000000000E+000
 32
0.0000000000000000E+000
 13
3.3025000000000000E+002
 23
1.2820000000000000E+003
 33
0.0000000000000000E+000
 14
3.1975000000000000E+002
 24
1.2820000000000000E+003
 34
0.0000000000000000E+000
");
        }

        #endregion

        #region Block tests

        public void ReadBlockTest()
        {
            var file = Parse(@"
  0
SECTION
  2
BLOCKS
  0
BLOCK
  2
block #1
 10
1
 20
2
 30
3
  0
LINE
 10
10
 20
20
 30
30
 11
11
 21
21
 31
31
  0
ENDBLK
  0
BLOCK
  2
block #2
  0
CIRCLE
 40
40
  0
ARC
 40
41
  0
ENDBLK
  0
ENDSEC
  0
EOF");

            // 2 blocks
            Assert.Equal(2, file.Blocks.Count);

            // first block
            var first = file.Blocks[0];
            Assert.Equal("block #1", first.Name);
            Assert.Equal(new DxfPoint(1, 2, 3), first.BasePoint);
            Assert.Equal(1, first.Entities.Count);
            var entity = first.Entities.First();
            Assert.Equal(DxfEntityType.Line, entity.EntityType);
            var line = (DxfLine)entity;
            Assert.Equal(new DxfPoint(10, 20, 30), line.P1);
            Assert.Equal(new DxfPoint(11, 21, 31), line.P2);

            // second block
            var second = file.Blocks[1];
            Assert.Equal("block #2", second.Name);
            Assert.Equal(2, second.Entities.Count);
            Assert.Equal(DxfEntityType.Circle, second.Entities[0].EntityType);
            Assert.Equal(40.0, ((DxfCircle)second.Entities[0]).Radius);
            Assert.Equal(DxfEntityType.Arc, second.Entities[1].EntityType);
            Assert.Equal(41.0, ((DxfArc)second.Entities[1]).Radius);
        }

        #endregion
    }
}