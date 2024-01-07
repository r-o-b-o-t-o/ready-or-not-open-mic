using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace ReadyOrNotOpenMic.Data
{
    public class Config
    {
        public Settings Settings { get; set; } = new();

        private void SetEvents()
        {
            Settings.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Save();
        }

        private void OnListChanged(object sender, ListChangedEventArgs e)
        {
            Save();
        }

        public static Config Load()
        {
            string path = GetSaveFilePath();
            Config cfg = null;
            if (File.Exists(path))
            {
                string contents = File.ReadAllText(path);
                try
                {
                    cfg = JsonConvert.DeserializeObject<Config>(contents, GetJsonSerializerSettings());
                }
                catch (Exception ex)
                {
                    // TODO: log exception
                }
            }
            cfg ??= new();
            cfg.SetEvents();
            return cfg;
        }

        public void Save()
        {
            Directory.CreateDirectory(GetSaveDirectory());
            string serialized = JsonConvert.SerializeObject(this, Formatting.Indented, GetJsonSerializerSettings());
            File.WriteAllText(GetSaveFilePath(), serialized);
        }

        public async Task SaveAsync()
        {
            Directory.CreateDirectory(GetSaveDirectory());
            string serialized = JsonConvert.SerializeObject(this, Formatting.Indented, GetJsonSerializerSettings());
            await File.WriteAllTextAsync(GetSaveFilePath(), serialized);
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
        }

        public static string GetSaveDirectory()
        {
            string appData = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            return Path.Combine(appData, "ReadyOrNotOpenMic");
        }

        public static string GetSaveFilePath()
        {
            return Path.Combine(GetSaveDirectory(), "config.json");
        }
    }
}
