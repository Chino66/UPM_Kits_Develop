using System.Text.RegularExpressions;
using CommandTool;
using UnityEngine;

namespace UPMKits
{
    public class GitRepositoryModel
    {
        private const string Pattern =
            "(https://gitee.com.*.git)|(git@gitee.com:.*.git)|(https://github.com.*.git)|(git@github.com:.*.git)|(git@.*.git)|(https:.*.git)|(http:.*.git)";

        private string _repositoryUrl;

        private string _repositoryName;

        public string RepositoryName => _repositoryName;

        public GitRepositoryModel()
        {
            _repositoryUrl = GetRepositoryUrl();
        }

        public string GetRepositoryUrl()
        {
            if (string.IsNullOrEmpty(_repositoryUrl) == false)
            {
                return _repositoryUrl;
            }

            Command.RunAsync("git config --get remote.origin.url", msgs =>
            {
                var ret = msgs.Messages.Dequeue();

                Match match = Regex.Match(ret, Pattern);
                if (match.Success)
                {
                    _repositoryUrl = match.Value;
                    // https://github.com/Chino66/Object_Pool_Develop.git
                    var startIndex = _repositoryUrl.LastIndexOf("/") + 1;
                    var endIndex = _repositoryUrl.LastIndexOf(".");
                    _repositoryName = _repositoryUrl.Substring(startIndex, endIndex - startIndex);
                }
                else
                {
                    _repositoryUrl = "";
                }

                Debug.Log($"git path is {_repositoryUrl}, repository name is {_repositoryName}");
            });

            return _repositoryUrl;
        }
    }
}