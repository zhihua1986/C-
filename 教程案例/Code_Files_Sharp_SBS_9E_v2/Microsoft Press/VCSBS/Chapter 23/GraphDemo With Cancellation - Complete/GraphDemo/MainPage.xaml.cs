﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GraphDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Reduce pixelWidth and pixelHeight if there is insufficient memory available
        private int pixelWidth = 15000;
        private int pixelHeight = 10000;

        private WriteableBitmap graphBitmap = null;
        private int bytesPerPixel = 4;
        private byte[] data;

        private byte redValue, greenValue, blueValue;
        private CancellationTokenSource tokenSource = null;

        public MainPage()
        {
            this.InitializeComponent();

            // Initialize the array that will hold the graph data
            int dataSize = bytesPerPixel * pixelWidth * pixelHeight;
            data = new byte[dataSize];

            // Initialize the bitmap that will be used to display the graph data
            graphBitmap = new WriteableBitmap(pixelWidth, pixelHeight);
        }

        // Generate the data for the graph and display it
        private async void plotButton_Click(object sender, RoutedEventArgs e)
        {
            // Generate random values for the RGB intensity of the graph
            // The graph will appear in a different color each time the user clicks the Plot button
            Random rand = new Random();
            redValue = (byte)rand.Next(0xFF);
            greenValue = (byte)rand.Next(0xFF);
            blueValue = (byte)rand.Next(0xFF);

            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            // Start a timer
            Stopwatch watch = Stopwatch.StartNew();

            // Generate the data for the graph
            Task first = Task.Run(() => generateGraphData(data, 0, pixelWidth / 4, token), token);
            Task second = Task.Run(() => generateGraphData(data, pixelWidth / 4, pixelWidth / 2, token), token);
            // Task.WaitAll(first, second);

            try
            {
                await first;
                await second;

                // Display the time taken to generate the data
                duration.Text = $"Duration (ms): {watch.ElapsedMilliseconds}";
            }
            catch (OperationCanceledException oce)
            {
                duration.Text = oce.Message;
            }

            string message = $"Status of tasks is {first.Status}, {second.Status}";
            messages.Text = message;

            // Display the data by using the bitmap
            Stream pixelStream = graphBitmap.PixelBuffer.AsStream();
            pixelStream.Seek(0, SeekOrigin.Begin);
            pixelStream.Write(data, 0, data.Length);
            graphBitmap.Invalidate();
            graphImage.Source = graphBitmap;
        }

        // Stop graph generation and display
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
        }

        // Complex function that generates the data for the graph
        private void generateGraphData(byte[] data, int partitionStart, int partitionEnd, CancellationToken token)
        {
            int a = pixelWidth / 2;
            int b = a * a;
            int c = pixelHeight / 2;

            for (int x = partitionStart; x < partitionEnd; x++)
            {
                int s = x * x;
                double p = Math.Sqrt(b - s);
                for (double i = -p; i < p; i += 3)
                {
                    //if (token.IsCancellationRequested)
                    //{
                    //    return;
                    //}
                    token.ThrowIfCancellationRequested();

                    double r = Math.Sqrt(s + i * i) / a;
                    double q = (r - 1) * Math.Sin(24 * r);
                    double y = i / 3 + (q * c);
                    plotXY(data, (int)(-x + (pixelWidth / 2)), (int)(y + (pixelHeight / 2)));
                    plotXY(data, (int)(x + (pixelWidth / 2)), (int)(y + (pixelHeight / 2)));
                }
            }
        }

        // Store the data for a given point in the graph in the array
        private void plotXY(byte[] data, int x, int y)
        {
            int pixelIndex = (x + y * pixelWidth) * bytesPerPixel;
            data[pixelIndex] = blueValue;
            data[pixelIndex + 1] = greenValue;
            data[pixelIndex + 2] = redValue;
            data[pixelIndex + 3] = 0xBF;
        }
    }
}
