using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace WaterTurret.Common
{
    public abstract class NavigationViewModel : NotificationObject, INavigationAware, IRegionMemberLifetime
    {
        public abstract string RegionName { get; }

        protected abstract void Selected();
        protected abstract void UnSelected();

        SubscriptionToken _subscriptionToken;

        public NavigationViewModel()
        {
            // Subscribe to Composite Presentation Events
            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            var navigationCompletedEvent = eventAggregator.GetEvent<NavigationCompletedEvent>();
            _subscriptionToken = navigationCompletedEvent.Subscribe(OnNavigationCompleted, ThreadOption.PublisherThread);
        }

        private void OnNavigationCompleted(NavigationViewModel publisher)
        {
            if (publisher.RegionName == this.RegionName)
            {
                if (publisher != this)
                {
                    var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    var navigationCompletedEvent = eventAggregator.GetEvent<NavigationCompletedEvent>();
                    navigationCompletedEvent.Unsubscribe(_subscriptionToken);
                    UnSelected();
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private bool _selected = false;

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Publish
            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            var navigationCompletedEvent = eventAggregator.GetEvent<NavigationCompletedEvent>();
            navigationCompletedEvent.Publish(this);

            if (_selected == false)
            {
                _selected = true;
                Selected();
            }
        }

        public bool KeepAlive
        {
            get { return false; }
        }
    }
}
