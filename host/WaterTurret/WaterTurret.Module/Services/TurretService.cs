using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaterTurret.Module.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using AForge.Math.Geometry;
using AForge;

namespace WaterTurret.Module.Services
{
    public class TurretService : NotificationObject, ITurretService
    {
        private readonly IImageProcessorService _imageProcessorService;
        private readonly ICameraService _cameraService;
        private readonly IConfigService _configService;
        private readonly IDeviceService _deviceService;
        private readonly IPlantService _plantService;
        private readonly IPhysicsService _measurementService;

        AutoResetEvent _exitEvent = new AutoResetEvent(false);

        public bool Mono { get; set; }
        public object ImageProcessorLock { get; set; }
        public ImageProcessorResult ImageProcessorResult { get; set; }
        public DateTime ImageProcessed { get; set; }

        public void ImageProcess()
        {
            _cameraService.WaitNewFrame();
            lock (ImageProcessorLock)
            {
                ImageProcessorResult = _imageProcessorService.Process(_cameraService.CurrentFrame, !Mono);
                ImageProcessed = DateTime.Now;
            }
        }

        public DateTime ImageProcess(DateTime time)
        {
            _cameraService.WaitNewFrame();
            lock (ImageProcessorLock)
            {
                if (ImageProcessed == time)
                {
                    ImageProcessorResult = _imageProcessorService.Process(_cameraService.CurrentFrame, !Mono);
                    ImageProcessed = DateTime.Now;
                }
                return ImageProcessed;
            }
        }

        public TurretService(IImageProcessorService imageProcessorService, ICameraService cameraService,
            IConfigService configService, IDeviceService deviceService, IPlantService plantService,
            IPhysicsService measurementService)
        {
            _imageProcessorService = imageProcessorService;
            _cameraService = cameraService;
            _configService = configService;
            _deviceService = deviceService;
            _plantService = plantService;
            _measurementService = measurementService;

            ImageProcessorLock = new object();

            // 이미지 프로세싱 작업
            Task.Factory.StartNew(new Action(() =>
                {
                    for (; ; )
                    {
                        lock (ImageProcessorLock)
                        {
                            if ((DateTime.Now - ImageProcessed).TotalMilliseconds >= 50)
                            {
                                ImageProcess();
                            }
                        }
                        Thread.Sleep(30);
                    }
                }));

            _tokenSource = new CancellationTokenSource();
            _exitEvent.Set();
        }

        CancellationTokenSource _tokenSource;

        ModeBase _currentMode;

        private TurretServiceMode _mode;

        public TurretServiceMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                if (_mode != value)
                {
                    // 이전 모드 작업 취소
                    _tokenSource.Cancel();
                    _exitEvent.WaitOne();

                    // 새로운 작업에 사용할 토큰
                    _tokenSource = new CancellationTokenSource();

                    _mode = value;
                    switch (_mode)
                    {
                        case TurretServiceMode.Standby:
                            Standby();
                            break;

                        case TurretServiceMode.Watering:
                            Watering();
                            break;

                        case TurretServiceMode.Cleaning:
                            Cleaning();
                            break;

                        case TurretServiceMode.Manual:
                            Manual();
                            break;
                    }

                    RaisePropertyChanged(() => Mode);
                    RaisePropertyChanged(() => ModeDescription);
                }
            }
        }

        public string ModeDescription
        {
            get
            {
                switch (Mode)
                {
                    case TurretServiceMode.Standby:
                        return "대기 모드";

                    case TurretServiceMode.Watering:
                        return "관수 모드";

                    case TurretServiceMode.Cleaning:
                        return "청소 모드";

                    case TurretServiceMode.Manual:
                        return "수동 모드";
                }

                return "모드 오류";
            }
        }

        void Standby()
        {
            _currentMode = new StandbyMode();
            _currentMode.Start(_exitEvent, _tokenSource.Token);
        }

        void Watering()
        {
            _currentMode = new WateringMode();
            _currentMode.Start(_exitEvent, _tokenSource.Token);
        }

        void Cleaning()
        {
            _currentMode = new CleaningMode();
            _currentMode.Start(_exitEvent, _tokenSource.Token);
        }

        void Manual()
        {
            _currentMode = new ManualMode();
            _currentMode.Start(_exitEvent, _tokenSource.Token);
        }
    }

    public abstract class ModeBase
    {
        AutoResetEvent _exitEvent;
        CancellationToken _token;

        protected readonly IDeviceService _deviceService;
        protected readonly ITurretService _turretService;
        protected readonly IConfigService _configService;
        protected readonly IPlantService _plantService;
        protected readonly IPhysicsService _physicsService;

        public ModeBase()
        {
            _deviceService = ServiceLocator.Current.GetInstance<IDeviceService>();
            _turretService = ServiceLocator.Current.GetInstance<ITurretService>();
            _configService = ServiceLocator.Current.GetInstance<IConfigService>();
            _plantService = ServiceLocator.Current.GetInstance<IPlantService>();
            _physicsService = ServiceLocator.Current.GetInstance<IPhysicsService>();
        }

        public void Track(Marker targetMarker)
        {
            for (; ; CheckCancelAndImageProcess())
            {
                // Tracking
                Stopwatch failStopwatch = new Stopwatch();

                // 이동 멈춤 대기
                SpinCheck(() => _deviceService.PanRunning() == false);

                CheckCancelAndImageProcess();
                for (; ; CheckCancelAndImageProcess())
                {
                    var targetRecognized = from m in _result.Markers
                                           where m.Color == targetMarker.Color
                                           select m;

                    if (targetRecognized.Count() == 0)
                    {
                        Console.WriteLine("실패");

                        // 인식 실패
                        if (failStopwatch.IsRunning == false)
                        {
                            failStopwatch.Start();
                        }

                        // 추적 포기
                        if (failStopwatch.ElapsedMilliseconds > 3500)
                        {
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("성공");

                        // 추적 중
                        failStopwatch.Reset();

                        var target = targetRecognized.First();
                        
                        bool
                            xOk = false,
                            yOk = false;

                        var x = target.X;
                        var y = target.Y;

                        var xa = Math.Abs(x);
                        var ya = Math.Abs(y);

                        target.TransX *= 1000;
                        target.TransY *= 1000;
                        target.TransZ *= 1000;

                        /*
                        var angleX = GeometryTools.GetAngleBetweenVectors(
                            new IntPoint(0, 0), new IntPoint((int)target.TransZ, (int)target.TransX), new IntPoint((int)target.TransZ, 0));

                        var angleY = GeometryTools.GetAngleBetweenVectors(
                            new IntPoint(0, 0), new IntPoint((int)target.TransZ, (int)target.TransY), new IntPoint((int)target.TransZ, 0));

                        if (Math.Abs(angleX) > 5)
                        {
                            if (target.TransX < 0)
                            {
                                angleX *= -1;
                            }
                            var add = _physicsService.PanAngleToStep(_physicsService.DegreeToRadian(angleX));
                            _deviceService.PanAdd(add, 0);
                        }
                        else
                        {
                            xOk = true;
                        }
                        */

                        
                        if (xa > 50)
                        {
                            _deviceService.PanAdd((short)(x > 0 ? 40 : -40), 0);
                        }
                        else if (xa > 15)
                        {
                            _deviceService.PanAdd((short)(x > 0 ? 5 : -5), 0);
                        }
                        else
                        {
                            xOk = true;
                        }

                        if (ya > 50)
                        {
                            _deviceService.TiltCameraAdd((short)(y > 0 ? 6 : -6));
                        }
                        else if (ya > 15)
                        {
                            _deviceService.TiltCameraAdd((short)(y > 0 ? 2 : -2));
                        }
                        else
                        {
                            yOk = true;
                        }
                        
                        // 추적 성공
                        if (xOk && yOk)
                        {
                            CheckCancel();

                            // Watering
                            var result = _physicsService.Measure(
                                target,
                                _deviceService.TiltCameraGet(),
                                _deviceService.PanGet(), false);

                            _deviceService.TiltNozzleSet(result.EstimatedNozzleWidth);
                            _deviceService.PanSet(result.EstimatedPanStep, 0);

                            double timeFactor = 1.0;

                            switch (target.Rotate)
                            {
                                case MarkerRotate.Quad1: break;
                                case MarkerRotate.Quad2: timeFactor *= 2; break;
                                case MarkerRotate.Quad3: timeFactor *= 0.5; break;
                                case MarkerRotate.Quad4: timeFactor *= 0; break;
                            }

                            var plant = (from p in _plantService.GetModel()
                                         where p.MarkerColor == target.Color
                                         select p).First();

                            TimeSpan plantWaterTime = new TimeSpan((long)(plant.WaterTime.Ticks * timeFactor));

                            // 살수 안함
                            if (plantWaterTime.Ticks == 0)
                            {
                                plant.WateredNow();
                            }
                            else
                            {
                                // 이동 멈춤 대기
                                SpinCheck(() => _deviceService.PanRunning() == false);

                                // 살수 개시
                                _deviceService.WaterOn();

                                // 지연
                                SpinCheckSleep((long)(plantWaterTime.TotalMilliseconds));

                                // 살수 중단
                                _deviceService.WaterOff();
                                plant.WateredNow();

                                Thread.Sleep(1000);
                            }

                            return;
                        }
                        else
                        {
                            // 추적 실패

                            // 멈춤 대기
                            SpinCheck(() => _deviceService.PanRunning() == false);
                        }
                    }
                }
            }
        }

        public void Start(AutoResetEvent exitEvent, CancellationToken token)
        {
            _exitEvent = exitEvent;
            _token = token;

            Task.Factory.StartNew((obj) =>
                {
                    try
                    {
                        CheckCancel();

                        _deviceService.Open();
                        Reset();

                        CheckCancel();

                        // 최초 영상 처리
                        CheckCancelAndImageProcess();

                        // 무한 루프
                        Loop();
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("catch");
                    }
                    finally
                    {
                        Reset();
                        _deviceService.Close();
                        _exitEvent.Set();
                    }
                }, _token);
        }

        DateTime _last = DateTime.MinValue;
        public ImageProcessorResult _result { protected get; set; }

        protected void SpinCheck(Func<bool> func)
        {
            while (func.Invoke() == false)
            {
                CheckCancel();
                Thread.Sleep(10);
            }
        }


        protected void SpinCheckSleep(long ms)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            SpinCheck(() => stopwatch.ElapsedMilliseconds >= ms);
        }

        protected void CheckCancelAndImageProcess()
        {
            // 종료 검사
            CheckCancel();

            // 반드시 새로운 이미지 프로세싱이 필요한 경우
            _last = _turretService.ImageProcess(_last);

            // 이미지 프로세싱 결과
            lock (_turretService.ImageProcessorLock)
            {
                _result = _turretService.ImageProcessorResult;
            }
        }

        protected IEnumerable<Plant> GetWaterRequiredPlants()
        {
            // 살수가 필요한 식물 목록
            return from p in _plantService.GetModel()
                   where p.CheckWaterRequired()
                   select p;

        }

        protected void CheckCancel()
        {
            _token.ThrowIfCancellationRequested();
        }

        protected void Reset()
        {
            Console.WriteLine("Reset 1");
            _deviceService.DeviceCheck();
            _deviceService.ValveOff();
            _deviceService.PumpOff();
            _deviceService.PanSet(0, 0);
            _deviceService.TiltCameraSet(_configService.DeviceConfig.TiltCameraDefault);
            _deviceService.TiltNozzleSet(_configService.DeviceConfig.TiltNozzleDefault);
            _deviceService.DeviceCheck();
            Console.WriteLine("Reset 2");
        }

        protected abstract void Loop();
    }

    public class StandbyMode : ModeBase
    {
        protected override void Loop()
        {
            for (; ; )
            {
                CheckCancel();
                Thread.Sleep(10);
            }
        }
    }

    public class WateringMode : ModeBase
    {
        protected override void Loop()
        {
            _turretService.Mono = false;

            bool completed = false;

            if (completed == true)
            {
                completed = false;
            }

            for (; ; CheckCancelAndImageProcess())
            {
                completed = false;

                // Idle
                for (; ; CheckCancelAndImageProcess())
                {
                    // 살수 필요 검사
                    if (GetWaterRequiredPlants().Count() != 0)
                    {
                        break;
                    }
                }

                // Searching 시작을 위해 기본값으로 이동
                _deviceService.TiltNozzleSet(_configService.DeviceConfig.TiltNozzleDefault);
                _deviceService.TiltCameraSet((short)(_configService.DeviceConfig.TiltCameraDefault + 130));
                _deviceService.PanSet((short)(_configService.DeviceConfig.PanMin), 0);
                
                CheckCancelAndImageProcess();
                for (var i = (short)(_configService.DeviceConfig.PanMin);
                    i <= _configService.DeviceConfig.PanMax;
                    i += 120)
                {
                    for (var j = (short)(_configService.DeviceConfig.TiltCameraDefault + 130);
                        _configService.DeviceConfig.TiltCameraMin <= j;
                        j -= 55)
                    {
                        // 이동
                        _deviceService.TiltCameraSet(j);
                        _deviceService.PanSet(i, 0);


                        // 화면 잔상 제거
                        SpinCheckSleep(600);
                        CheckCancelAndImageProcess();

                        // 마커 검사
                        foreach (var marker in _result.Markers)
                        {
                            var match = from p in GetWaterRequiredPlants()
                                        where marker.Color == p.MarkerColor
                                        select p;

                            if (match.Count() != 0)
                            {
                                // 추적 도움
                                _deviceService.TiltCameraAdd(-30);
                                _deviceService.PanAdd(50, 0);

                                Track(marker);

                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public class CleaningMode : ModeBase
    {

        protected override void Loop()
        {

            _turretService.Mono = true;

            bool completed = false;

            Marker m1 = null;

            bool w = false,
                b = false;

            PhysicsResult p1 = null;

            for (; ; CheckCancelAndImageProcess())
            {
                completed = false;

                // Idle
                for (; ; CheckCancelAndImageProcess())
                {
                    break;
                }

                // Searching 시작을 위해 기본값으로 이동
                _deviceService.TiltNozzleSet(_configService.DeviceConfig.TiltNozzleDefault);
                _deviceService.TiltCameraSet((short)(_configService.DeviceConfig.TiltNozzleDefault));
                _deviceService.PanSet((short)(1), 0);

                Marker targetMarker = null;


                CheckCancelAndImageProcess();
                for (var i = _configService.DeviceConfig.TiltNozzleDefault;
                    i <= _configService.DeviceConfig.TiltCameraMax;
                    i += 60)
                {
                    for (short j = 0;
                        j <= _configService.DeviceConfig.PanMax;
                        j += 200)
                    {
                        // 이동
                        _deviceService.TiltCameraSet(i);
                        _deviceService.PanSet(j, 0);

                        // 화면 잔상 제거
                        SpinCheckSleep(1500);
                        CheckCancelAndImageProcess();

                        // 마커 검사
                        foreach (var marker in _result.Markers)
                        {
                            var match = from p in GetWaterRequiredPlants()
                                        where (marker.Color == MarkerColor.Black && !b) ||
                                        (marker.Color == MarkerColor.White && !w)
                                        select p;

                            if (match.Count() != 0)
                            {
                                // 추적 도움
                                // TODO 방향성
                                _deviceService.TiltCameraAdd(5);
                                _deviceService.PanAdd(10, 0);

                                targetMarker = marker;
                                break;
                            }
                        }

                        if (targetMarker != null)
                            break;
                    }
                    if (targetMarker != null)
                        break;
                }

                if (targetMarker == null)
                {
                    // 처음부터 다시 시작
                    completed = true;
                    continue;
                }

                for (; completed == false; CheckCancelAndImageProcess())
                {
                    Track(targetMarker);

                    // Tracking

                    Stopwatch failStopwatch = new Stopwatch();

                    // 이동 멈춤 대기
                    SpinCheck(() => _deviceService.PanRunning() == false);

                    CheckCancelAndImageProcess();
                    for (; completed == false; CheckCancelAndImageProcess())
                    {
                        var targetRecognized = from m in _result.Markers
                                               where m.Color == targetMarker.Color
                                               select m;

                        if (targetRecognized.Count() == 0)
                        {
                            Console.WriteLine("실패");

                            // 인식 실패
                            if (failStopwatch.IsRunning == false)
                            {
                                failStopwatch.Start();
                            }

                            // 추적 포기
                            if (failStopwatch.ElapsedMilliseconds > 3000)
                            {
                                completed = true;
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("성공");

                            // 추적 중
                            failStopwatch.Reset();

                            var target = targetRecognized.First();

                            bool
                                xOk = false,
                                yOk = false;

                            var x = target.X;
                            var y = target.Y;

                            var xa = Math.Abs(x);
                            var ya = Math.Abs(y);

                            if (xa > 50)
                            {
                                if (xa * 1.2 > 80)
                                {
                                    _deviceService.PanAdd((short)(x > 0 ? 80 : -80), 0);
                                }
                                else
                                {
                                    _deviceService.PanAdd((short)(x * 1.2), 0);
                                }
                            }
                            else if (xa > 15)
                            {
                                _deviceService.PanAdd((short)(x > 0 ? 5 : -5), 0);
                            }
                            else
                            {
                                xOk = true;
                            }

                            if (ya > 50)
                            {
                                _deviceService.TiltCameraAdd((short)(y > 0 ? 6 : -6));
                            }
                            else if (ya > 15)
                            {
                                _deviceService.TiltCameraAdd((short)(y > 0 ? 2 : -2));
                            }
                            else
                            {
                                yOk = true;
                            }

                            // 추적 성공
                            if (xOk && yOk)
                            {
                                CheckCancel();

                                if (targetMarker.Color == MarkerColor.White)
                                {
                                    w = true;
                                }
                                else
                                {
                                    b = true;
                                }

                                if (m1 == null)
                                {
                                    m1 = target;
                                    p1 = _physicsService.Measure(m1,
                                        _deviceService.TiltCameraGet(),
                                        _deviceService.PanGet(), true);
                                    completed = true;
                                    break;
                                }
                                else
                                {
                                    var m2 = target;
                                    var p2 = _physicsService.Measure(m2,
                                        _deviceService.TiltCameraGet(),
                                        _deviceService.PanGet(), true);


                                    short leftStep = Math.Min(p1.EstimatedPanStep, p2.EstimatedPanStep);
                                    short rightStep = Math.Max(p1.EstimatedPanStep, p2.EstimatedPanStep);

                                    // inv
                                    short topWidth = Math.Min(p1.EstimatedNozzleWidth, p2.EstimatedNozzleWidth);
                                    short bottomWidth = Math.Max(p1.EstimatedNozzleWidth, p2.EstimatedNozzleWidth);

                                    while (true)
                                    {
                                        Console.WriteLine("{0} {1}", leftStep, rightStep);
                                        Console.WriteLine("{0} {1}", topWidth, bottomWidth);
                                        Console.WriteLine();
                                        break;
                                    }
                                    /*
                                    lock (_turretService.ImageProcessorLock)
                                    {
                                        Thread.Sleep(100000);
                                    }
                                    */
                                    _deviceService.WaterOn();

                                    _deviceService.TiltCameraSet(short.MinValue);

                                    for (int i = leftStep; i <= rightStep; i += 60)
                                    {
                                        for (int j = topWidth; j <= bottomWidth; j += 3)
                                        {
                                            _deviceService.PanSet((short)i, 0);
                                            _deviceService.TiltNozzleSet((short)j);
                                            Thread.Sleep(20);
                                            CheckCancel();
                                        }
                                    }

                                    _deviceService.WaterOff();

                                    m1 = null;
                                    completed = true;
                                    break;

                                    // 수평 분해
                                    for (int i = 0; i < 1000; i++)
                                    {
                                        if (m2.HorizontalDistance > m1.HorizontalDistance)
                                        {
                                            var temp = m2.HorizontalDistance;
                                            m2.HorizontalDistance = m1.HorizontalDistance;
                                            m1.HorizontalDistance = temp;
                                        }

                                        double distanceDiff = m2.HorizontalDistance - m1.HorizontalDistance;
                                        double distance = m1.HorizontalDistance + (distanceDiff / 1000) * i;

                                        if (p2.EstimatedPanStep < p1.EstimatedPanStep)
                                        {
                                            var temp = p2.EstimatedPanStep;
                                            p2.EstimatedPanStep = p1.EstimatedPanStep;
                                            p1.EstimatedPanStep = temp;
                                        }

                                        double panDiff = p2.EstimatedPanStep - p1.EstimatedPanStep;
                                        double pan = p1.EstimatedPanStep + (panDiff / 1000) * i;

                                        _deviceService.PanSet((short)pan, 0);

                                        // 수직 분해
                                        for (int j = 0; j < 1000; j++)
                                        {
                                            if (m2.Height < m1.Height)
                                            {
                                                var temp = m2.Height;
                                                m2.Height = m1.Height;
                                                m1.Height = temp;
                                            }

                                            double hDiff = m2.Height - m1.Height;
                                            double height = m1.Height + (hDiff / 1000) * i;

                                            double angle = _physicsService.WaterAngle(distance, height, true);
                                            _deviceService.TiltCameraSet(_physicsService.NozzleAngleToWidth(angle));


                                        }
                                    }

                                    _deviceService.WaterOff();

                                    m1 = null;
                                    completed = true;
                                    break;
                                }
                            }
                            else
                            {
                                // 추적 실패

                                // 멈춤 대기
                                SpinCheck(() => _deviceService.PanRunning() == false);
                            }
                        }
                    }
                }
            }
        }
    }

    public class ManualMode : ModeBase
    {
        protected override void Loop()
        {
            _turretService.Mono = false;

            for (; ; )
            {
                CheckCancel();
                Thread.Sleep(10);
            }
        }
    }
}
