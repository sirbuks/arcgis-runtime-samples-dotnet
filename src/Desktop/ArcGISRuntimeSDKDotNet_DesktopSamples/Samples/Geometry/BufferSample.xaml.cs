﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArcGISRuntimeSDKDotNet_DesktopSamples.Samples
{
    /// <summary>
    /// This sample demonstrates use of the GeometryEngine to calculate a buffer. To use the sample, click a point on the map. The click point and a buffer of 5 miles around the point will be shown.
    /// </summary>
    /// <title>Buffer</title>
	/// <category>Geometry</category>
	public partial class BufferSample : UserControl
    {
        private const double MILES_TO_METERS = 1609.34;

        private PictureMarkerSymbol _pinSymbol;
        private SimpleFillSymbol _bufferSymbol;

        /// <summary>Construct Buffer sample control</summary>
        public BufferSample()
        {
            InitializeComponent();

            MyMapView.Map.InitialViewpoint = new Envelope(
				-10863035.970, 3838021.340, -10744801.344, 3887145.299, 
				SpatialReferences.WebMercator);
            var _ = SetupSymbolsAsync();
        }

        private void MyMapView_MapViewTapped(object sender, MapViewInputEventArgs e)
        {
            try
            {
				graphicOverlay.Graphics.Clear();

                // Convert screen point to map point
                var point = e.Location;
                var buffer = GeometryEngine.Buffer(point, 5 * MILES_TO_METERS);

                var bufferGraphic = new Graphic { Geometry = buffer, Symbol = _bufferSymbol };
				graphicOverlay.Graphics.Add(bufferGraphic);

				var pointGraphic = new Graphic { Geometry = point, Symbol = _pinSymbol };
				graphicOverlay.Graphics.Add(pointGraphic);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Geometry Engine Failed!");
            }
        }

        private async Task SetupSymbolsAsync()
        {
            _pinSymbol = new PictureMarkerSymbol() { Width = 48, Height = 48, YOffset = 24 };
            await _pinSymbol.SetSourceAsync(
				new Uri("pack://application:,,,/ArcGISRuntimeSDKDotNet_DesktopSamples;component/Assets/RedStickpin.png"));

            _bufferSymbol = layoutGrid.Resources["BufferSymbol"] as SimpleFillSymbol;
        }
    }
}
