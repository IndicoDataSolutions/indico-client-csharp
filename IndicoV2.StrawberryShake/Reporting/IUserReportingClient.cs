using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake;

namespace IndicoV2.Reporting
{
    public interface IUserReportingClient
    {
        Task<IUserSnapshotReportResult> CreateSnapshotReportAsync(DateTime? date, UserReportFilter? filters, CancellationToken cancellationToken);

        Task<IUserChangelogReportResult> CreateChangelogReportAsync(DateTime? startDate, DateTime? endDate, UserReportFilter? filters, CancellationToken cancellationToken);

        Task<IUserChangelogResult> UserChangelogQuery(DateTime? startDate, DateTime? endDate, UserReportFilter? filters, CancellationToken cancellationToken);

        Task<IUserSummaryResult> UserSummary(DateTime? date, CancellationToken cancellationToken);

        Task<IGetUserSnapshotResult> GetUserSnapshots(DateTime? date, UserReportFilter? filters, int? after, int? limit, CancellationToken cancellationToken);
    }
}
