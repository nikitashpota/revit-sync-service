using RevitSyncService.Core.Services;

namespace RevitSyncService.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ProjectsViewModel ProjectsVM { get; }
        public LogViewModel LogVM { get; }
        public DatabaseConnectionService DbConnectionService { get; }

        public MainViewModel(ProjectsViewModel projectsVm, LogViewModel logVm,
            DatabaseConnectionService dbConnectionService)
        {
            ProjectsVM = projectsVm;
            LogVM = logVm;
            DbConnectionService = dbConnectionService;
        }
    }
}