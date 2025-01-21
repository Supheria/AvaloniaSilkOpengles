// using System.Collections.Generic;
// using Avalonia;
// using LocalUtilities.General;
//
// namespace LocalUtilities.Extensions;
//
// public static class RectExtension
// {
//     public static Size ScaleSizeWithinRatio(this Size fromSize, Size toSize)
//     {
//         var toWidth = toSize.Width;
//         var toHeight = toSize.Height;
//         var toRatio = toSize.Width / toSize.Height;
//         var sourceRatio = fromSize.Width / fromSize.Height;
//         if (sourceRatio > toRatio)
//         {
//             toWidth = toSize.Width;
//             toHeight = toWidth / sourceRatio;
//         }
//         else if (sourceRatio < toRatio)
//         {
//             toHeight = toSize.Height;
//             toWidth = toHeight * sourceRatio;
//         }
//         return new(toWidth, toHeight);
//     }
//
//     /// <summary>
//     /// guarantee vertexes of <paramref name="target"/> permanently site on <paramref name="range"/>
//     /// </summary>
//     /// <param name="target"></param>
//     /// <param name="range"></param>
//     /// <param name="left"></param>
//     /// <param name="right"></param>
//     /// <param name="top"></param>
//     /// <param name="bottom"></param>
//     private static void SiteRectInRange(
//         Rect target,
//         Rect range,
//         out double left,
//         out double right,
//         out double top,
//         out double bottom
//     )
//     {
//         left = range.Left + (target.Left - range.Left) % range.Width;
//         left = left < range.Left ? left + range.Width : left;
//         right = range.Right + (target.Right - range.Right) % range.Width;
//         right = right > range.Right ? right - range.Width : right;
//         top = range.Top + (target.Top - range.Top) % range.Height;
//         top = top < range.Top ? top + range.Height : top;
//         bottom = range.Bottom + (target.Bottom - range.Bottom) % range.Height;
//         bottom = bottom > range.Bottom ? bottom - range.Height : bottom;
//     }
//
//     public static List<Edge> CutRectLoopEdgesInRange(this Rect target, Rect range)
//     {
//         if (target.Width is 0 || target.Height is 0)
//             return [];
//         SiteRectInRange(target, range, out var left, out var right, out var top, out var bottom);
//         var edges = new List<Edge>();
//         if (left < right)
//             edges.AddRange(
//                 [new(new(left, top), new(right, top)), new(new(left, bottom), new(right, bottom))]
//             );
//         else
//             edges.AddRange(
//                 [
//                     new(new(range.Left, top), new(right, top)),
//                     new(new(range.Left, bottom), new(right, bottom)),
//                     new(new(left, top), new(range.Right, top)),
//                     new(new(left, bottom), new(range.Right, bottom)),
//                 ]
//             );
//         if (top < bottom)
//             edges.AddRange(
//                 [new(new(left, top), new(left, bottom)), new(new(right, top), new(right, bottom))]
//             );
//         else
//             edges.AddRange(
//                 [
//                     new(new(left, range.Top), new(left, bottom)),
//                     new(new(right, range.Top), new(right, bottom)),
//                     new(new(left, top), new(left, range.Bottom)),
//                     new(new(right, top), new(right, range.Bottom)),
//                 ]
//             );
//         return edges;
//     }
//
//     public static List<Rect> CutRectLoopRectsInRange(this Rect source, Rect range)
//     {
//         if (source.Width is 0 || source.Height is 0)
//             return [];
//         SiteRectInRange(source, range, out var left, out var right, out var top, out var bottom);
//         if (left > right && top > bottom)
//             return
//             [
//                 new(range.Left, range.Top, right - range.Left, bottom - range.Top),
//                 new(range.Left, top, right - range.Left, range.Bottom - top),
//                 new(left, range.Top, range.Right - left, bottom - range.Top),
//                 new(left, top, range.Right - left, range.Bottom - top),
//             ];
//         if (left < right && top > bottom)
//             return
//             [
//                 new(left, range.Top, right - left, bottom - range.Top),
//                 new(left, top, right - left, range.Bottom - top),
//             ];
//         if (top < bottom && left > right)
//             return
//             [
//                 new(range.Left, top, right - range.Left, bottom - top),
//                 new(left, top, range.Right - left, bottom - top),
//             ];
//         return [new(left, top, right - left, bottom - top)];
//     }
//
//     public static List<Edge> GetRectEdges(this Rect rect)
//     {
//         return
//         [
//             new(new(rect.Left, rect.Top), new(rect.Right, rect.Top)),
//             new(new(rect.Left, rect.Bottom), new(rect.Right, rect.Bottom)),
//             new(new(rect.Left, rect.Top), new(rect.Left, rect.Bottom)),
//             new(new(rect.Right, rect.Top), new(rect.Right, rect.Bottom)),
//         ];
//     }
// }
