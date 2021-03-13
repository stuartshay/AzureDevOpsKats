using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDevOpsKats.Common.HealthChecks
{
    public class SqliteConnectionHealthCheck : IHealthCheck
    {
        private static readonly string DefaultTestQuery = "Select 1";

        public string ConnectionString { get; }

        public string TestQuery { get; }

        public SqliteConnectionHealthCheck(string connectionString) :
            this(connectionString, testQuery: DefaultTestQuery)
        { }

        public SqliteConnectionHealthCheck(string connectionString, string testQuery)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            TestQuery = testQuery;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            await using var connection = new SqliteConnection(ConnectionString);

            try
            {
                await connection.OpenAsync(cancellationToken);
                if (TestQuery != null)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = TestQuery;
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
            catch (DbException ex)
            {
                return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
            }

            return HealthCheckResult.Healthy();
        }
    }
}
