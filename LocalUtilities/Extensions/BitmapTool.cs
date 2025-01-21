//using LocalUtilities.TypeGeneral;
//using System.Drawing.Drawing2D;

//namespace LocalUtilities.TypeToolKit.Graph;

//public static class BitmapTool
//{
//    public static Bitmap CopyToNewSize(this Bitmap source, Size newSize, InterpolationMode mode)
//    {
//        var target = new Bitmap(newSize.Width, newSize.Height);
//        using var g = Graphics.FromImage(target);
//        //g.Clear(Color.White);
//        g.InterpolationMode = mode;
//        g.DrawImage(source, 0, 0, newSize.Width, newSize.Height);
//        //g.Flush(); g.Dispose();
//        //source.Dispose();
//        return target;
//    }

//    public static void TemplateDrawIntoRect(this Bitmap template, Bitmap target, Rectangle toRect, bool ignoreTransparent)
//    {
//        var dWidth = toRect.Width - template.Width;
//        var dHeight = toRect.Height - template.Height;
//        var startX = dWidth < 0 ? throw GraphicException.SizeOutRange(template.Size, toRect) : toRect.Left + dWidth / 2;
//        var startY = dHeight < 0 ? throw GraphicException.SizeOutRange(template.Size, toRect) : toRect.Top + dHeight / 2;
//        var pTemplate = new PointBitmap(template);
//        var pTarget = new PointBitmap(target);
//        pTemplate.LockBits();
//        pTarget.LockBits();
//        for (int x = 0; x < template.Width; x++)
//        {
//            for (int y = 0; y < template.Height; y++)
//            {
//                var pixel = pTemplate.GetPixel(x, y);
//                if (!ignoreTransparent || pixel.A != 0 || pixel.R != 0 || pixel.G != 0 || pixel.B != 0)
//                    pTarget.SetPixel(x + startX, y + startY, pixel);
//            }
//        }
//        pTemplate.UnlockBits();
//        pTarget.UnlockBits();
//    }

//    public static void DrawTemplateSamePartsOnto(this Bitmap template, Bitmap source, List<Rectangle> drawRects, bool ignoreTransparent)
//    {
//        if (source.Size != template.Size)
//            throw GraphicException.SizeMismatch();
//        using var g = Graphics.FromImage(source);
//        foreach (var rect in drawRects)
//            g.DrawImage(template, rect, rect, GraphicsUnit.Pixel);
//        //PointBitmap pTemplate = new(template);
//        //PointBitmap pSource = new(source);
//        //pTemplate.LockBits();
//        //pSource.LockBits();
//        //foreach (var rect in drawRects)
//        //{
//        //    for (var x = rect.X; x < rect.Right; x++)
//        //    {
//        //        for (var y = rect.Y; y < rect.Bottom; y++)
//        //        {
//        //            var pixel = pTemplate.GetPixel(x, y);
//        //            if (!ignoreTransparent || pixel.A != 0 || pixel.R != 0 || pixel.G != 0 || pixel.B != 0)
//        //                pSource.SetPixel(x, y, pixel);
//        //        }
//        //    }
//        //}
//        //pTemplate.UnlockBits();
//        //pSource.UnlockBits();
//    }

//    public static void DrawTemplateOnto(this Bitmap template, Bitmap source, Rectangle part, bool ignoreTransparent)
//    {
//        //PointBitmap pTemplate = new(template);
//        //PointBitmap pSource = new(source);
//        //pTemplate.LockBits();
//        //pSource.LockBits();
//        //for (var i = 0; i < part.Width; i++)
//        //{
//        //    for (var j = 0; j < part.Height; j++)
//        //    {
//        //        var pixel = pTemplate.GetPixel(i, j);
//        //        if (!ignoreTransparent || pixel.A != 0 || pixel.R != 0 || pixel.G != 0 || pixel.B != 0)
//        //            pSource.SetPixel(i + part.Left, j + part.Top, pixel);
//        //    }
//        //}
//        //pTemplate.UnlockBits();
//        //pSource.UnlockBits();
//        using var g = Graphics.FromImage(source);
//        g.DrawImage(template, part);
//    }

//    /// <summary>
//    /// 反色
//    /// </summary>
//    /// <param name="color"></param>
//    /// <returns></returns>
//    public static Color GetInverseColor(Color color)
//    {
//        var r = 255 - color.R;
//        var g = 255 - color.G;
//        var b = 255 - color.B;
//        return Color.FromArgb(color.A, r, g, b);
//    }

//    /// <summary>
//    /// 混合二色
//    /// </summary>
//    /// <param name="color1"></param>
//    /// <param name="color2"></param>
//    /// <returns></returns>
//    public static Color GetMixedColor(Color color1, Color color2)
//    {
//        var a = (color1.A + color2.A) / 2;
//        var r = (color1.R + color2.R) / 2;
//        var g = (color1.G + color2.G) / 2;
//        var b = (color1.B + color2.B) / 2;
//        return Color.FromArgb(a, r, g, b);
//    }
//}
