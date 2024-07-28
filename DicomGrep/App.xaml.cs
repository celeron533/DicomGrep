using DicomGrep.Services.Interfaces;
using DicomGrep.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DicomGrepCore.Services.Interfaces;
using DicomGrepCore.Services;

namespace DicomGrep
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Services = ConfigureServices();

            base.OnStartup(e);
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IDicomDictionaryLookupService, DicomDictionaryLookupService>();
            services.AddSingleton<IExportService, ExportService>();
            services.AddSingleton<IFileOperationService, FileOperationService>();
            services.AddSingleton<IFolderPickupService, FolderPickupService>();
            services.AddSingleton<ISopClassLookupService, SopClassLookupService>();
            services.AddSingleton<ITagValueDetailService, TagValueDetailService>();
            services.AddSingleton<ISearchService, SearchService>();
            services.AddSingleton<IDictionaryService, DictionaryService>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; private set; }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;
    }
}
