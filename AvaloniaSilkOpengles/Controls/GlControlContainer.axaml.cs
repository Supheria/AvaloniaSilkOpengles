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

public partial class GlControlContainer : UserControl
{
    public GlControlContainer()
    {
        InitializeComponent();
    }
    
    public void SetGlControl(SilkNetOpenGlControl glControl)
    {
        GlControl.Content = glControl;
        glControl.FrameInfoUpdated += FrameInfoUpdated;
    }

    private void FrameInfoUpdated(object? sender, FrameInfo e)
    {
        Fps.Text = $"{e.Fps:F1} fps";
        Mspf.Text = $"{e.Mspf:F1} mspf";
    }
}