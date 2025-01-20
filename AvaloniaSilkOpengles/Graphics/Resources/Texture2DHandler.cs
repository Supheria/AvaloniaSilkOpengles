using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;
using Avalonia.OpenGL;
using Avalonia.Skia;
using AvaloniaSilkOpengles.Assets.Textures;
using Silk.NET.OpenGLES;
using SkiaSharp;
using StbImageSharp;
using Bitmap = System.Drawing.Bitmap;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public class Texture2DHandler : ResourceHandler
{
    public TextureType Type { get; }
    public int Unit { get; }

    public Texture2DHandler(GL gl, Stream stream, TextureType type, int unit)
        : base(gl)
    {
        Type = type;
        Unit = unit;
        Handle = Load(gl, stream, unit);
    }

    record ImageData(byte[] Data, int Width, int Height, int ColumnNumber);

    static unsafe ImageData LoadFromStream(Stream stream)
    {
        var ptr = (void*)null;
        try
        {
            int width;
            int height;
            int comp;
            var info = new StbImage.stbi__result_info();
            ptr = StbImage.stbi__load_and_postprocess_8bit(
                new StbImage.stbi__context(stream),
                &width,
                &height,
                &comp,
                0
            );
            var data = new byte[width * height * comp];
            Marshal.Copy(new IntPtr(ptr), data, 0, data.Length);
            return new ImageData(data, width, height, comp);
            // return ImageResult.FromResult(ptr, width, height, (ColorComponents) comp, requiredComponents);
        }
        finally
        {
            if ((IntPtr)ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(new IntPtr(ptr));
        }
    }

    private static uint Load(GL gl, Stream stream, int unit)
    {
        // ImageResult.FromStream()
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

        var image = LoadFromStream(stream);
        InternalFormat internalFormat;
        PixelFormat pixelFormat;
        switch (image.ColumnNumber)
        {
            case 4:
                internalFormat = InternalFormat.Rgba;
                pixelFormat = PixelFormat.Rgba;
                break;
            case 3:
                internalFormat = InternalFormat.Rgb;
                pixelFormat = PixelFormat.Rgb;
                break;
            case 1:
                internalFormat = InternalFormat.Red;
                pixelFormat = PixelFormat.Red;
                break;
            default:
                throw new ArgumentException("Automatic Texture type recognition failed");
        }
        gl.TexImage2D<byte>(
            TextureTarget.Texture2D,
            0,
            internalFormat,
            (uint)image.Width,
            (uint)image.Height,
            0,
            pixelFormat,
            PixelType.UnsignedByte,
            image.Data
        );

        gl.BindTexture(TextureTarget.Texture2D, 0);

        return handle;
    }

    public void Bind()
    {
        Gl.ActiveTexture(TextureUnit.Texture0 + Unit);
        Gl.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Unbind()
    {
        Gl.ActiveTexture(TextureUnit.Texture0 + Unit);
        Gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Delete()
    {
        Gl.DeleteTexture(Handle);
        Gl.Dispose();
    }
}
