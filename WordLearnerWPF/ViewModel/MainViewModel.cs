using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using WordLearnerWPF.Core.Abstract;
using WordLearnerWPF.Params.Abstract;
using WordLearnerWPF.Services.Abstract;

namespace WordLearnerWPF.ViewModel
{
    public class MainViewModel : CoreViewModel
    {
        private ICoreNavigationServie _navigationServie;
        private IStaticParams _staticParams;
        public MainViewModel(ICoreNavigationServie coreNavigationServie, IStaticParams staticParams)
        {
            _navigationServie = coreNavigationServie ?? throw new ArgumentNullException(nameof(coreNavigationServie));
            _staticParams = staticParams ?? throw new ArgumentNullException(nameof(staticParams));
        }

       
        
        public override Task Initialize()
        {
            CreateFolders();
            _navigationServie.NavigateTo("HomeView");

            return Task.FromResult(1);
        }

        private void CreateFolders()
        {
            var folders = _staticParams.FoldersToCreate;
            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }

    }
}