﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using Newtonsoft.Json;

namespace NuGet.Server.Core.Infrastructure
{
    public class ServerPackage
        : IServerPackage
    {
        public ServerPackage()
        {
        }

        public ServerPackage(
            IPackage package,
            PackageDerivedData packageDerivedData)
        {
            Id = package.Id;
            Version = package.Version;
            Title = package.Title;
            Authors = package.Authors;
            Owners = package.Owners;
            IconUrl = package.IconUrl;
            LicenseUrl = package.LicenseUrl;
            ProjectUrl = package.ProjectUrl;
            RequireLicenseAcceptance = package.RequireLicenseAcceptance;
            DevelopmentDependency = package.DevelopmentDependency;
            Description = package.Description;
            Summary = package.Summary;
            ReleaseNotes = package.ReleaseNotes;
            Language = package.Language;
            Tags = package.Tags;
            Copyright = package.Copyright;
            MinClientVersion = package.MinClientVersion;
            ReportAbuseUrl = package.ReportAbuseUrl;
            DownloadCount = package.DownloadCount;
            Listed = package.Listed;

            IsSemVer2 = IsPackageSemVer2(package);

            _dependencySets = package.DependencySets.ToList();
            Dependencies = DependencySetsAsString(package.DependencySets);

            _supportedFrameworks = package.GetSupportedFrameworks().ToList();
            SupportedFrameworks = string.Join("|", package.GetSupportedFrameworks().Select(VersionUtility.GetFrameworkString));

            PackageSize = packageDerivedData.PackageSize;
            PackageHash = packageDerivedData.PackageHash;
            PackageHashAlgorithm = packageDerivedData.PackageHashAlgorithm;
            LastUpdated = packageDerivedData.LastUpdated;
            Created = packageDerivedData.Created;
            FullPath = packageDerivedData.FullPath;

            SemVer1IsAbsoluteLatest = false;
            SemVer1IsLatest = false;
            SemVer2IsAbsoluteLatest = false;
            SemVer2IsLatest = false;

            //enhancements
            ProjectSourceUrl = package.ProjectSourceUrl;
            PackageSourceUrl = package.PackageSourceUrl;
            DocsUrl = package.DocsUrl;
            WikiUrl = package.WikiUrl;
            MailingListUrl = package.MailingListUrl;
            BugTrackerUrl = package.BugTrackerUrl;
            

            Replaces = package.Replaces;
            Provides = package.Provides;
            Conflicts = package.Conflicts;

            // server metadata 
            //todo: need a place to save this local to the package itself
            IsApproved = package.IsApproved;
            PackageStatus = package.PackageStatus;
            PackageSubmittedStatus = package.PackageSubmittedStatus;
            PackageTestResultStatus = package.PackageTestResultStatus;
            PackageTestResultStatusDate = package.PackageTestResultStatusDate;
            PackageValidationResultStatus = package.PackageValidationResultStatus;
            PackageValidationResultDate = package.PackageValidationResultDate;
            PackageCleanupResultDate = package.PackageCleanupResultDate;
            PackageReviewedDate = package.PackageReviewedDate;
            PackageApprovedDate = package.PackageApprovedDate;
            PackageReviewer = package.PackageReviewer;
            IsDownloadCacheAvailable = package.IsDownloadCacheAvailable;
            DownloadCacheDate = package.DownloadCacheDate;
            DownloadCache = package.DownloadCache;

            SoftwareDisplayName = package.SoftwareDisplayName;
            SoftwareDisplayVersion = package.SoftwareDisplayVersion;

        }

        [JsonRequired]
        public string Id { get; set; }

        [JsonRequired, JsonConverter(typeof(SemanticVersionJsonConverter))]
        public SemanticVersion Version { get; set; }

        public string Title { get; set; }

        public IEnumerable<string> Authors { get; set; }

        public IEnumerable<string> Owners { get; set; }

        public Uri IconUrl { get; set; }

        public Uri LicenseUrl { get; set; }

        public Uri ProjectUrl { get; set; }

        public bool RequireLicenseAcceptance { get; set; }

        public bool DevelopmentDependency { get; set; }

        public string Description { get; set; }

        public string Summary { get; set; }

        public string ReleaseNotes { get; set; }

        public string Language { get; set; }

        public string Tags { get; set; }

        public string Copyright { get; set; }

        #region NuSpec Enhancements
        public Uri ProjectSourceUrl { get; set; }
        public Uri PackageSourceUrl { get; set; }
        public Uri DocsUrl { get; set; }
        public Uri WikiUrl { get; set; }
        public Uri MailingListUrl { get; set; }
        public Uri BugTrackerUrl { get; set; }
        public IEnumerable<string> Replaces { get; set; }
        public IEnumerable<string> Provides { get; set; }
        public IEnumerable<string> Conflicts { get; set; }
        // round 2
        public string SoftwareDisplayName { get; set; }
        public string SoftwareDisplayVersion { get; set; }
        #endregion

        #region Server Metadata Only

        public bool IsApproved { get; set; }
        public string PackageStatus { get; set; }
        public string PackageSubmittedStatus { get; set; }
        public string PackageTestResultStatus { get; set; }
        public DateTime? PackageTestResultStatusDate { get; set; }
        public string PackageValidationResultStatus { get; set; }
        public DateTime? PackageValidationResultDate { get; set; }
        public DateTime? PackageCleanupResultDate { get; set; }
        public DateTime? PackageReviewedDate { get; set; }
        public DateTime? PackageApprovedDate { get; set; }
        public string PackageReviewer { get; set; }
        public bool IsDownloadCacheAvailable { get; set; }
        public DateTime? DownloadCacheDate { get; set; }
        public IEnumerable<DownloadCache> DownloadCache { get; set; }

        #endregion
        

        public string Dependencies { get; set; }

        private List<PackageDependencySet> _dependencySets;

        [JsonIgnore]
        public IEnumerable<PackageDependencySet> DependencySets
        {
            get
            {
                if (String.IsNullOrEmpty(Dependencies))
                {
                    return Enumerable.Empty<PackageDependencySet>();
                }

                if (_dependencySets == null)
                {
                    _dependencySets = ParseDependencySet(Dependencies);
                }

                return _dependencySets;
            }
        }

        [JsonConverter(typeof(SemanticVersionJsonConverter))]
        public Version MinClientVersion { get; set; }

        public Uri ReportAbuseUrl { get; set; }

        public int DownloadCount { get; set; }
        
        public string SupportedFrameworks { get; set; }
       
        private List<FrameworkName> _supportedFrameworks;
        public IEnumerable<FrameworkName> GetSupportedFrameworks()
        {
            if (String.IsNullOrEmpty(SupportedFrameworks))
            {
                return Enumerable.Empty<FrameworkName>();
            }

            if (_supportedFrameworks == null)
            {
                var supportedFrameworksAsStrings = SupportedFrameworks.Split('|').ToList();

                _supportedFrameworks = supportedFrameworksAsStrings
                    .Select(VersionUtility.ParseFrameworkName)
                    .ToList();
            }

            return _supportedFrameworks;
        }

        public bool SemVer1IsAbsoluteLatest { get; set; }

        public bool SemVer1IsLatest { get; set; }

        public bool SemVer2IsAbsoluteLatest { get; set; }

        public bool SemVer2IsLatest { get; set; }

        public bool Listed { get; set; }

        public bool IsSemVer2 { get; set; }

        public long PackageSize { get; set; }

        public string PackageHash { get; set; }

        public string PackageHashAlgorithm { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public DateTimeOffset Created { get; set; }

        public string FullPath { get; set; }

        private static string DependencySetsAsString(IEnumerable<PackageDependencySet> dependencySets)
        {
            if (dependencySets == null)
            {
                return null;
            }

            var dependencies = new List<string>();
            foreach (var dependencySet in dependencySets)
            {
                if (dependencySet.Dependencies.Count == 0)
                {
                    dependencies.Add(string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", null, null, dependencySet.TargetFramework.ToShortNameOrNull()));
                }
                else
                {
                    foreach (var dependency in dependencySet.Dependencies.Select(d => new { d.Id, d.VersionSpec, dependencySet.TargetFramework }))
                    {
                        dependencies.Add(string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", dependency.Id, dependency.VersionSpec == null ? null : dependency.VersionSpec.ToString(), dependencySet.TargetFramework.ToShortNameOrNull()));
                    }
                }
            }

            return string.Join("|", dependencies);
        }

        private static List<PackageDependencySet> ParseDependencySet(string value)
        {
            var dependencySets = new List<PackageDependencySet>();

            var dependencies = value.Split('|').Select(ParseDependency).ToList();

            // group the dependencies by target framework
            var groups = dependencies.GroupBy(d => d.Item3);

            dependencySets.AddRange(
                groups.Select(g => new PackageDependencySet(
                    g.Key,   // target framework 
                    g.Where(pair => !String.IsNullOrEmpty(pair.Item1))                   // the Id is empty when a group is empty.
                     .Select(pair => new PackageDependency(pair.Item1, pair.Item2)))));  // dependencies by that target framework
            return dependencySets;
        }

        /// <summary>
        /// Parses a dependency from the feed in the format:
        ///     id or id:versionSpec, or id:versionSpec:targetFramework
        /// </summary>
        private static Tuple<string, IVersionSpec, FrameworkName> ParseDependency(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // IMPORTANT: Do not pass StringSplitOptions.RemoveEmptyEntries to this method, because it will break 
            // if the version spec is null, for in that case, the Dependencies string sent down is "<id>::<target framework>".
            // We do want to preserve the second empty element after the split.
            var tokens = value.Trim().Split(new[] { ':' });

            if (tokens.Length == 0)
            {
                return null;
            }

            // Trim the id
            var id = tokens[0].Trim();

            IVersionSpec versionSpec = null;
            if (tokens.Length > 1)
            {
                // Attempt to parse the version
                VersionUtility.TryParseVersionSpec(tokens[1], out versionSpec);
            }

            var targetFramework = (tokens.Length > 2 && !String.IsNullOrEmpty(tokens[2]))
                                    ? VersionUtility.ParseFrameworkName(tokens[2])
                                    : null;

            return Tuple.Create(id, versionSpec, targetFramework);
        }

        private static bool IsPackageSemVer2(IPackage package)
        {
            if (package.Version.IsSemVer2())
            {
                return true;
            }

            if (package.DependencySets != null)
            {
                foreach (var dependencySet in package.DependencySets)
                {
                    foreach (var dependency in dependencySet.Dependencies)
                    {
                        var range = dependency.VersionSpec;
                        if (range == null)
                        {
                            continue;
                        }

                        if (range.MinVersion != null && range.MinVersion.IsSemVer2())
                        {
                            return true;
                        }

                        if (range.MaxVersion != null && range.MaxVersion.IsSemVer2())
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
        }
    }
}
