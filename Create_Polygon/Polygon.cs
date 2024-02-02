using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.UF;

namespace PolygonSketch
{
    public class Polygon
    {
        public static Session theSession = Session.GetSession();
        public static UFSession theUFSession = UFSession.GetUFSession();
        public static Part workPart = theSession.Parts.Work;
        public static UI theUI = UI.GetUI();

        public static void Main(string[] args)
        {
            // Calling Block UI function
            Polygon_SketchBlockUI.PolygonSketchMain();

        }

        public static int GetUnloadOption(string dummy)
        {
            return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
        }

        /// <summary>
        /// To Create Polygon Sketch
        /// </summary>
        public static void CreatePolygonSketch(Plane datumPlane, int numberofSidesOfPolygon, double lengthOfSide, double filletRadius, double extrudeLength)
        {
            // Undo Mark for the Sketch
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Sketch");

            Sketch sketch1 = null; // Declaration of Sketch            
            CreateSketch(datumPlane, out sketch1); // Calling Create Sketch function
            // calling Function CreateSidesOfPolygon
            CreateSidesOfPolygon(sketch1, numberofSidesOfPolygon, lengthOfSide, filletRadius);

            // Undo mark
            theSession.SetUndoMarkName(markId1, "Remove Created Polygon");

            // Undo Mark for the Extrude
            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Extrude");

            // Extrude length should not be zero
            if (extrudeLength > 0)
            {
                SketchExtrude(sketch1, extrudeLength); // calling Extrude Sketch function

                // Hide the Extruded Sketch
                NXOpen.DisplayableObject[] extrudedSketch = { sketch1 };
                theSession.DisplayManager.BlankObjects(extrudedSketch);
            }


            // Undo mark
            theSession.SetUndoMarkName(markId2, "Remove Extrude");

            workPart.ModelingViews.WorkView.Fit(); // Fit to Screen
        }
        /// <summary>
        ///  Coverting Angle from Degree's to Radian's
        /// </summary>
        /// <param name="DegreesAngle">Angle in Degree</param>
        /// <returns>Angle in Radian</returns>
        private static double RadianAngle(double DegreesAngle)
        {
            // Coverting Angle from Degree's to Radian's
            return DegreesAngle * (Math.PI / 180);
        }
        /// <summary>
        /// To Create the Vertices of the Polygon.
        /// </summary>
        /// <returns>Vertices of the Polygon </returns>
        private static List<NXOpen.Point3d> VerticesOfPolygon(int numberofSidesOfPolygon, double lengthOfSide, Vector3d skecthNormalVector)
        {
            // List to Store vertices of the polygon.
            List<Point3d> polygonVertices = new List<Point3d>();

            // Angle to calculate the vertices of Polygon.
            double angle = 360.0 / numberofSidesOfPolygon;

            // Calculate the radius of circumference of a Polygon (Circumscribed)
            // based on Side length of Polygon.
            double radiusOfCircle = lengthOfSide / (2 * Math.Sin(Math.PI / numberofSidesOfPolygon));

            int vertice; // Vertice counter
            double startAngle = 0; // Start angle of the Vertice of Polygon and the Incrementer of angle
            double pointX = 0, pointY = 0, pointZ = 0; // Declaration of Vertices 

            //loop to create vertice of Polygon
            for (vertice = 1; vertice <= numberofSidesOfPolygon; vertice++)
            {
                // Check for Skecth Plane is XY-Plane
                if (skecthNormalVector.Z == 1)
                {
                    // calculate the X and Y point of the Vertices of Polygon.
                    pointX = radiusOfCircle * Math.Cos(RadianAngle(startAngle));
                    pointY = radiusOfCircle * Math.Sin(RadianAngle(startAngle));
                    pointZ = 0;
                }

                // Check for Skecth Plane is YZ-Plane
                else if (skecthNormalVector.X == 1)
                {
                    // calculate the Y and Z point of the Vertices of Polygon.
                    pointX = 0;
                    pointY = radiusOfCircle * Math.Cos(RadianAngle(startAngle));
                    pointZ = radiusOfCircle * Math.Sin(RadianAngle(startAngle));
                }

                // Check for Skecth Plane is XZ-Plane
                else
                {
                    // calculate the X and Y point of the Vertices of Polygon.
                    pointX = radiusOfCircle * Math.Cos(RadianAngle(startAngle));
                    pointY = 0;
                    pointZ = radiusOfCircle * Math.Sin(RadianAngle(startAngle));
                }

                // Point 3d for Vertices of Polygon
                Point3d point = new Point3d(Math.Round(pointX, 3), Math.Round(pointY, 3), Math.Round(pointZ, 3));

                polygonVertices.Add(point); // Add Point3d to polygonVertices List

                startAngle = startAngle + angle;//  Increment the angle.
            }
            return polygonVertices; // Return the polygonVertices List
        }
        /// <summary>
        /// To Create the Sides of the Polygon.
        /// </summary>
        /// <param name="sketch">Sketch</param>
        /// <param name="numberofSidesOfPolygon">Number of Sides for Polygon</param>
        /// <param name="lengthOfSide"> Side Length of Polygon</param>
        /// <param name="filletRadius">Fillet Radius</param>
        private static void CreateSidesOfPolygon(Sketch sketch, int numberofSidesOfPolygon, double lengthOfSide, double filletRadius)
        {
            // Active Sketch created sketch.
            theSession.ActiveSketch.Activate(Sketch.ViewReorient.True);

            // To Identify the Plane of Sketch eg: XY, YZ and XZ Plane information
            DatumPlane plane = (DatumPlane)sketch.AttachPlane; // Datum Plane of Sketch
            Vector3d skecthNormalVector = plane.Normal; // Vector Normal to the sketch

            // List to Store vertices of the polygon. 
            List<Point3d> verticesOfPolygon = new List<Point3d>();
            // calling Function VerticesOfPolygon
            verticesOfPolygon = VerticesOfPolygon(numberofSidesOfPolygon, lengthOfSide, skecthNormalVector);

            // List to store the side of Polygon
            List<Line> polygonSides = new List<Line>();
            // List to store the Mid Point of sides of Polygon
            List<Point3d> lineMidPoint = new List<Point3d>();

            int vertice;// Vertice counter;

            //loop to create side of Polygon
            for (vertice = 0; vertice < numberofSidesOfPolygon; vertice++)
            {
                // Check for which side of Polygon
                if (vertice < numberofSidesOfPolygon - 1)
                {
                    // Create sides of the Polygon as Lines
                    Line line = workPart.Curves.CreateLine(verticesOfPolygon[vertice], verticesOfPolygon[vertice + 1]);

                    sketch.AddGeometry(line); // Add side of Ploygon to the sketch

                    polygonSides.Add(line); // Add side of Ploygon to the polygonSides List

                    // Add side of Ploygon to the lineMidPoint List by caling the function "MidPointOfLine"
                    lineMidPoint.Add(MidPointOfLine(line));
                }

                // To create last side of Polygon
                else
                {
                    // Create last sides of the Polygon as Lines
                    Line endLine = workPart.Curves.CreateLine(verticesOfPolygon.ElementAt(vertice), verticesOfPolygon.ElementAt(0));

                    sketch.AddGeometry(endLine); // Add side of Ploygon to the sketch

                    polygonSides.Add(endLine); // Add side of Ploygon to the polygonSides List

                    // Add side of Ploygon to the lineMidPoint List by caling the function "MidPointOfLine"
                    lineMidPoint.Add(MidPointOfLine(endLine));
                }
            }

            Line line1, line2; // lines to create fillet
            Point3d helpPoint1, helpPoint2; // point to create fillet

            // Undo Mark for the Fillet
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Fillets");

            // check for Fillet radius not equal to Zero
            if (filletRadius > 0)
            {
                //loop all side of Polygon to Create a Fillets
                for (vertice = 0; vertice < numberofSidesOfPolygon; vertice++)
                {
                    // Check for which side of Polygon
                    if (vertice < numberofSidesOfPolygon - 1)
                    {
                        // first and second lines to create fillet in between them
                        // side from the polygonSides List
                        line1 = polygonSides.ElementAt(vertice);
                        line2 = polygonSides.ElementAt(vertice + 1);

                        // first and second lines mid point to create fillet in between them
                        // points from lineMidPoint List
                        helpPoint1 = lineMidPoint.ElementAt(vertice);
                        helpPoint2 = lineMidPoint.ElementAt(vertice + 1);

                        // calling Function to create fillet
                        CreateFilletSketch(line1, line2, helpPoint1, helpPoint2, filletRadius);

                    }
                    else
                    {
                        // first and second lines to create fillet in between them
                        // side from the polygonSides List
                        line1 = polygonSides.ElementAt(vertice);
                        line2 = polygonSides.ElementAt(0);

                        // first and second lines mid point to create fillet in between them
                        // points from lineMidPoint List
                        helpPoint1 = lineMidPoint.ElementAt(vertice);
                        helpPoint2 = lineMidPoint.ElementAt(0);

                        // calling Function to create fillet
                        CreateFilletSketch(line1, line2, helpPoint1, helpPoint2, filletRadius);
                    }
                }
            }

            // Undo mark
            theSession.SetUndoMarkName(markId1, "Remove Created Fillets");

            theSession.ActiveSketch.Update(); // Update the sketch
            // Deactivate the sketch
            sketch.Deactivate(Sketch.ViewReorient.False, Sketch.UpdateLevel.Model);

            workPart.ModelingViews.WorkView.Fit(); // Fit to Screen
        }
        /// <summary>
        /// To Create Sketch on Selected Plane using Sketch InPlace Builder
        /// </summary>
        /// <param name="datumPlane">Pass the Plane on which Sketch to Create</param>
        /// <param name="sketch">Skecth is create in given Plane</param>
        /// <returns></returns>
        public static bool CreateSketch(Plane datumPlane, out Sketch sketch)
        {
            sketch = null;
            try
            {
                // Declaration Sketch In Place Builder
                NXOpen.SketchInPlaceBuilder sketchBuilder1 = workPart.Sketches.CreateSketchInPlaceBuilder2(null);

                // Declaration of Direction Vector to rotate the Sketch
                Vector3d directionVector = new Vector3d();
                if (datumPlane.Normal.Z == 1) // Check for XY Datum Palne
                {
                    directionVector = new Vector3d(1, 0, 0); // Assign X Direction Vector to Rotate the Sketch
                }
                else if (datumPlane.Normal.X == 1) // Check for YZ Datum Palne
                {
                    directionVector = new Vector3d(0, 1, 0); // Assign Y Direction Vector to Rotate the Sketch
                }
                else // Check for XZ Datum Palne
                {
                    directionVector = new Vector3d(0, 0, 1);  // Assign Z Direction Vector to Rotate the Sketch
                }

                // Declarartion of Normal Vector to Sketching Plane
                Vector3d normalVector = datumPlane.Normal;

                // Declaration and Create the Origin point for Sketch
                NXOpen.Point originPoint = workPart.Points.CreatePoint(datumPlane.Origin);

                sketchBuilder1.SketchOrigin = originPoint; // Assign Origin Point to Sketch Builder

                // Create the Sketch Reference Palne and Assign to Sketch Builder
                sketchBuilder1.PlaneReference = workPart.Planes.CreatePlane(datumPlane.Origin, normalVector, SmartObject.UpdateOption.Mixed);

                // Create the Sketch Reference Axis and Assign to Sketch Builder
                sketchBuilder1.AxisReference = workPart.Directions.CreateDirection(originPoint, directionVector);

                // Assign Inferred Option to Sketch Builder
                sketchBuilder1.PlaneOption = NXOpen.Sketch.PlaneOption.Inferred;

                // Commit the Sketch Builder
                sketch = (NXOpen.Sketch)sketchBuilder1.Commit();

                sketchBuilder1.Destroy(); // Destroy the Sketch Builder

                //Active the Sketch 
                sketch.Activate(Sketch.ViewReorient.True);

                return true;
            }
            catch (Exception ex)
            {
                theUI.NXMessageBox.Show("Exception", NXMessageBox.DialogType.Error, ex.ToString());
                return false;
                throw;
            }
        }
        /// <summary>
        /// To find the Mid point of the given Line
        /// </summary>
        /// <param name="line">Line</param>
        /// <returns>Mid Point of given Line</returns>
        private static NXOpen.Point3d MidPointOfLine(Line line)
        {
            NXOpen.Point3d midPointOfLine = new Point3d(); // Declaration of midPointOfLine 
            Point3d lineStartPoint = line.StartPoint;  // Declaration of lineStartPoint 
            Point3d lineEndPoint = line.EndPoint;  // Declaration of lineEndPoint 

            // Assign the X,Y,Z value of the midPointOfLine from a given line
            midPointOfLine.X = (lineStartPoint.X + lineEndPoint.X) / 2;
            midPointOfLine.Y = (lineStartPoint.Y + lineEndPoint.Y) / 2;
            midPointOfLine.Z = (lineStartPoint.Z + lineEndPoint.Z) / 2;

            return midPointOfLine; //return midPointOfLine
        }
        /// <summary>
        /// To Create the Fillet in sketch
        /// </summary>
        /// <param name="line1">Line 1</param>
        /// <param name="line2">Line 2</param>
        /// <param name="helpPoint1">Point 1 </param>
        /// <param name="helpPoint2">Point 2</param>
        /// <param name="radius">Radius Fillet </param>
        /// <returns></returns>
        private static bool CreateFilletSketch(Line line1, Line line2, Point3d helpPoint1, Point3d helpPoint2, double radius)
        {
            try
            {
                NXOpen.Arc[] fillets; // Array of fillets
                NXOpen.SketchConstraint[] constraints; // Array of constraints
                fillets = theSession.ActiveSketch.Fillet(line1, line2, helpPoint1, helpPoint2, radius, NXOpen.Sketch.TrimInputOption.True, NXOpen.Sketch.CreateDimensionOption.False, NXOpen.Sketch.AlternateSolutionOption.False, out constraints);

                return true;
            }
            catch (Exception ex)
            {
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
                return false;
                throw;
            }
        }
        /// <summary>
        /// Extrude the Sketch to given Length
        /// </summary>
        /// <param name="sketch">Sketch to Extrude</param>
        /// <param name="length">Length of Extrude</param>
        /// <returns></returns>
        public static bool SketchExtrude(Sketch sketch, double length)
        {
            try
            {
                NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
                NXOpen.Features.ExtrudeBuilder extrudeBuilder1;
                extrudeBuilder1 = workPart.Features.CreateExtrudeBuilder(nullNXOpen_Features_Feature);

                NXOpen.Section section1;
                section1 = workPart.Sections.CreateSection(0.009, 0.01, 0.5);

                extrudeBuilder1.Section = section1;

                extrudeBuilder1.AllowSelfIntersectingSection(true);

                extrudeBuilder1.DistanceTolerance = 0.01;

                extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                NXOpen.Body[] targetBodies1 = new NXOpen.Body[1];
                NXOpen.Body nullNXOpen_Body = null;
                targetBodies1[0] = nullNXOpen_Body;
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies1);

                extrudeBuilder1.Limits.StartExtend.Value.RightHandSide = "0";

                extrudeBuilder1.Limits.EndExtend.Value.RightHandSide = length.ToString(); // Extrude Length Value

                NXOpen.GeometricUtilities.SmartVolumeProfileBuilder smartVolumeProfileBuilder1;
                smartVolumeProfileBuilder1 = extrudeBuilder1.SmartVolumeProfile;

                smartVolumeProfileBuilder1.OpenProfileSmartVolumeOption = false;

                smartVolumeProfileBuilder1.CloseProfileRule = NXOpen.GeometricUtilities.SmartVolumeProfileBuilder.CloseProfileRuleType.Fci;

                section1.DistanceTolerance = 0.01;

                section1.ChainingTolerance = 0.0094999999999999998;

                section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

                int sketchCurvesCount = 0;
                List<Curve> sketchCurves = new List<Curve>();
                foreach (NXObject curves in sketch.GetAllGeometry())
                {
                    sketchCurvesCount++;
                    Curve curveInSkecth = (Curve)curves;
                    sketchCurves.Add((Curve)curves);
                }
                NXOpen.ICurve[] curves1 = sketchCurves.ToArray(); //new NXOpen.ICurve[sketchCurvesCount];

                NXOpen.Point3d seedPoint1 = new NXOpen.Point3d(0, 0, 0.0);
                NXOpen.RegionBoundaryRule regionBoundaryRule1;
                regionBoundaryRule1 = workPart.ScRuleFactory.CreateRuleRegionBoundary(sketch, curves1, seedPoint1, 0.01);

                section1.AllowSelfIntersection(true);

                NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
                rules1[0] = regionBoundaryRule1;
                NXOpen.NXObject nullNXOpen_NXObject = null;
                NXOpen.Point3d helpPoint1 = new NXOpen.Point3d(0.0, 0.0, 0.0);
                section1.AddToSection(rules1, nullNXOpen_NXObject, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

                NXOpen.Direction direction1;
                direction1 = workPart.Directions.CreateDirection(sketch, NXOpen.Sense.Forward, NXOpen.SmartObject.UpdateOption.WithinModeling);

                extrudeBuilder1.Direction = direction1;

                extrudeBuilder1.ParentFeatureInternal = false;

                NXOpen.Features.Feature feature1;
                feature1 = extrudeBuilder1.CommitFeature();

                extrudeBuilder1.Destroy();

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
