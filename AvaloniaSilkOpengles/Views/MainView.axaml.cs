using Avalonia.Controls;
using AvaloniaSilkOpengles.Controls;

namespace AvaloniaSilkOpengles.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Gl1.SetGlControl(new ChunkLoader());
        Gl2.SetGlControl(new HelloModel());
        Gl3.SetGlControl(new TerrainLoader());
        Gl4.SetGlControl(new HelloSphere());
    }
}