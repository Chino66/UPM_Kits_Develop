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

        private List<ConfigItem> Items => _items;

        public bool IsDirty { get; set; } = false;

        public UECConfigModel()
        {
            LoadConfig();
        }

        private void LoadConfig()

        {
            var file = new FileInfo(UECConfigPath);

            if (file.Exists == false)
            {
                CreateConfig();
            }

            var json = "";
            using (var reader = new StreamReader(UECConfigPath, System.Text.Encoding.Default))
            {
                json = reader.ReadToEnd();
                reader.Close();
            }

            _items = JsonConvert.DeserializeObject<List<ConfigItem>>(json) ?? new List<ConfigItem>();
        }

        public string GetTokenByUsername(string username)
        {
            var items = GetItems();

            var ret = items.Select(item => item).Where(item => item.Username == username);

            return ret.Any() ? ret.First().Token : null;
        }

        public int GetItemsCount()
        {
            return _items.Count;
        }

        private bool AddItem(string username, string token, List<string> scopes)
        {
            // todo check
            var item = _items.Where(i => i.Username == username).Select(i => i);

            // 已经存在同名username不能添加
            if (item.Any())
            {
                Debug.LogError($"已经存在{username}不能添加");
                return false;
            }

            var ci = new ConfigItem()
            {
                Username = username,
                Token = token,
                Scopes = scopes
            };

            _items.Add(ci);
            IsDirty = true;
            return true;
        }

        public bool RemoveItem(string username)
        {
            var item = _items.Where(i => i.Username == username).Select(i => i);
            if (!item.Any())
            {
                return false;
            }

            _items.Remove(item.First());
            IsDirty = true;
            return true;
        }

        public bool ModifyItem(string username, string token, List<string> scopes)
        {
            var has = false;
            foreach (var item in _items)
            {
                if (item.Username != username)
                {
                    continue;
                }

                has = true;

                item.Username = username;
                item.Token = token;
                item.Scopes = scopes;
                break;
            }

            if (has)
            {
                IsDirty = true;
                return true;
            }
            else
            {
                return AddItem(username, token, scopes);
            }
        }

        public List<ConfigItem> GetItems()
        {
            return _items;
        }

        public bool AddItem(ConfigItem item)
        {
            if (Items.Contains(item))
            {
                return false;
            }

            Items.Add(item);
            IsDirty = true;
            return true;
        }

        public bool RemoveItemByIndex(int index)
        {
            IsDirty = true;
            return Items.Count > index && RemoveItem(Items[index]);
        }

        private bool RemoveItem(ConfigItem item)
        {
            if (!Items.Contains(item))
            {
                return false;
            }

            Items.Remove(item);
            IsDirty = true;
            return true;
        }

        public void SetUsername(ConfigItem item, string username)
        {
            if (item == null)
            {
                Debug.LogError("item is null");
                return;
            }

            item.Username = username;
        }

        #region file operate

        private void CreateConfig()
        {
            Write("");
        }

        public void Revert()
        {
            LoadConfig();
            IsDirty = false;
        }

        public void Apply()
        {
            SaveConfig();
            LoadConfig();
            IsDirty = false;
        }

        private void SaveConfig()
        {
            var setting = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var json = JsonConvert.SerializeObject(_items, Formatting.Indented, setting);
            Write(json);
        }

        private void Write(string content)
        {
            using (var sw = new StreamWriter(UECConfigPath, false, System.Text.Encoding.Default))
            {
                sw.Write(content);
                sw.Close();
            }
        }

        #endregion
    }
}