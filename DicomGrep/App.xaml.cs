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
            
            var services = new ServiceCollection();
            ConfigServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigServices(IServiceCollection services)
        {

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

            services.AddTransient<MainWindow>();
        }

        public IServiceProvider ServiceProvider { get; private set; }
    }
}
