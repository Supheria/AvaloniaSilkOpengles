using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaSilkOpengles.Graphics;

namespace AvaloniaSilkOpengles.Controls;

public partial class HelloSquareContainer : UserControl
{
    // DispatcherTimer Timer {get;} = new();
    // int FrameCount {get;set;}
    // Stopwatch Stopwatch { get; } = new();
    public HelloSquareContainer()
    {
        InitializeComponent();
        HelloSquare.FrameInfoUpdated += FrameInfoUpdated;
        // Timer.Interval = TimeSpan.FromMilliseconds(10);
        // Timer.Tick += TimerOnTick;
        // Timer.Start();
        // Stopwatch.Start();
    }

    private void FrameInfoUpdated(object? sender, FrameInfo e)
    {
        Fps.Text = $"{e.Fps:F1} fps";
        Mspf.Text = $"{e.Mspf:F1} mspf";
    }

    // private void TimerOnTick(object? sender, EventArgs e)
    // {
    //     HelloSquare.RequestNextFrameRendering();
    //     FrameCount++;
    //     var ms = Stopwatch.ElapsedMilliseconds;
    //     if (ms > 1000)
    //     {
    //         var fps = FrameCount * 1000 / ms;
    //         FpsCount.Text = fps.ToString();
    //         FrameCount = 0;
    //         Stopwatch.Restart();
    //     }
    // }
}