﻿using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Reporting;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.Reporting
{
    public class UserReportingSsClient : ErrorHandlingWrapper, IUserReportingClient
    {
        private readonly ServiceProvider _services;

        public UserReportingSsClient(ServiceProvider services) => _services = services;

        public async Task<IUserSnapshotReportResult> CreateSnapshotReportAsync(DateTime? date, UserReportFilter filters, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () => await _services.GetRequiredService<UserSnapshotReportMutation>().ExecuteAsync(date, filters, cancellationToken));

        public async Task<IUserChangelogReportResult> CreateChangelogReportAsync(DateTime? startDate, DateTime? endDate, UserReportFilter? filters,
            CancellationToken cancellationToken) =>
            await ExecuteAsync(async () => await _services.GetRequiredService<UserChangelogReportMutation>()
                .ExecuteAsync(startDate, endDate, filters, cancellationToken));

        public async Task<IGetUserChangelogResult> GetUserChangelog(DateTime? startDate, DateTime? endDate,
            UserReportFilter? filters, int? after, int? limit, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () =>
            await _services.GetRequiredService<GetUserChangelogQuery>()
                .ExecuteAsync(startDate, endDate, filters, after, limit, cancellationToken));

        public async Task<IUserSummaryResult> UserSummary(DateTime? date, CancellationToken cancellationToken)
            => await ExecuteAsync(async () =>
                await _services.GetService<UserSummaryQuery>().ExecuteAsync(date, cancellationToken));

        public async Task<IGetUserSnapshotResult> GetUserSnapshots(DateTime? date, UserReportFilter? filters, int? after, int? limit, CancellationToken cancellationToken) => 
            await ExecuteAsync(async () =>
                await _services.GetService<GetUserSnapshotQuery>().ExecuteAsync(date, filters, after, limit, cancellationToken));
    }
}
