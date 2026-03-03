using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitSyncService.Core.Models;
using RevitSyncService.Infrastructure.Database;

namespace RevitSyncService.Core.Services
{
    public class ConfigService
    {
        private DbRepository? _repository;
        private AppConfig _config = new();

        public AppConfig Config => _config;
        public string ConfigFilePath => "PostgreSQL: tables global_settings + projects";

        public bool Initialize(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return false;

            try
            {
                _repository = new DbRepository(connectionString);
                Task.Run(() => _repository.EnsureSchemaAsync()).GetAwaiter().GetResult();
                Load();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ConfigService] Init failed: {ex.Message}");
                _repository = null;
                return false;
            }
        }

        public void Load()
        {
            if (_repository == null) return;
            try
            {
                _config.GlobalSettings = _repository.LoadGlobalSettings();
                _config.Projects = _repository.LoadProjects();
                System.Diagnostics.Debug.WriteLine($"[ConfigService] Загружено проектов: {_config.Projects.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ConfigService] Ошибка Load: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ConfigService] Stack: {ex.StackTrace}");
                // Не падаем — оставляем пустой конфиг
                _config = new AppConfig();
            }
        }

        public void Save()
        {
            if (_repository == null) return;
            _repository.SaveGlobalSettings(_config.GlobalSettings);
            // Проекты сохраняются через UpsertProject/DeleteProject в ProjectManager
        }

        public DbRepository? Repository => _repository;

        public string GetExpandedTempFolder()
            => Environment.ExpandEnvironmentVariables(_config.GlobalSettings.TempFolder);

        public string GetExpandedLogFolder()
            => Environment.ExpandEnvironmentVariables(_config.GlobalSettings.LogFolder);
    }
}