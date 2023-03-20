#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace GitDependecyResolvers
{
    [InitializeOnLoad]
    public static class GitDependecyResolver
    {
        private const string _dependenciesJsonFileName = "Dependencies.json";

        static GitDependecyResolver()
        {
            Events.registeringPackages += RegisterPackagesEvents;
        }

        private static void RegisterPackagesEvents(PackageRegistrationEventArgs packageRegistrationEventArgs)
        {
            CheckDependencies(packageRegistrationEventArgs.added);
            CheckDependencies(packageRegistrationEventArgs.changedTo);
        }

        private static void CheckDependencies(System.Collections.ObjectModel.ReadOnlyCollection<PackageInfo> readOnlyCollection)
        {
            foreach (var packageInfo in readOnlyCollection)
            {
                if (packageInfo.source == PackageSource.Embedded)
                    continue;

                AddDependenciesFromPackage(packageInfo);
            }
        }

        private static void AddDependenciesFromPackage(PackageInfo packageInfo)
        {
            string path = Path.Combine(packageInfo.resolvedPath, _dependenciesJsonFileName);

            if (!File.Exists(path))
                return;

            string json = File.ReadAllText(path);

            Dependencies dependencies = JsonUtility.FromJson<Dependencies>(json);

            if (null == dependencies)
                return;

            List<Dependency> dependencyList = dependencies.dependencies;

            foreach (var dependency in dependencyList)
            {
                if (string.IsNullOrEmpty(dependency.gitUrl))
                    continue;

                if (IsPackageInstalled(dependency.gitUrl))
                    continue;

                PackageAdder.Add(dependency.gitUrl);
            }
        }

        public static bool IsPackageInstalled(string packageId)
        {
            if (!File.Exists("Packages/manifest.json"))
                return false;

            string jsonText = File.ReadAllText("Packages/manifest.json");
            return jsonText.Contains(packageId);
        }
    }
}
#endif