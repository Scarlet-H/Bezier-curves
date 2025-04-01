﻿using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bezier;

public partial class MainWindow : Window
{
    readonly PointCollection points = [];
    int counter = 0;
    private DispatcherTimer timer = new();
    private int animationStep = 0;
    PointCollection curvePoints = [];
    Polyline bezierCurve = new();
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Polyline lines = new() { Stroke = Brushes.Black, StrokeThickness = 0.5 };
        if (countInput.Text != "" && counter < int.Parse(countInput.Text))
        {
            lines.Points = points;
            counter++;
            Point mousePos = Mouse.GetPosition(canvas);
            points.Add(mousePos);
            Ellipse dot = new() { Width = 5, Height = 5, Fill = Brushes.Black };
            Canvas.SetLeft(dot, mousePos.X - dot.Width / 2);
            Canvas.SetTop(dot, mousePos.Y - dot.Height / 2);
            canvas.Children.Add(dot);
            canvas.Children.Add(lines);
        }
        if (counter > 0)
            if (counter == int.Parse(countInput.Text))
                DrawBezierCurve();
    }
    private void DrawBezierCurve()
    {
        // ------------------- красивая кисточка --------------------
        LinearGradientBrush myLinearGradientBrush = new() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
        myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
        myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.25));
        myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.5));
        myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.LimeGreen, 1.0));
        // ----------------------------------------------------------
        if (points.Count < 3) return;
        bezierCurve = new Polyline { Stroke = myLinearGradientBrush, StrokeThickness = 4 };
        curvePoints = [];
        bezierCurve.Points = curvePoints;
        canvas.Children.Add(bezierCurve);
        StartAnimation();
    }
    private static Point GetBezierPoint(double t, PointCollection points)
    {
        double n = points.Count - 1;
        Point result = new(0, 0);
        for (int i = 0; i <= n; i++)
        {
            result.X += BinomialCoefficient(n, i) * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * points[i].X;
            result.Y += BinomialCoefficient(n, i) * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * points[i].Y;
        }
        return result;
    }
    private static double Factorial(double n)
    {
        double factorial = 1;
        for (int i = 1; i <= n; i++)
        {
            factorial *= i;
        }
        return factorial;
    }
    private static double BinomialCoefficient(double n, double k)
    {
        if (k == 0 || k == n) return 1;
        return Factorial(n) / (Factorial(k) * Factorial(n - k));
    }
    private void StartAnimation()
    {
        timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        timer.Tick += AnimateBezier;
        timer.Start();
    }
    private void AnimateBezier(object sender, EventArgs e)
    {
        double t = animationStep / 100.0;
        if (t > 1)
        {
            timer.Stop();
            return;
        }
        curvePoints.Add(GetBezierPoint(t, points));
        bezierCurve.Points = curvePoints;
        animationStep++;
    }
    private void countInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (countInput.Text == "")
        {
            canvas.Children.Clear();
            points.Clear();
            counter = 0;
            animationStep = 0;
        }
    }
}