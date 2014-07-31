﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcGISRuntimeSDKDotNet_DesktopSamples.Samples
{
    /// <summary>
    /// This sample demonstrates use of the GeometryEngine.GeodesicMove method to move a geometry be a specified distance. To use the sample, first digitize a polygon on the map. Then set the move distance and angle properties and click the 'Geodesic Move' button. The original polygon and the new moved polygon will be displayed.
    /// </summary>
    /// <title>Geodesic Move</title>
	/// <category>Geometry</category>
	public partial class GeodesicMove : UserControl
    {
        private Symbol _origSymbol;

        /// <summary>Construct Geodesic Move sample control</summary>
        public GeodesicMove()
        {
            InitializeComponent();

            _origSymbol = layoutGrid.Resources["OriginalSymbol"] as Symbol;

            MyMapView.ExtentChanged += MyMapView_ExtentChanged;
        }

        // Start map interaction once the mapview extent is set
        private async void MyMapView_ExtentChanged(object sender, EventArgs e)
        {
            MyMapView.ExtentChanged -= MyMapView_ExtentChanged;

            MyMapView.Editor.EditorConfiguration.MidVertexSymbol = null;
            MyMapView.Editor.EditorConfiguration.VertexSymbol = null;
            MyMapView.Editor.EditorConfiguration.SelectedVertexSymbol = new SimpleMarkerSymbol() 
			{ 
				Color = System.Windows.Media.Colors.Blue, 
				Size = 6 
			};

            await AcceptUserPolygon();
        }

        // Digitize a new original polygon to move
        private async void DigitizeButton_Click(object sender, RoutedEventArgs e)
        {
            await AcceptUserPolygon();
        }

        // Get the polygon from the user
        private async Task AcceptUserPolygon()
        {
            try
            {
                movedOverlay.Graphics.Clear();
				originalOverlay.Graphics.Clear();

                var polygon = await MyMapView.Editor.RequestShapeAsync(DrawShape.Polygon, _origSymbol);

				originalOverlay.Graphics.Add(new Graphic(polygon));
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Geodesic Move Sample");
            }
        }

        // Move the polygon the specified distance and angle
        private void GeodesicMoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (originalOverlay.Graphics.Count == 0)
                    throw new ApplicationException("Digitize a polygon to move.");

				var coords = originalOverlay.Graphics[0].Geometry as Multipart;
                if (coords == null)
                    throw new ApplicationException("Digitize a polygon to move.");

                var points = coords.Parts.First();
                var distance = (double)comboDistance.SelectedItem;
                var azimuth = (double)sliderAngle.Value;
                var movedPoints = GeometryEngine.GeodesicMove(points, distance, LinearUnits.Miles, azimuth);

                Polygon movedPoly = new Polygon(movedPoints, MyMapView.SpatialReference);
				movedOverlay.Graphics.Clear();
				movedOverlay.Graphics.Add(new Graphic(movedPoly));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Geodesic Move Sample");
            }
        }
    }
}
