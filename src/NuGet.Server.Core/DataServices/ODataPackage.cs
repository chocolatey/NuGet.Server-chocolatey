// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace NuGet.Server.Core.DataServices
{
    public class ODataPackage
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public string NormalizedVersion { get; set; }

        public bool IsPrerelease { get; set; }

        public string Title { get; set; }

        public string Authors { get; set; }

        public string Owners { get; set; }

        public string IconUrl { get; set; }

        public string LicenseUrl { get; set; }

        public string ProjectUrl { get; set; }

        public int DownloadCount { get; set; }

        public bool RequireLicenseAcceptance { get; set; }

        public bool DevelopmentDependency { get; set; }

        public string Description { get; set; }

        public string Summary { get; set; }

        public string ReleaseNotes { get; set; }

        public DateTime Published { get; set; }

        public DateTime LastUpdated { get; set; }

        public string Dependencies { get; set; }

        public string PackageHash { get; set; }

        public string PackageHashAlgorithm { get; set; }

        public long PackageSize { get; set; }

        public string Copyright { get; set; }

        public string Tags { get; set; }

        public bool IsAbsoluteLatestVersion { get; set; }

        public bool IsLatestVersion { get; set; }

        public bool Listed { get; set; }

        public int VersionDownloadCount { get; set; }

        public string MinClientVersion { get; set; }

        public string Language { get; set; }

        #region NuSpec Enhancements
        public string ProjectSourceUrl { get; set; }
        public string PackageSourceUrl { get; set; }
        public string DocsUrl { get; set; }
        public string WikiUrl { get; set; }
        public string MailingListUrl { get; set; }
        public string BugTrackerUrl { get; set; }
        public string Replaces { get; set; }
        public string Provides { get; set; }
        public string Conflicts { get; set; }
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
        public string DownloadCache { get; set; }

        #endregion

    }
}