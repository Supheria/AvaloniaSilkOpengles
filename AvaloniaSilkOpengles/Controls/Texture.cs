using System;
using System.IO;
using Avalonia.OpenGL;
using AvaloniaSilkOpengles.Assets.Textures;
using Silk.NET.OpenGLES;
using StbImageSharp;

namespace AvaloniaSilkOpengles.Controls;

public class Texture
{
    GL Gl { get; }
    string UniformUniformName {get;}
    uint TextureHandler { get; set; }

    public Texture(GlInterface gl, string uniformName)
    {
        Gl = GL.GetApi(gl.GetProcAddress);
        UniformUniformName = uniformName;
    }

    public void Load(Stream source)
    {
        // Gl.ActiveTexture(TextureUnit.Texture1);
        
        TextureHandler = Gl.GenTexture();
        Gl.BindTexture(TextureTarget.Texture2D, TextureHandler);

        Gl.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapS,
            (int)TextureWrapMode.Repeat
        );
        Gl.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapT,
            (int)TextureWrapMode.Repeat
        );
        Gl.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Nearest
        );
        Gl.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter,
            (int)TextureMinFilter.Nearest
        );

        StbImage.stbi_set_flip_vertically_on_load(1);
        var texture = ImageResult.FromStream(source, ColorComponents.RedGreenBlueAlpha);
        Gl.TexImage2D<byte>(
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

        Gl.BindTexture(TextureTarget.Texture2D, 0);
    }
    
    public void Bind()
    {
        Gl.BindTexture(TextureTarget.Texture2D, TextureHandler);
    }
    
    public void Unbind()
    {
        Gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Delete()
    {
        if (TextureHandler is not 0)
            Gl.DeleteTexture(TextureHandler);
        Gl.Dispose();
    }
}
