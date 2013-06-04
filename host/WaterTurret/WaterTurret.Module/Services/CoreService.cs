using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using AForge;
using System.IO;
using WaterTurret.Module.Models;
using System.Runtime.InteropServices;
using OpenCvSharp;

namespace WaterTurret.Module.Services
{
    public class CoreService : ICoreService
    {
        CvMat
            _camera,
            _distortion,
            _object,
            _image,
            _rotation,
            _translation;

        public CoreService()
        {
            _camera = new CvMat(3, 3, MatrixType.F64C1);
            _distortion = new CvMat(5, 1, MatrixType.F64C1);
            _object = new CvMat(4, 3, MatrixType.F32C1);
            _image = new CvMat(4, 2, MatrixType.F32C1);
            _rotation = new CvMat(3, 1, MatrixType.F32C1);
            _translation = new CvMat(3, 1, MatrixType.F32C1);

            Setting();
            Test();
        }

        private void Test()
        {
            Query(new List<IntPoint>(new[]
            {
                new IntPoint{X=1, Y=1},
                new IntPoint{X=1, Y=2},
                new IntPoint{X=2, Y=2},
                new IntPoint{X=2, Y=1}
            }), 10);
        }

        void Setting()
        {            
            var cameraSetting = new[]
            {
               1.1695652991231043e+003,
               0,
               6.4021949743978928e+002, 
               0,
               1.1681614889356322e+003,
               3.9445679497160154e+002,
               0,
               0,
               1
            };

            var distortionSetting = new[]
            {
                 2.6841557745990408e-001,
                 -1.2536292326179452e+000,
                 -5.1949672285252447e-003, 
                 3.4873826852522883e-003,
                 1.6164774281680905e+000
            };
            

            /*
            var cameraSetting = new[]
            {
                1.1442471246455027e+003, 
                0,
                6.4812020158754228e+002,
                0,
                1.1443391554606812e+003,
                3.8794788696276186e+002, 
                0,
                0, 
                1
            };

            var distortionSetting = new[]
            {
                  2.0156935445340091e-001,
                  -1.0814314304293935e+000,
                  -6.7653166226057113e-003,
                  3.7355841949211124e-003,
                  1.7023014402148473e+000
            };
            */

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    _camera.Set2D(i, j, cameraSetting[i * 3 + j]);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                _distortion.Set2D(i, 0, distortionSetting[i]);
            }
        }

        ~CoreService()
        {
        }

        public CoreResult Query(List<IntPoint> points, double markerSize)
        {
            Console.WriteLine("Query");
            lock (this)
            {
                var result = new CoreResult();

                // 마커 정보
                var half = (float)(markerSize / 2);

                var objectMatrix = new[]
                {
                    half, half, 0,
                    half, -half, 0,
                    -half, -half, 0,
                    -half, half, 0
                };

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        _object.Set2D(i, j, objectMatrix[i * 3 + j]);
                    }
                }

                // 이미지 정보
                for (int i = 0; i < 4; i++)
                {
                    _image.Set2D(i, 0, points[i].X);
                    _image.Set2D(i, 1, points[i].Y);
                }

                try
                {
                    Console.WriteLine("Find");
                    CvInvoke.cvFindExtrinsicCameraParams2(
                        _object.CvPtr, _image.CvPtr, _camera.CvPtr, _distortion.CvPtr,
                        _rotation.CvPtr, _translation.CvPtr, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // 결과
                result.RotationVector = new List<float>(3);
                result.TranslationVector = new List<float>(3);

                for (int i = 0; i < 3; i++)
                {
                    result.RotationVector.Add((float)_rotation.Get2D(i, 0).Val0);
                    result.TranslationVector.Add((float)_translation.Get2D(i, 0).Val0);
                }
                
                result.Distance = Math.Sqrt(
                    result.TranslationVector[0] * result.TranslationVector[0] +
                    result.TranslationVector[1] * result.TranslationVector[1] +
                    result.TranslationVector[2] * result.TranslationVector[2]);
                
                Console.WriteLine("Diatance : {0}", result.Distance);

                return result;
            }
        }
    }
}
