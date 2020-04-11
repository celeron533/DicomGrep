namespace DicomGrep {
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using DicomGrep.Services;
    using DicomGrep.Services.Interfaces;
    using DicomGrep.ViewModels;
    using DicomGrep.ViewModels.Interfaces;

    public class AppBootstrapper : BootstrapperBase {
        SimpleContainer container;

        public AppBootstrapper() {
            Initialize();
        }

        protected override void Configure() {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();

            // service
            container.Singleton<ISearchService, SearchService>();

            // shell
            container.Singleton<IShell, ShellViewModel>();
            container.Singleton<IMainViewModel, MainViewModel>();
            container.Singleton<INotifyIconService, NotifyIconService>();

        }

        protected override object GetInstance(Type service, string key) {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance) {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e) {
            DisplayRootViewFor<IShell>();
        }
    }
}