using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace UPMKits
{
    [Serializable]
    public class ConfigItem
    {
        public string Username;
        public bool IsDeveloper;
        public string Token;
        public List<string> Scopes = new List<string>();

        public string GetScopesOverview()
        {
            var context = "*";
            if (Scopes == null)
            {
                return context;
            }

            context = "";
            for (int i = 0; i < Scopes.Count; i++)
            {
                context += Scopes[i];
                if (i < Scopes.Count - 1)
                {
                    context += "|";
                }
            }

            return context;
        }

        public bool AddScope(string scope)
        {
            if (Scopes.Contains(scope))
            {
                Debug.LogError($"{scope} was already exist");
                return false;
            }

            Scopes.Add(scope);
            return true;
        }

        public bool RemoveScope(string scope)
        {
            if (!Scopes.Contains(scope))
            {
                Debug.LogError($"{scope} is not exist");
                return false;
            }

            Scopes.Remove(scope);
            return true;
        }

        public bool ModifyScope(string old, string scope)
        {
            if (!Scopes.Contains(old))
            {
                Debug.LogError($"{old} is not exist");
                return false;
            }

            var index = Scopes.IndexOf(old);
            Scopes[index] = scope;
            return true;
        }
    }

    public class UECConfigModel
    {
        #region Static

        private const string UECConfigFile = ".uecconfig";

        public static string UECConfigPath => Path.Combine(UserProfilePath(), UECConfigFile);

        public static string UserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        #endregion

        private List<ConfigItem> _items;

        public UECConfigModel()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (HasConfig() == false)
            {
                return;
            }

            var json = "";
            using (var reader = new StreamReader(UECConfigPath, System.Text.Encoding.Default))
            {
                json = reader.ReadToEnd();
                reader.Close();
            }

            _items = JsonConvert.DeserializeObject<List<ConfigItem>>(json) ?? new List<ConfigItem>();
        }

        public bool HasConfig()
        {
            return File.Exists(UECConfigPath);
        }

        public string GetTokenByUsername(string username)
        {
            var items = GetItems();

            var ret = items.Select(item => item).Where(item => item.Username == username);

            return ret.Any() ? ret.First().Token : null;
        }

        public List<ConfigItem> GetItems()
        {
            return _items;
        }
    }
}