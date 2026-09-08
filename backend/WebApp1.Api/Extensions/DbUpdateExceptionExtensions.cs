using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WebApp1.Api.Extensions;

public static class DbUpdateExceptionExtensions
{
    public static bool IsUniqueViolation(this DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };
    }

    public static bool TryGetUniqueViolation(this DbUpdateException exception, out PostgresException postgresException)
    {
        if (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            postgresException = (PostgresException)exception.InnerException;
            return true;
        }

        postgresException = null!;
        return false;
    }
}