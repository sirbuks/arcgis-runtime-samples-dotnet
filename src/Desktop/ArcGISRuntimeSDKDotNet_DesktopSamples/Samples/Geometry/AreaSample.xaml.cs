﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcGISRuntimeSDKDotNet_DesktopSamples.Samples
{
    /// <summary>
    /// Demonstrates how to calculate the area and perimiter of a polygon using the GeometryEngine.
    /// </summary>
    /// <title>Area</title>
	/// <category>Geometry</category>
	public partial class AreaSample : UserControl
    {
        private const double toMilesConversion = 0.0006213700922;
        private const double toSqMilesConversion = 0.0000003861003;

        /// <summary>Construct Area sample control</summary>
        public AreaSample()
        {
            InitializeComponent();

			MyMapView.Map.InitialViewpoint = new Envelope(-130, 20, -65, 55, SpatialReferences.Wgs84);

			MyMapView.ExtentChanged += MyMapView_ExtentChanged; 
        }

		private async void MyMapView_ExtentChanged(object sender, System.EventArgs e)
		{
			MyMapView.ExtentChanged -= MyMapView_ExtentChanged;
			await DoCalculateAreaAndLengthAsync();
		}

        private async Task DoCalculateAreaAndLengthAsync()
        {
            try
            {
                // Wait for user to draw
                var geom = await MyMapView.Editor.RequestShapeAsync(DrawShape.Polygon);

                // show geometry on map
				AreaOverlay.Graphics.Clear();

                var graphic = new Graphic { Geometry = geom, Symbol = LayoutRoot.Resources["DefaultFillSymbol"] as Symbol };
				AreaOverlay.Graphics.Add(graphic);

                // Calculate results
                var areaPlanar = GeometryEngine.Area(geom);
                ResultsAreaPlanar.Text = string.Format("{0} sq. miles", (areaPlanar * toSqMilesConversion).ToString("n3"));

                var perimPlanar = GeometryEngine.Length(geom);
                ResultsPerimeterPlanar.Text = string.Format("{0} miles", (perimPlanar * toMilesConversion).ToString("n3"));

                var areaGeodesic = GeometryEngine.GeodesicArea(geom);
                ResultsAreaGeodesic.Text = string.Format("{0} sq. miles", (areaGeodesic * toSqMilesConversion).ToString("n3"));

                var perimGeodesic = GeometryEngine.GeodesicLength(geom);
                ResultsPerimeterGeodesic.Text = string.Format("{0} miles", (perimGeodesic * toMilesConversion).ToString("n3"));

                Instructions.Visibility = Visibility.Collapsed;
                Results.Visibility = Visibility.Visible;
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show("Current sketch has been canceled.", "Task Canceled!");
            }
        }

        private async void CancelCurrent_Click(object sender, RoutedEventArgs e)
        {
            MyMapView.Editor.Cancel.Execute(null);
            ResetUI();
			await DoCalculateAreaAndLengthAsync();
        }

        private async void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
			await DoCalculateAreaAndLengthAsync();
        }

        private void ResetUI()
        {
			AreaOverlay.Graphics.Clear();
            Instructions.Visibility = Visibility.Visible;
            Results.Visibility = Visibility.Collapsed;
        }
    }
}
