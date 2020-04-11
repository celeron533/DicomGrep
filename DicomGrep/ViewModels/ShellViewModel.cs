using Caliburn.Micro;
using DicomGrep.ViewModels.Interfaces;

namespace DicomGrep.ViewModels
{
    public class ShellViewModel : PropertyChangedBase, IShell
    {
        public IMainViewModel MainViewModel { get; }

        public ShellViewModel(IMainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;
        }
    }
}