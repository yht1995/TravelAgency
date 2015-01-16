using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TravelAgency
{
    internal class ValueToProcessConverter : IValueConverter
    {
        private const double Thickness = 8;
        private const double Padding = 1;
        private const double WarnValue = 60;
        private const int SuccessRateFontSize = 34;
        private static readonly SolidColorBrush NormalBrush;
        private static readonly SolidColorBrush WarnBrush;
        private static readonly Typeface SuccessRateTypeface;
        private Point centerPoint;
        private string percentString;
        private double radius;

        static ValueToProcessConverter()
        {
            NormalBrush = new SolidColorBrush(Colors.Green);
            WarnBrush = new SolidColorBrush(Colors.Red);
            SuccessRateTypeface = new Typeface(new FontFamily("MSYH"), new FontStyle(), new FontWeight(),
                new FontStretch());
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && !string.IsNullOrEmpty((string) parameter))
            {
                var arg = (double) value;
                var width = double.Parse((string) parameter);
                radius = width/2;
                centerPoint = new Point(radius, radius);

                return DrawBrush(arg, 100, radius, radius, Thickness, Padding);
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     根据角度获取坐标
        /// </summary>
        /// <param name="CenterPoint"></param>
        /// <param name="r"></param>
        /// <param name="angel"></param>
        /// <returns></returns>
        private Point GetPointByAngel(Point CenterPoint, double r, double angel)
        {
            var p = new Point();
            p.X = Math.Sin(angel*Math.PI/180)*r + CenterPoint.X;
            p.Y = CenterPoint.Y - Math.Cos(angel*Math.PI/180)*r;

            return p;
        }

        /// <summary>
        ///     根据4个坐标画出扇形
        /// </summary>
        /// <param name="bigFirstPoint"></param>
        /// <param name="bigSecondPoint"></param>
        /// <param name="smallFirstPoint"></param>
        /// <param name="smallSecondPoint"></param>
        /// <param name="bigRadius"></param>
        /// <param name="smallRadius"></param>
        /// <param name="isLargeArc"></param>
        /// <returns></returns>
        private Geometry DrawingArcGeometry(Point bigFirstPoint, Point bigSecondPoint, Point smallFirstPoint,
            Point smallSecondPoint, double bigRadius, double smallRadius, bool isLargeArc)
        {
            var pathFigure = new PathFigure {IsClosed = true};
            pathFigure.StartPoint = bigFirstPoint;
            pathFigure.Segments.Add(
                new ArcSegment
                {
                    Point = bigSecondPoint,
                    IsLargeArc = isLargeArc,
                    Size = new Size(bigRadius, bigRadius),
                    SweepDirection = SweepDirection.Clockwise
                });
            pathFigure.Segments.Add(new LineSegment {Point = smallSecondPoint});
            pathFigure.Segments.Add(
                new ArcSegment
                {
                    Point = smallFirstPoint,
                    IsLargeArc = isLargeArc,
                    Size = new Size(smallRadius, smallRadius),
                    SweepDirection = SweepDirection.Counterclockwise
                });
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            return pathGeometry;
        }

        /// <summary>
        ///     根据当前值和最大值获取扇形
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        private Geometry GetGeometry(double value, double maxValue, double radiusX, double radiusY, double thickness,
            double padding)
        {
            var isLargeArc = false;
            var percent = value/maxValue;
            percentString = string.Format("{0}%", Math.Round(percent*100));
            var angel = percent*360D;
            if (angel > 180) isLargeArc = true;
            var bigR = radiusX;
            var smallR = radiusX - thickness + padding;
            var firstpoint = GetPointByAngel(centerPoint, bigR, 0);
            var secondpoint = GetPointByAngel(centerPoint, bigR, angel);
            var thirdpoint = GetPointByAngel(centerPoint, smallR, 0);
            var fourpoint = GetPointByAngel(centerPoint, smallR, angel);
            return DrawingArcGeometry(firstpoint, secondpoint, thirdpoint, fourpoint, bigR, smallR, isLargeArc);
        }

        private void DrawingGeometry(DrawingContext drawingContext, double value, double maxValue, double radiusX,
            double radiusY, double thickness, double padding)
        {
            if (value != maxValue)
            {
                SolidColorBrush brush;
                if (value < WarnValue)
                {
                    brush = WarnBrush;
                }
                else
                {
                    brush = NormalBrush;
                }
                drawingContext.DrawEllipse(null,
                    new Pen(new SolidColorBrush(Color.FromRgb(0xdd, 0xdf, 0xe1)), thickness), centerPoint, radiusX,
                    radiusY);
                drawingContext.DrawGeometry(brush, new Pen(),
                    GetGeometry(value, maxValue, radiusX, radiusY, thickness, padding));
                var formatWords = new FormattedText(percentString,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    SuccessRateTypeface,
                    SuccessRateFontSize,
                    brush);
                var startPoint = new Point(centerPoint.X - formatWords.Width/2, centerPoint.Y - formatWords.Height/2);
                drawingContext.DrawText(formatWords, startPoint);
            }
            else
            {
                drawingContext.DrawEllipse(null, new Pen(NormalBrush, thickness), centerPoint, radiusX, radiusY);
                var formatWords = new FormattedText("100%",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    SuccessRateTypeface,
                    SuccessRateFontSize,
                    NormalBrush);
                var startPoint = new Point(centerPoint.X - formatWords.Width/2, centerPoint.Y - formatWords.Height/2);
                drawingContext.DrawText(formatWords, startPoint);
            }

            drawingContext.Close();
        }

        /// <summary>
        ///     根据当前值和最大值画出进度条
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        private Visual DrawShape(double value, double maxValue, double radiusX, double radiusY, double thickness,
            double padding)
        {
            var drawingWordsVisual = new DrawingVisual();
            var drawingContext = drawingWordsVisual.RenderOpen();

            DrawingGeometry(drawingContext, value, maxValue, radiusX, radiusY, thickness, padding);

            return drawingWordsVisual;
        }

        /// <summary>
        ///     根据当前值和最大值画出进度条
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        private Brush DrawBrush(double value, double maxValue, double radiusX, double radiusY, double thickness,
            double padding)
        {
            var drawingGroup = new DrawingGroup();
            var drawingContext = drawingGroup.Open();

            DrawingGeometry(drawingContext, value, maxValue, radiusX, radiusY, thickness, padding);

            var brush = new DrawingBrush(drawingGroup);

            return brush;
        }
    }
}