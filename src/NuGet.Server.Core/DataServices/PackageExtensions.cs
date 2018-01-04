// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Server.Core.Infrastructure;

namespace NuGet.Server.Core.DataServices
{
    public static class PackageExtensions
    {
        private static readonly DateTime PublishedForUnlisted = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static ODataPackage AsODataPackage(this IServerPackage package, ClientCompatibility compatibility)
        {
            return new ODataPackage
            {
                Id = package.Id,
                Version = package.Version.ToOriginalString(),
                NormalizedVersion = package.Version.ToNormalizedString(),
                IsPrerelease = !package.IsReleaseVersion(),
                Title = package.Title,
                Authors = string.Join(",", package.Authors),
                Owners = string.Join(",", package.Owners),
                IconUrl = package.IconUrl == null ? null : package.IconUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
                LicenseUrl = package.LicenseUrl == null ? null : package.LicenseUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
                ProjectUrl = package.ProjectUrl == null ? null : package.ProjectUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
                DownloadCount = package.DownloadCount,
                RequireLicenseAcceptance = package.RequireLicenseAcceptance,
                DevelopmentDependency = package.DevelopmentDependency,
                Description = package.Description,
                Summary = package.Summary,
                ReleaseNotes = package.ReleaseNotes,
                Published = package.Listed ? package.Created.UtcDateTime : PublishedForUnlisted,
                LastUpdated = package.LastUpdated.UtcDateTime,
                Dependencies = string.Join("|", package.DependencySets.SelectMany(ConvertDependencySetToStrings)),
                PackageHash = package.PackageHash,
                PackageHashAlgorithm = package.PackageHashAlgorithm,
                PackageSize = package.PackageSize,
                Copyright = package.Copyright,
                Tags = package.Tags,
                IsAbsoluteLatestVersion = compatibility.AllowSemVer2 ? package.SemVer2IsAbsoluteLatest : package.SemVer1IsAbsoluteLatest,
                IsLatestVersion = compatibility.AllowSemVer2 ? package.SemVer2IsLatest : package.SemVer1IsLatest,
                Listed = package.Listed,
                VersionDownloadCount = package.DownloadCount,
                MinClientVersion = package.MinClientVersion == null ? null : package.MinClientVersion.ToString(),
                Language = package.Language,
                // enhancements
                ProjectSourceUrl = package.ProjectSourceUrl == null ? null : package.ProjectSourceUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
                PackageSourceUrl = package.PackageSourceUrl == null ? null : package.PackageSourceUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
                DocsUrl = package.DocsUrl == null ? null : package.DocsUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
                WikiUrl = package.WikiUrl == null ? null : package.WikiUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
                MailingListUrl = package.MailingListUrl == null ? null : package.MailingListUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
                BugTrackerUrl = package.BugTrackerUrl == null ? null : package.BugTrackerUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
                Replaces = package.Replaces == null ? null : string.Join(",", package.Replaces),
                Provides = package.Provides == null ? null : string.Join(",", package.Provides),
                Conflicts = package.Conflicts == null ? null : string.Join(",", package.Conflicts),
                // server metadata
                IsApproved = package.IsApproved,
                PackageStatus = package.PackageStatus,
                PackageSubmittedStatus = package.PackageSubmittedStatus,
                PackageTestResultStatus = package.PackageTestResultStatus,
                PackageTestResultStatusDate = package.PackageTestResultStatusDate,
                PackageValidationResultStatus = package.PackageValidationResultStatus,
                PackageValidationResultDate = package.PackageValidationResultDate,
                PackageCleanupResultDate = package.PackageCleanupResultDate,
                PackageReviewedDate = package.PackageReviewedDate,
                PackageApprovedDate = package.PackageApprovedDate,
                PackageReviewer = package.PackageReviewer,
                IsDownloadCacheAvailable = package.IsDownloadCacheAvailable,
                DownloadCacheDate = package.DownloadCacheDate,
                DownloadCache = package.DownloadCache == null ? null : string.Join("|", package.DownloadCache.Select(ConvertDownloadCacheToStrings)),
                // enhancements round 2
                SoftwareDisplayName = package.SoftwareDisplayName,
                SoftwareDisplayVersion = package.SoftwareDisplayVersion,
        };
        }

        private static IEnumerable<string> ConvertDependencySetToStrings(PackageDependencySet dependencySet)
        {
            if (dependencySet.Dependencies.Count == 0)
            {
                if (dependencySet.TargetFramework != null)
                {
                    // if this Dependency set is empty, we still need to send down one string of the form "::<target framework>",
                    // so that the client can reconstruct an empty group.
                    return new[] { string.Format("::{0}", VersionUtility.GetShortFrameworkName(dependencySet.TargetFramework)) };
                }
            }
            else
            {
                return dependencySet.Dependencies.Select(dependency => ConvertDependency(dependency, dependencySet.TargetFramework));
            }

            return new string[0];
        }

        private static string ConvertDependency(PackageDependency packageDependency, FrameworkName targetFramework)
        {
            if (targetFramework == null)
            {
                if (packageDependency.VersionSpec == null)
                {
                    return packageDependency.Id;
                }
                else
                {
                    return string.Format("{0}:{1}", packageDependency.Id, packageDependency.VersionSpec);
                }
            }
            else
            {
                return string.Format("{0}:{1}:{2}", packageDependency.Id, packageDependency.VersionSpec, VersionUtility.GetShortFrameworkName(targetFramework));
            }
        }

        private static string ConvertDownloadCacheToStrings(DownloadCache cache)
        {
            return string.Format("{0}^{1}^{2}", cache.OriginalUrl, cache.FileName, cache.Checksum);
        }
    }
}