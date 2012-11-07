﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleContent.Resources;
using Microsoft.Phone.Maps.Controls;
using System.Diagnostics;
using System.Device.Location;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace SimpleContent
{
    public partial class MainPage : PhoneApplicationPage
    {
        Popup PopupP;
        String selected_shape = "";
        MapPolygon poly = null;
        MapPolyline polyline = null;
        MapLayer markerLayer = null;
        float _count = 0;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Debug.WriteLine("We are started"); //Check to see if the "Redirect all Output Window text to the Immediate Window" is checked under Tools -> Options -> Debugging -> General.  

            AddSelectionPopUp();

            PopupP = new Popup();

            map1.MouseLeftButtonDown += map1_MouseLeftButtonDown;

        }

        void map1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IList<MapElement> listt = map1.GetMapElementsAt(e.GetPosition(map1));
            Debug.WriteLine("map1_MouseLeftButtonDown, MapElement count: " + listt.Count());
        }

        private void AddSelectionPopUp()
        {
            Popup SelectionPopupP = new Popup();


            StackPanel horpanel = new StackPanel();
            horpanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            horpanel.Background = new SolidColorBrush(Colors.Black);
            horpanel.Opacity = 0.7;

            Button buttonMarker = new Button();
            buttonMarker.Content = "Marker";
            buttonMarker.Margin = new Thickness(5.0);
            buttonMarker.Click += new RoutedEventHandler(Button_Markers_Click);

            Button buttonPolygon = new Button();
            buttonPolygon.Content = "Polygon";
            buttonPolygon.Margin = new Thickness(5.0);
            buttonPolygon.Click += new RoutedEventHandler(Button_Polygon_Click);

            Button buttonPolyline = new Button();
            buttonPolyline.Content = "Polyline";
            buttonPolyline.Margin = new Thickness(5.0);
            buttonPolyline.Click += new RoutedEventHandler(Button_Polyline_Click);

            horpanel.Children.Add(buttonMarker);
            horpanel.Children.Add(buttonPolygon);
            horpanel.Children.Add(buttonPolyline);
            SelectionPopupP.Child = horpanel;


            // Set where the popup will show up on the screen.
            SelectionPopupP.VerticalOffset = 30;
            SelectionPopupP.HorizontalOffset = 20;

            // Open the popup.
            SelectionPopupP.IsOpen = true;

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("We are loaded");
        }

        private void Button_Markers_Click(object sender, RoutedEventArgs e)
        {
            selected_shape = "Markers";
            OpenPopUpSelection();
        }

        private void Button_Polygon_Click(object sender, RoutedEventArgs e)
        {
            selected_shape = "Polygon";
            OpenPopUpSelection();
        }

        private void Button_Polyline_Click(object sender, RoutedEventArgs e)
        {
            selected_shape = "Polyline";
            OpenPopUpSelection();
        }

        private void OpenPopUpSelection()
        {
            // Create some content to show in the popup. Typically you would 
            // create a user control.
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(5.0);


            StackPanel panel1 = new StackPanel();
            panel1.Background = new SolidColorBrush(Colors.Black);
            panel1.Opacity = 0.7;

            TextBlock textBox1 = new TextBlock();

            textBox1.Text = selected_shape;
            textBox1.FontSize = 60;
            textBox1.Width = 250;
            textBox1.Foreground = new SolidColorBrush(Colors.Blue);
            textBox1.Opacity = 1;

            ListBox listBox1 = new ListBox();

            listBox1.FontSize = 35;
            listBox1.Foreground = new SolidColorBrush(Colors.White);
            listBox1.Opacity = 1;

            listBox1.Items.Add("Add");
            listBox1.Items.Add("Remove");
            listBox1.Items.Add("Change Z order");
            //       listBox1.Items.Add("Sent to back");
            listBox1.Items.Add("Toggle visibility");
            listBox1.Items.Add("Fit to View");

            listBox1.SelectionChanged += SelectionChangedEventHandler;

            Button button1 = new Button();
            button1.Content = "Close";
            button1.Margin = new Thickness(5.0);
            button1.Foreground = new SolidColorBrush(Colors.White);
            button1.Opacity = 1;
            button1.Click += new RoutedEventHandler(Cancel_Click);

            panel1.Children.Add(textBox1);
            panel1.Children.Add(listBox1);
            panel1.Children.Add(button1);
            border.Child = panel1;

            // Set the Child property of Popup to the border 
            // which contains a stackpanel, textblock and button.
            PopupP.Child = border;

            // Set where the popup will show up on the screen.
            PopupP.VerticalOffset = 100;
            PopupP.HorizontalOffset = 50;

            // Open the popup.
            PopupP.IsOpen = true;

        }

        void SelectionChangedEventHandler(Object sender, SelectionChangedEventArgs e)
        {

            String sellected = (sender as ListBox).SelectedItem.ToString();

            Debug.WriteLine("ListBox1SelectedIndexChanged " + sellected);

            if (sellected == "Add")
            {
                AddItem();
            }
            else if (sellected == "Remove")
            {
                RevomeItem();
            }
            else if (sellected == "Change Z order")
            {
                ChangeZOrder();
            }
            else if (sellected == "Sent to back")
            {
                SentToBack();
            }
            else if (sellected == "Toggle visibility")
            {
                ToggleVisibility();
            }
            else if (sellected == "Fit to View")
            {
                FitToView();
            }
        }


        void AddItem()
        {
            if (selected_shape == "Polygon")
            {
                if (poly == null)
                {
                    poly = new MapPolygon();

                    //Define the polygon vertices
                    GeoCoordinateCollection boundingLocations = new GeoCoordinateCollection();
                    boundingLocations.Add(new GeoCoordinate(60.22, 24.81));
                    boundingLocations.Add(new GeoCoordinate(60.30, 24.70));
                    boundingLocations.Add(new GeoCoordinate(60.14, 24.57));

                    //Set the polygon properties
                    poly.Path = boundingLocations;
                    poly.FillColor = Color.FromArgb(0x55, 0x00, 0xFF, 0x00);
                    poly.StrokeColor = Color.FromArgb(0xFF, 0x00, 0x00, 0xFF);
                    poly.StrokeThickness = 20;


                    //Add the polygon to the map
                    map1.MapElements.Add(poly);
                }
            }
            else if (selected_shape == "Polyline")
            {
                if (polyline == null)
                {
                    polyline = new MapPolyline();
                    polyline.StrokeColor = Color.FromArgb(0xFF, 0xFF, 0x00, 0x00);
                    polyline.StrokeThickness = 10;

                    polyline.Path = new GeoCoordinateCollection() { 
                            new GeoCoordinate(60.27, 24.81), 
                            new GeoCoordinate(60.35, 24.70), 
                            new GeoCoordinate(60.19, 24.57), 
                            new GeoCoordinate(60.27, 24.81) 
                        };

                    
                    map1.MapElements.Add(polyline);
                }
            }
            else if (selected_shape == "Markers")
            {
                //       if (markerLayer == null)
                {
                    BitmapImage MarkerBitmap = new BitmapImage(new Uri("animals-cat.png", UriKind.Relative));

                    map1.Layers.Add(CreateNewMarker(MarkerBitmap, new GeoCoordinate(60.27, 24.70)));
                    map1.Layers.Add(CreateNewMarker(MarkerBitmap, new GeoCoordinate(60.27, 24.75)));
                    map1.Layers.Add(CreateNewMarker(MarkerBitmap, new GeoCoordinate(60.20, 24.75)));

                }
            }
        }

        MapLayer CreateNewMarker(ImageSource bitmap, GeoCoordinate coords, double Height = 500, double Width = 500)
        {

            Image markerImage = new Image();
            markerImage.Source = bitmap;
            markerImage.Visibility = System.Windows.Visibility.Visible;
            markerImage.Height = Height;
            markerImage.Width = Width;

            MapOverlay pin = new MapOverlay();
            pin.GeoCoordinate = coords;
            pin.Content = markerImage;

            MapLayer markerLayer = new MapLayer();
            markerLayer.Add(pin);
            return markerLayer;

        }


        void FitToView()
        {

            bool gotRect = false;
            double north = 0;
            double west = 0;
            double south = 0;
            double east = 0;

            if (selected_shape == "Polygon" && (poly != null))
            {
                gotRect = true;

                north = south = poly.Path[0].Latitude;
                west = east = poly.Path[0].Longitude;

                foreach (var p in poly.Path.Skip(1))
                {
                    if (north < p.Latitude) north = p.Latitude;
                    if (west > p.Longitude) west = p.Longitude;
                    if (south > p.Latitude) south = p.Latitude;
                    if (east < p.Longitude) east = p.Longitude;
                }
            }
            else if (selected_shape == "Polyline" && (polyline != null))
            {
                gotRect = true;

                north = south = polyline.Path[0].Latitude;
                west = east = polyline.Path[0].Longitude;

                foreach (var p in polyline.Path.Skip(1))
                {
                    if (north < p.Latitude) north = p.Latitude;
                    if (west > p.Longitude) west = p.Longitude;
                    if (south > p.Latitude) south = p.Latitude;
                    if (east < p.Longitude) east = p.Longitude;
                }
            }
            else if (selected_shape == "Markers" && (markerLayer != null))
            {
                for (var p = 0; p < markerLayer.Count(); p++)
                {
                    MapOverlay MarkerElement = markerLayer[p];

                    if (!gotRect)
                    {
                        gotRect = true;
                        north = south = MarkerElement.GeoCoordinate.Latitude;
                        west = east = MarkerElement.GeoCoordinate.Longitude;
                    }
                    else
                    {
                        if (north < MarkerElement.GeoCoordinate.Latitude) north = MarkerElement.GeoCoordinate.Latitude;
                        if (west > MarkerElement.GeoCoordinate.Longitude) west = MarkerElement.GeoCoordinate.Longitude;
                        if (south > MarkerElement.GeoCoordinate.Latitude) south = MarkerElement.GeoCoordinate.Latitude;
                        if (east < MarkerElement.GeoCoordinate.Longitude) east = MarkerElement.GeoCoordinate.Longitude;
                    }
                }
            }

            if (gotRect)
            {
                map1.SetView(new LocationRectangle(north, west, south, east));
            }
        }

        void RevomeItem()
        {
            if (selected_shape == "Polygon")
            {
                if (poly != null)
                {
                    map1.MapElements.Remove(poly);
                    poly = null;
                }
            }
            else if (selected_shape == "Polyline")
            {
                if (polyline != null)
                {
                    map1.MapElements.Remove(polyline);
                    polyline = null;
                }
            }
            else if (selected_shape == "Markers")
            {
                if (markerLayer != null)
                {
                    map1.Layers.Remove(markerLayer);
                    markerLayer = null;
                }
            }
        }

        void ChangeZOrder()
        {
            // Send to the back the topmost marker
            Collection<MapLayer> markersCollection = map1.Layers;
            MapLayer topLayer = markersCollection[0]; // The topmost marker
            markersCollection.RemoveAt(0);            // Remove it from top

            // Create a copy and place that copy at the bottom. 
            MapOverlay overlay = topLayer[0]; // Assume there is only one overlay in collection
            Image markerImage = overlay.Content as Image;
            MapLayer newLayer = CreateNewMarker(markerImage.Source, overlay.GeoCoordinate,
                 markerImage.Height, markerImage.Width); 

            // Add the marker to the bottom of the stack
            map1.Layers.Add(newLayer);


        }

        void ToggleVisibility()
        {
            foreach (MapLayer layer in map1.Layers)
            {
                foreach (MapOverlay marker in layer)
                {
                    Image markerImage = marker.Content as Image;

                    if (markerImage.Visibility == System.Windows.Visibility.Collapsed)
                        markerImage.Visibility = System.Windows.Visibility.Visible;
                    else

                        markerImage.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            //  markersCollection.Clear();

            //    var ccc = aaa.GetValue(Canvas.ZIndexProperty);
            /*            if (selected_shape == "Polygon")
                        {
                            if (poly != null)
                            {
                                if (poly .Visibility == System.Windows.Visibility.Visible)
                                {
                                    Debug.WriteLine("Set polygon Visibility off ");
                                    poly.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                else
                                {
                                    Debug.WriteLine("Set polygon Visibility on ");
                                    poly.Visibility = System.Windows.Visibility.Visible;
                                }
                            }
                        }
                        else if (selected_shape == "Polyline")
                        {
                            if (polyline != null)
                            {
                                if (polyline.Visibility == System.Windows.Visibility.Visible)
                                {
                                    Debug.WriteLine("Set polyline Visibility off ");
                                    polyline.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                else
                                {
                                    Debug.WriteLine("Set polyline Visibility on ");
                                    polyline.Visibility = System.Windows.Visibility.Visible;
                                }
                            }
                        }
                        else if (selected_shape == "Markers")
                        {
                            if (markerLayer != null)
                            {
                                if (markerLayer.Visibility == System.Windows.Visibility.Visible)
                                {
                                    Debug.WriteLine("Set marker Visibility off ");
                                    markerLayer.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                else
                                {
                                    Debug.WriteLine("Set marker Visibility on ");
                                    markerLayer.Visibility = System.Windows.Visibility.Visible;
                                }
                            }
                        }*/
        }

        void SentToBack()
        {
            /*          UIElement MarkerElement = markerLayer as UIElement;
                      UIElement polylineElement = polyline as UIElement;
                      UIElement polyElement = poly as UIElement;

                      var markerZ = 0;
                      var polyZ = 0;
                      var lineZ = 0;

                      if (MarkerElement != null)
                      {
                          markerZ = Canvas.GetZIndex(MarkerElement);
                      }

                      if (polylineElement != null)
                      {
                          lineZ = Canvas.GetZIndex(polylineElement);
                      }

                      if (polyElement != null)
                      {
                          polyZ = Canvas.GetZIndex(polyElement);
                      }

                      if (selected_shape == "Polygon")
                      {
                          if (polyZ >= markerZ)
                              polyZ = markerZ - 1;

                          if (polyZ >= lineZ)
                              polyZ = lineZ - 1;
                      }
                      else if (selected_shape == "Polyline")
                      {
                          if (lineZ >= markerZ)
                              lineZ = markerZ - 1;

                          if (lineZ >= polyZ)
                              lineZ = polyZ - 1;
                      }
                      else if (selected_shape == "Markers")
                      {
                          if (markerZ >= lineZ)
                              markerZ = lineZ - 1;

                          if (markerZ >= polyZ)
                              markerZ = polyZ - 1;
                      }

                      if (MarkerElement != null)
                      {
                          Debug.WriteLine("Set MarkerElement: " + markerZ);
                          Canvas.SetZIndex(MarkerElement, markerZ);
                      }

                      if (polylineElement != null)
                      {
                          Debug.WriteLine("Set polylineElement: " + lineZ);
                          Canvas.SetZIndex(polylineElement, lineZ);
                      }

                      if (polyElement != null)
                      {
                          Debug.WriteLine("Set polyElement: " + polyZ);
                          Canvas.SetZIndex(polyElement, polyZ);
                      }*/
        }

        void BringtoTop()
        {
            /*        UIElement MarkerElement = markerLayer as UIElement;
                    UIElement polylineElement = polyline as UIElement;
                    UIElement polyElement = poly as UIElement;

                    var markerZ = 0;
                    var polyZ = 0;
                    var lineZ = 0;

                    if (MarkerElement != null)
                    {
                        markerZ = Canvas.GetZIndex(MarkerElement);
                    }

                    if (polylineElement != null)
                    {
                        lineZ = Canvas.GetZIndex(polylineElement);
                    }

                    if (polyElement != null)
                    {
                        polyZ = Canvas.GetZIndex(polyElement);
                    }

                    if (selected_shape == "Polygon")
                    {
                        if (polyZ <= markerZ)
                            polyZ = markerZ + 1;

                        if (polyZ <= lineZ)
                            polyZ = lineZ + 1;
                    }
                    else if (selected_shape == "Polyline")
                    {
                        if (lineZ <= markerZ)
                            lineZ = markerZ + 1;

                        if (lineZ <= polyZ)
                            lineZ = polyZ + 1;
                    }
                    else if (selected_shape == "Markers")
                    {
                        if (markerZ <= lineZ)
                            markerZ = lineZ + 1;

                        if (markerZ <= polyZ)
                            markerZ = polyZ + 1;
                    }

                    if (MarkerElement != null)
                    {
                        Debug.WriteLine("Set MarkerElement: " + markerZ);
                        Canvas.SetZIndex(MarkerElement, markerZ);
                    }

                    if (polylineElement != null)
                    {
                        Debug.WriteLine("Set polylineElement: " + lineZ);
                        Canvas.SetZIndex(polylineElement, lineZ);
                    }

                    if (polyElement != null)
                    {
                        Debug.WriteLine("Set polyElement: " + polyZ);
                        Canvas.SetZIndex(polyElement, polyZ);
                    }*/
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Close the popup.
            PopupP.IsOpen = false;
            Debug.WriteLine("Cancel_Click ");
        }

    }
}