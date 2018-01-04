// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 
namespace NuGet.Server.Core.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public partial interface IServerPackage
    {
        Uri ProjectSourceUrl { get; }
        Uri PackageSourceUrl { get; }
        Uri DocsUrl { get; }
        Uri WikiUrl { get; }
        Uri MailingListUrl { get; }
        Uri BugTrackerUrl { get; }
        IEnumerable<string> Replaces { get; }
        IEnumerable<string> Provides { get; }
        IEnumerable<string> Conflicts { get; }

        string SoftwareDisplayName { get; }
        string SoftwareDisplayVersion { get; }

        bool IsApproved { get; }
        string PackageStatus { get; }
        string PackageSubmittedStatus { get; }
        string PackageTestResultStatus { get; }
        DateTime? PackageTestResultStatusDate { get; }
        string PackageValidationResultStatus { get; }
        DateTime? PackageValidationResultDate { get; }
        DateTime? PackageCleanupResultDate { get; }
        DateTime? PackageReviewedDate { get; }
        DateTime? PackageApprovedDate { get; }
        string PackageReviewer { get; }

        bool IsDownloadCacheAvailable { get; }
        DateTime? DownloadCacheDate { get; }
        IEnumerable<DownloadCache> DownloadCache { get; }

    }
}
