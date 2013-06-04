using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WaterTurret.Module.Models;
using WaterTurret.Module.Services;
using System.Drawing;
using Microsoft.Practices.ServiceLocation;

namespace WaterTurret.Module.Controls
{
    /// <summary>
    /// XAML 파일에서 이 사용자 지정 컨트롤을 사용하려면 1a 또는 1b단계를 수행한 다음 2단계를 수행하십시오.
    ///
    /// 1a단계) 현재 프로젝트에 있는 XAML 파일에서 이 사용자 지정 컨트롤 사용.
    /// 이 XmlNamespace 특성을 사용할 마크업 파일의 루트 요소에 이 특성을 
    /// 추가합니다.
    ///
    ///     xmlns:MyNamespace="clr-namespace:WaterTurret.Module.Controls"
    ///
    ///
    /// 1b단계) 다른 프로젝트에 있는 XAML 파일에서 이 사용자 지정 컨트롤 사용.
    /// 이 XmlNamespace 특성을 사용할 마크업 파일의 루트 요소에 이 특성을 
    /// 추가합니다.
    ///
    ///     xmlns:MyNamespace="clr-namespace:WaterTurret.Module.Controls;assembly=WaterTurret.Module.Controls"
    ///
    /// 또한 XAML 파일이 있는 프로젝트의 프로젝트 참조를 이 프로젝트에 추가하고
    /// 다시 빌드하여 컴파일 오류를 방지해야 합니다.
    ///
    ///     솔루션 탐색기에서 대상 프로젝트를 마우스 오른쪽 단추로 클릭하고
    ///     [참조 추가]->[프로젝트]를 차례로 클릭한 다음 이 프로젝트를 찾아서 선택합니다.
    ///
    ///
    /// 2단계)
    /// 계속 진행하여 XAML 파일에서 컨트롤을 사용합니다.
    ///
    ///     <MyNamespace:VisualizationControl/>
    ///
    /// </summary>
    public class VisualizationControl : Control
    {
        static VisualizationControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VisualizationControl), new FrameworkPropertyMetadata(typeof(VisualizationControl)));
        }

        public VisualizationControl()
        {
            ImageProcessorService = ServiceLocator.Current.GetInstance<IImageProcessorService>();
            TurretService = ServiceLocator.Current.GetInstance<ITurretService>();
            ConfigService = ServiceLocator.Current.GetInstance<IConfigService>();
        }



        public IConfigService ConfigService
        {
            get { return (IConfigService)GetValue(ConfigServiceProperty); }
            set { SetValue(ConfigServiceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConfigService.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigServiceProperty =
            DependencyProperty.Register("ConfigService", typeof(IConfigService), typeof(VisualizationControl), new UIPropertyMetadata());

        

        public IImageProcessorService ImageProcessorService
        {
            get { return (IImageProcessorService)GetValue(ImageProcessorServiceProperty); }
            set { SetValue(ImageProcessorServiceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageProcessorService.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProcessorServiceProperty =
            DependencyProperty.Register("ImageProcessorService", typeof(IImageProcessorService), typeof(VisualizationControl), new UIPropertyMetadata());

        public ITurretService TurretService
        {
            get { return (ITurretService)GetValue(TurretServiceProperty); }
            set { SetValue(TurretServiceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TurretService.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TurretServiceProperty =
            DependencyProperty.Register("TurretService", typeof(ITurretService), typeof(VisualizationControl), new UIPropertyMetadata());
    }
}
