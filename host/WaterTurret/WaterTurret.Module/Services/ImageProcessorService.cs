using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Practices.Prism.ViewModel;
using WaterTurret.Module.Models;
using System.Windows.Media.Imaging;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using WaterTurret.Module.Helpers;

namespace WaterTurret.Module.Services
{
    public class ImageProcessorService : NotificationObject, IImageProcessorService
    {
        private readonly IConfigService _configService;
        private readonly ICoreService _coreService;

        public ImageProcessorService(IConfigService configService, ICoreService coreService)
        {
            _configService = configService;
            _coreService = coreService;

            BackgroundBitmap = new Bitmap(_configService.DeviceConfig.PixelWidth, _configService.DeviceConfig.PixelHeight);
        }

        public BitmapImage BackgroundBitmapImage
        {
            get
            {
                return BackgroundBitmap.ToBitmapImage();
            }
        }

        private Bitmap _backgroundBitmap;
        public Bitmap BackgroundBitmap
        {
            get
            {
                return _backgroundBitmap;
            }
            private set
            {
                _backgroundBitmap = value;
                RaisePropertyChanged(() => BackgroundBitmap);
                RaisePropertyChanged(() => BackgroundBitmapImage);
            }
        }


        BlobCounter _blobCounter;
        ViewConfig _viewConfig;
        ImageProcessorConfig _imageProcessorConfig;


        public ImageProcessorResult Process(Bitmap bitmap, bool rgb)
        {
            var result = new ImageProcessorResult();

            _viewConfig = _configService.ViewConfig;
            _imageProcessorConfig = _configService.ImageProcessorConfig;

            new Blur().ApplyInPlace(bitmap);

            Bitmap overlay = bitmap;

            if (_viewConfig.BackgroundImage == ViewConfigBackgroundImage.CameraRaw)
            {
                // 카메라 원본
                overlay = bitmap;
            }

            // 그레이 스케일
            var grayscale = Grayscale.CommonAlgorithms.BT709.Apply(bitmap);

            // 경계 검출
            var edges = new DifferenceEdgeDetector().Apply(grayscale);
            if (_viewConfig.BackgroundImage == ViewConfigBackgroundImage.Edge)
            {
                overlay = new GrayscaleToRGB().Apply(edges);
            }

            // 이진화
            // var threshold = new Threshold(_imageProcessorConfig.Threshold).Apply(edges);
            var threshold = new OtsuThreshold().Apply(edges);
            if (_viewConfig.BackgroundImage == ViewConfigBackgroundImage.Binary)
            {
                overlay = new GrayscaleToRGB().Apply(threshold);
            }

            // 오버레이 복제
            overlay = overlay.CloneBitmap();

            // 오버레이 데이터
            var overlayData = overlay.LockBits(overlay.GetRectangle(), ImageLockMode.ReadWrite, overlay.PixelFormat);

            _blobCounter = new BlobCounter();
            _blobCounter.MinHeight = _imageProcessorConfig.BlobMinHeight;
            _blobCounter.MinHeight = _imageProcessorConfig.BlobMinWidth;
            _blobCounter.FilterBlobs = true;
            _blobCounter.ObjectsOrder = ObjectsOrder.XY;
            _blobCounter.ProcessImage(threshold);

            var blobs = _blobCounter.GetObjectsInformation();

            var shapeChecker = new SimpleShapeChecker();


            // 각 영역에 대해 처리 수행
            foreach (var blob in blobs)
            {
                // 현재 시도하는 마커
                Marker marker = new Marker();

                var edgePoints = _blobCounter.GetBlobsEdgePoints(blob);

                // 사각형 판정
                var points = new List<IntPoint>();
                if (shapeChecker.IsQuadrilateral(edgePoints, out points))
                {
                    marker.Points = points;
                    List<IntPoint> leftEdge, rightEdge;
                    _blobCounter.GetBlobsLeftAndRightEdges(blob, out leftEdge, out rightEdge);

                    // 각도와 길이 판정
                    if (MoreQuadTest(blob, marker, leftEdge, rightEdge))
                    {
                        // 검은색 테두리 판정
                        if (BorderTest(blob, marker, grayscale, leftEdge, rightEdge))
                        {
                            // 판정 완료
                            result.Markers.Add(marker);

                            // 무게 중심 좌표
                            marker.X = blob.CenterOfGravity.X - (threshold.Width / 2);
                            marker.Y = -(blob.CenterOfGravity.Y - (threshold.Height / 2));

                            // 프레임워크에서 계산한 넓이
                            marker.FrameworkArea = blob.Area;

                            // 오버레이
                            ApplyOverlay(overlayData, marker.Points);
                        }
                    }
                }
            }

            overlay.UnlockBits(overlayData);

            foreach (var marker in result.Markers)
            {
                var points = marker.Points;

                // 방향 보정
                var sideLength = points[0].DistanceTo(points[1]);

                if (points[2].Y - points[1].Y < sideLength / 1.6)
                {
                    points = new List<IntPoint>(
                        new IntPoint[] { points[1], points[2], points[3], points[0] });

                    marker.Points = points;
                }

                // 변형 복구
                var quadrilateralTransformation = new QuadrilateralTransformation(points,
                    _imageProcessorConfig.QuadrilateralTransformationWidth,
                    _imageProcessorConfig.QuadrilateralTransformationHeight);

                var transformed = quadrilateralTransformation.Apply(bitmap);
                

                // 회전 및 색상 판정 시작

                int halfWidth = _imageProcessorConfig.QuadrilateralTransformationWidth / 2,
                    halfHeight = _imageProcessorConfig.QuadrilateralTransformationHeight / 2;

                // x => x + 1 사분면
                var crops = new[]
                {
                    new Crop(new Rectangle(halfWidth, 0, halfWidth, halfHeight)),
                    new Crop(new Rectangle(0, 0, halfWidth, halfHeight)),
                    new Crop(new Rectangle(0, halfHeight, halfWidth, halfHeight)),
                    new Crop(new Rectangle(halfWidth, halfHeight, halfWidth, halfHeight))
                };

                var quadImage = new[]
                {
                    crops[0].Apply(transformed),
                    crops[1].Apply(transformed),
                    crops[2].Apply(transformed),
                    crops[3].Apply(transformed)
                };

                var filteredResult = new[] {
                    new {
                        Img = quadImage[0],
                        Red = Filter(quadImage[0], MarkerColor.Red).Luminance(),
                        Green = Filter(quadImage[0], MarkerColor.Green).Luminance(),
                        Blue = Filter(quadImage[0], MarkerColor.Blue).Luminance(),
                        White = Filter(quadImage[0], MarkerColor.White).Luminance()
                    },
                    new {
                        Img = quadImage[1],
                        Red = Filter(quadImage[1], MarkerColor.Red).Luminance(),
                        Green = Filter(quadImage[1], MarkerColor.Green).Luminance(),
                        Blue = Filter(quadImage[1], MarkerColor.Blue).Luminance(),
                        White = Filter(quadImage[1], MarkerColor.White).Luminance()
                    },
                    new {
                        Img = quadImage[2],
                        Red = Filter(quadImage[2], MarkerColor.Red).Luminance(),
                        Green = Filter(quadImage[2], MarkerColor.Green).Luminance(),
                        Blue = Filter(quadImage[2], MarkerColor.Blue).Luminance(),
                        White = Filter(quadImage[2], MarkerColor.White).Luminance()
                    },
                    new {
                        Img = quadImage[3],
                        Red = Filter(quadImage[3], MarkerColor.Red).Luminance(),
                        Green = Filter(quadImage[3], MarkerColor.Green).Luminance(),
                        Blue = Filter(quadImage[3], MarkerColor.Blue).Luminance(),
                        White = Filter(quadImage[3], MarkerColor.White).Luminance()
                    }
                };

                var whiteDesc = filteredResult.OrderByDescending(a => a.White).ToArray();

                if (rgb)
                {
                    // RGB 색상 판별
                    var colorQuad = whiteDesc.Skip(1);

                    var red = colorQuad.Sum(a => a.Red);
                    var green = colorQuad.Sum(a => a.Green);
                    var blue = colorQuad.Sum(a => a.Blue);

                    Console.WriteLine("{0}: {1} {2} {3}", colorQuad.Count(), red, green, blue);

                    var max = Math.Max(red, Math.Max(green, blue));

                    if (red == max)
                    {
                        marker.Color = MarkerColor.Red;
                    }
                    else if (green == max)
                    {
                        marker.Color = MarkerColor.Green;
                    }
                    else if (blue == max)
                    {
                        marker.Color = MarkerColor.Blue;
                    }
                }
                else
                {
                    // 흑백 색상 판별

                    var whiteMax = whiteDesc[0].White;
                    var whiteRest = (whiteDesc[1].White + whiteDesc[2].White + whiteDesc[3].White) / 3;

                    if (whiteMax - whiteRest < _imageProcessorConfig.ColorTestWhite)
                    {
                        // White
                        marker.Color = MarkerColor.White;
                    }
                    else
                    {
                        // Black
                        marker.Color = MarkerColor.Black;
                    }
                }

                // 회전 판별
                for (int i = 0; i < 4; i++)
                {
                    if (filteredResult[i].White == whiteDesc.First().White)
                    {
                        marker.Rotate = (MarkerRotate)(i + 1);
                        break;
                    }
                }

                // 백색 마커에는 회전 방향이 없습니다.
                if (marker.Color == MarkerColor.White)
                {
                    // 지정되지 않습니다.
                    marker.Rotate = MarkerRotate.None;
                }


                // 화상 중심으로 좌표 변환
                for (int i = 0; i < marker.Points.Count; i++)
                {
                    marker.Points[i] = new IntPoint
                    {
                        X = marker.Points[i].X - _configService.DeviceConfig.PixelWidth / 2,
                        Y = marker.Points[i].Y - _configService.DeviceConfig.PixelHeight / 2
                    };
                }

                // 코어 서비스에서 기하학적 방법으로 거리 계산
                var coreResult = _coreService.Query(marker.Points, _imageProcessorConfig.MarkerSize);
                marker.EuclideanDistance = coreResult.Distance;
                marker.TiltAngle = Math.Asin(coreResult.TranslationVector[1] / marker.EuclideanDistance);
                marker.PanAngle = Math.Asin(coreResult.TranslationVector[0] / marker.EuclideanDistance);
                if (marker.PanAngle > Math.PI)
                {
                    // 음수
                    marker.PanAngle = 2 * Math.PI - marker.PanAngle;
                }
                marker.TransX = coreResult.TranslationVector[0];
                marker.TransY = coreResult.TranslationVector[1];
                marker.TransZ = coreResult.TranslationVector[2];
            }

            BackgroundBitmap = overlay;

            Console.WriteLine();
            foreach (var marker in result.Markers)
            {
                Console.WriteLine(marker.Color);
            }
            
            return result;
        }


        Bitmap Filter(Bitmap bitmap, MarkerColor color)
        {
            var filter = new ChannelFiltering();
            filter.Red = filter.Green = filter.Blue = new IntRange(0, 0);

            switch (color)
            {
                case MarkerColor.Red:
                    filter.Red = new IntRange(0, 255);
                    break;

                case MarkerColor.Green:
                    filter.Green = new IntRange(0, 255);
                    break;

                case MarkerColor.Blue:
                    filter.Blue = new IntRange(0, 255);
                    break;

                case MarkerColor.White:
                    filter.Red = new IntRange(0, 255);
                    filter.Green = new IntRange(0, 255);
                    filter.Blue = new IntRange(0, 255);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return filter.Apply(bitmap);
        }

        void ApplyOverlay(BitmapData bitmapData, List<IntPoint> points)
        {
            Drawing.Polygon(bitmapData, points, Color.Yellow);

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    Drawing.Line(bitmapData, points[i], points[j], Color.Yellow);
                }
            }
        }

        bool MoreQuadTest(Blob blob, Marker marker, List<IntPoint> leftEdge, List<IntPoint> rightEdge)
        {
            var points = marker.Points;

            if (marker.Points.Distinct().Count() != 4)
            {
                return false;
            }

            double anglePair1 = GeometryTools.GetAngleBetweenLines(points[0], points[1], points[2], points[3]);
            double anglePair2 = GeometryTools.GetAngleBetweenLines(points[1], points[2], points[3], points[0]);

            if (anglePair1 <= _imageProcessorConfig.AngleError1 && anglePair2 <= _imageProcessorConfig.AngleError1)
            {
                double angle1 = GeometryTools.GetAngleBetweenVectors(points[0], points[3], points[1]);
                double angle2 = GeometryTools.GetAngleBetweenVectors(points[1], points[0], points[2]);
                double angle3 = GeometryTools.GetAngleBetweenVectors(points[2], points[1], points[3]);
                double angle4 = GeometryTools.GetAngleBetweenVectors(points[3], points[2], points[0]);

                if (Math.Abs(angle1 - 90) <= _imageProcessorConfig.AngleError2 &&
                    Math.Abs(angle2 - 90) <= _imageProcessorConfig.AngleError2 &&
                    Math.Abs(angle3 - 90) <= _imageProcessorConfig.AngleError2 &&
                    Math.Abs(angle4 - 90) <= _imageProcessorConfig.AngleError2)
                {
                    var length1 = points[0].DistanceTo(points[1]);
                    var length2 = points[1].DistanceTo(points[2]);
                    var length3 = points[2].DistanceTo(points[3]);
                    var length4 = points[3].DistanceTo(points[0]);

                    var max = Math.Max(length1, Math.Max(length2, Math.Max(length3, length4)));
                    var min = Math.Min(length1, Math.Min(length2, Math.Min(length3, length4)));

                    if (max / min <= _imageProcessorConfig.LengthError)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool BorderTest(Blob blob, Marker marker, Bitmap grayscale, List<IntPoint> leftEdge, List<IntPoint> rightEdge)
        {
            // 흑색 테두리 세부 판정에 사용되는 그레이 스케일 데이터
            var grayscaleData = grayscale.LockBits(new Rectangle(0, 0, grayscale.Width, grayscale.Height),
                ImageLockMode.ReadWrite, grayscale.PixelFormat);

            var grayscaleUnmanaged = new UnmanagedImage(grayscaleData);

            try
            {
                List<IntPoint>
                    left1 = new List<IntPoint>(),
                    left2 = new List<IntPoint>(),
                    right1 = new List<IntPoint>(),
                    right2 = new List<IntPoint>();

                int x1, x2, y, width = grayscaleUnmanaged.Width;

                int borderTestSize = _imageProcessorConfig.BorderTestSize;

                for (int i = 0; i < leftEdge.Count; i++)
                {
                    x1 = leftEdge[i].X - borderTestSize;
                    x2 = leftEdge[i].X + borderTestSize;
                    y = leftEdge[i].Y;

                    left1.Add(new IntPoint(x1 >= 0 ? x1 : 0, y));
                    left2.Add(new IntPoint(x2 < width ? x2 : width - 1, y));

                    x1 = rightEdge[i].X - borderTestSize;
                    x2 = rightEdge[i].X + borderTestSize;
                    y = rightEdge[i].Y;

                    right1.Add(new IntPoint(x1 >= 0 ? x1 : 0, y));
                    right2.Add(new IntPoint(x2 < width ? x2 : width - 1, y));
                }

                var left1Values = grayscaleUnmanaged.Collect8bppPixelValues(left1);
                var left2Values = grayscaleUnmanaged.Collect8bppPixelValues(left2);
                var right1Values = grayscaleUnmanaged.Collect8bppPixelValues(right1);
                var right2Values = grayscaleUnmanaged.Collect8bppPixelValues(right2);

                int count = 0;
                double difference = 0;

                for (int i = 0; i < leftEdge.Count; i++)
                {
                    if (rightEdge[i].X - leftEdge[i].X > borderTestSize * 2)
                    {
                        difference += left1Values[i] - left2Values[i];
                        difference += right2Values[i] - right1Values[i];
                        count++;
                    }
                }

                difference /= count * 2;

                if (difference > _imageProcessorConfig.BorderTestDifference)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                grayscale.UnlockBits(grayscaleData);
            }
        }
    }
}
