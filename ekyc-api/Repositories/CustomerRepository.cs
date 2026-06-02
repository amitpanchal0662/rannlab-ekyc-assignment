using Dapper;
using Npgsql;
using ekyc_api.Models;
using ekyc_api.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace ekyc_api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IConfiguration _configuration;

    private readonly IDistributedCache _cache;

    public CustomerRepository(
        IConfiguration configuration,
        IDistributedCache cache)
    {
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<CustomerEkyc?> GetByIdAsync(Guid id)
    {
        string cacheKey = $"customer:{id}";

        var cachedCustomer =
            await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedCustomer))
        {
            return JsonSerializer.Deserialize<CustomerEkyc>(
                cachedCustomer);
        }

        await using var connection =
            new NpgsqlConnection(
                _configuration.GetConnectionString("Postgres"));

        const string sql = @"
        SELECT
            id,
            customer_ref_no as CustomerRefNo,
            first_name as FirstName,
            last_name as LastName,
            mobile,
            email,
            ekyc_status as EkycStatus,
            is_active as IsActive
        FROM customer_ekyc
        WHERE id = @Id";

        var customer =
            await connection.QueryFirstOrDefaultAsync<CustomerEkyc>(
                sql,
                new { Id = id });

        if (customer != null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(customer),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(10)
                });
        }

        return customer;
    }



    public async Task<Guid> CreateAsync(CreateCustomerRequest request)
    {
        var id = Guid.NewGuid();

        const string sql = @"
        INSERT INTO customer_ekyc
        (
            id,
            customer_ref_no,
            first_name,
            middle_name,
            last_name,
            dob,
            gender,
            mobile,
            email,
            ekyc_status,
            kyc_level,
            risk_score,
            org_id,
            org_name,
            created_at,
            updated_at,
            created_by,
            updated_by,
            is_active
        )
        VALUES
        (
            @Id,
            @CustomerRefNo,
            @FirstName,
            @MiddleName,
            @LastName,
            @Dob,
            @Gender,
            @Mobile,
            @Email,
            @EkycStatus,
            @KycLevel,
            @RiskScore,
            @OrgId,
            @OrgName,
            NOW(),
            NOW(),
            'api',
            'api',
            true
        );";

        await using var connection =
            new NpgsqlConnection(
                _configuration.GetConnectionString("Postgres"));

        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            request.CustomerRefNo,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.Dob,
            request.Gender,
            request.Mobile,
            request.Email,
            request.EkycStatus,
            request.KycLevel,
            request.RiskScore,
            request.OrgId,
            request.OrgName
        });

        await _cache.RemoveAsync($"customer:{id}");

        return id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        const string sql = @"
        UPDATE customer_ekyc
        SET
            first_name = @FirstName,
            middle_name = @MiddleName,
            last_name = @LastName,
            mobile = @Mobile,
            email = @Email,
            ekyc_status = @EkycStatus,
            is_active = @IsActive,
            updated_at = NOW(),
            updated_by = 'api'
        WHERE id = @Id";

        await using var connection =
            new NpgsqlConnection(
                _configuration.GetConnectionString("Postgres"));

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.Mobile,
            request.Email,
            request.EkycStatus,
            request.IsActive
        });

        await _cache.RemoveAsync($"customer:{id}");

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = @"
        UPDATE customer_ekyc
        SET
            is_active = false,
            updated_at = NOW(),
            updated_by = 'api'
        WHERE id = @Id";

        await using var connection =
            new NpgsqlConnection(
                _configuration.GetConnectionString("Postgres"));

        var rowsAffected =
            await connection.ExecuteAsync(
                sql,
                new { Id = id });

                    await _cache.RemoveAsync($"customer:{id}");

        return rowsAffected > 0;


    }

    public async Task<CustomerListResponse> GetCustomersAsync(
    CustomerListRequest request)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await using var connection =
            new NpgsqlConnection(
                _configuration.GetConnectionString("Postgres"));

        var sql = @"
        SELECT
            id,
            customer_ref_no AS CustomerRefNo,
            first_name AS FirstName,
            last_name AS LastName,
            mobile,
            email,
            ekyc_status AS EkycStatus,
            is_active AS IsActive,
            created_at AS CreatedAt
        FROM customer_ekyc
        WHERE 1 = 1 ";

        var parameters = new DynamicParameters();

        // Filters

        if (request.OrgId.HasValue)
        {
            sql += " AND org_id = @OrgId ";
            parameters.Add("OrgId", request.OrgId);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            sql += " AND ekyc_status = @Status ";
            parameters.Add("Status", request.Status);
        }

        if (request.IsActive.HasValue)
        {
            sql += " AND is_active = @IsActive ";
            parameters.Add("IsActive", request.IsActive);
        }

        if (request.CreatedFrom.HasValue)
        {
            sql += " AND created_at >= @CreatedFrom ";
            parameters.Add("CreatedFrom", request.CreatedFrom);
        }

        if (request.CreatedTo.HasValue)
        {
            sql += " AND created_at <= @CreatedTo ";
            parameters.Add("CreatedTo", request.CreatedTo);
        }

        if (request.RiskMin.HasValue)
        {
            sql += " AND risk_score >= @RiskMin ";
            parameters.Add("RiskMin", request.RiskMin);
        }

        if (request.RiskMax.HasValue)
        {
            sql += " AND risk_score <= @RiskMax ";
            parameters.Add("RiskMax", request.RiskMax);
        }

        // Search

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            sql += @"
        AND (
            first_name ILIKE @Q
            OR last_name ILIKE @Q
            OR mobile ILIKE @Q
            OR email ILIKE @Q
            OR customer_ref_no ILIKE @Q
            OR city ILIKE @Q
            OR org_name ILIKE @Q
        )";

            parameters.Add("Q", $"%{request.Q}%");
        }

        // Keyset Pagination

        if (request.CursorCreatedAt.HasValue &&
            request.CursorId.HasValue)
        {
            sql += @"
        AND (
            created_at < @CursorCreatedAt
            OR
            (
                created_at = @CursorCreatedAt
                AND id < @CursorId
            )
        )";

            parameters.Add(
                "CursorCreatedAt",
                request.CursorCreatedAt);

            parameters.Add(
                "CursorId",
                request.CursorId);
        }

        sql += @"
        ORDER BY created_at DESC, id DESC
        LIMIT @PageSize";

        parameters.Add(
            "PageSize",
            Math.Min(request.PageSize, 200));

        var customers =
            (await connection.QueryAsync<CustomerSummaryDto>(
                sql,
                parameters))
            .ToList();

        stopwatch.Stop();

        var lastRecord = customers.LastOrDefault();

        string? nextCursor = null;

        if (lastRecord != null)
        {
            nextCursor =
                $"{lastRecord.CreatedAt:O}|{lastRecord.Id}";
        }

        return new CustomerListResponse
        {
            Data = customers,
            HasMore = customers.Count == request.PageSize,
            NextCursor = nextCursor,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds
        };
    }

}