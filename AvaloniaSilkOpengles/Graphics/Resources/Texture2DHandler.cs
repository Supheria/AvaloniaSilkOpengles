using Avalonia.Media.Imaging;
using Avalonia.OpenGL;
using Avalonia.Skia;
using AvaloniaSilkOpengles.Assets.Textures;
using Silk.NET.OpenGLES;
using SkiaSharp;
using StbImageSharp;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public class Texture2DHandler : ResourceHandler
{
    public int Unit { get; }
    public string TextureName { get; }

    public Texture2DHandler(GL gl, string textureName, int unit)
        : base(gl)
    {
        TextureName = textureName;
        Unit = unit;
        Handle = Load(gl, textureName, unit);
    }

    private static uint Load(GL gl, string textureName, int unit)
    {
        gl.ActiveTexture(TextureUnit.Texture0 + unit);

        var handle = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, handle);

        gl.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapS,
            (int)TextureWrapMode.Repeat
        );
        gl.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapT,
            (int)TextureWrapMode.Repeat
        );
        gl.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Nearest
        );
        gl.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter,
            (int)TextureMinFilter.Nearest
        );

        using var source = TextureRead.Read(textureName);
        var texture = ImageResult.FromStream(source);
        gl.TexImage2D<byte>(
            TextureTarget.Texture2D,
            0,
            InternalFormat.Rgba,
            (uint)texture.Width,
            (uint)texture.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            texture.Data
        );

        gl.BindTexture(TextureTarget.Texture2D, 0);

        return handle;
    }

    public void Bind()
    {
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Unbind()
    {
        Gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Delete()
    {
        Gl.DeleteTexture(Handle);
        Gl.Dispose();
    }
}
