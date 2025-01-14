using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace AvaloniaSilkOpengles.Controls;

public partial class HelloSquareContainer : UserControl
{
    DispatcherTimer Timer {get;} = new();
    int FrameCount {get;set;}
    Stopwatch Stopwatch { get; } = new();
    public HelloSquareContainer()
    {
        InitializeComponent();
        Timer.Interval = TimeSpan.FromMilliseconds(10);
        Timer.Tick += TimerOnTick;
        Timer.Start();
        Stopwatch.Start();
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        HelloSquare.RequestNextFrameRendering();
        FrameCount++;
        var ms = Stopwatch.ElapsedMilliseconds;
        if (ms > 1000)
        {
            var fps = FrameCount * 1000 / ms;
            FpsCount.Text = fps.ToString();
            FrameCount = 0;
            Stopwatch.Restart();
        }
    }
}