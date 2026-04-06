using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SchedulingMS.Application.Interfaces.Ports;

namespace SchedulingMS.Infrastructure.Ports;

public sealed class HttpTechnicianDirectoryGateway(
    HttpClient httpClient,
    ILogger<HttpTechnicianDirectoryGateway> logger) : ITechnicianDirectoryGateway
{
    public async Task<IReadOnlyCollection<TechnicianSummary>> GetActiveTechniciansByProviderAsync(Guid providerEntityId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"api/v1/technician-profiles/by-provider/{providerEntityId}";

        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(requestUri, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(
                ex,
                "DirectoryMS is unreachable at {BaseAddress} while fetching technicians for provider {ProviderEntityId}",
                httpClient.BaseAddress,
                providerEntityId);

            throw new InvalidOperationException(
                $"DirectoryMS is unreachable at {httpClient.BaseAddress}. Verify Integrations:DirectoryMS:BaseUrl and that DirectoryMS is running.",
                ex);
        }

        using (response)
        {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "DirectoryMS returned {StatusCode} for provider {ProviderEntityId}. Body: {Body}",
                (int)response.StatusCode,
                providerEntityId,
                body);

            throw new InvalidOperationException($"DirectoryMS integration failed while fetching technicians for provider {providerEntityId}.");
        }

        var payload = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<DirectoryTechnicianProfileResponse>>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            throw new InvalidOperationException($"DirectoryMS returned an empty payload for provider {providerEntityId}.");
        }

        var technicians = payload
            .Where(static x => x.Status == (int)DirectoryTechnicianStatus.Active)
            .Select(static x => new TechnicianSummary(x.Id, x.ProviderEntityId, true))
            .ToArray();

        logger.LogInformation(
            "Fetched {TechnicianCount} active technicians from DirectoryMS for provider {ProviderEntityId}",
            technicians.Length,
            providerEntityId);

        return technicians;
        }
    }

    public async Task<TechnicianSummary?> GetActiveTechnicianByIdAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"api/v1/technician-profiles/{technicianId}";

        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(requestUri, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(
                ex,
                "DirectoryMS is unreachable at {BaseAddress} while fetching technician {TechnicianId}",
                httpClient.BaseAddress,
                technicianId);

            throw new InvalidOperationException(
                $"DirectoryMS is unreachable at {httpClient.BaseAddress}. Verify Integrations:DirectoryMS:BaseUrl and that DirectoryMS is running.",
                ex);
        }

        using (response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "DirectoryMS returned {StatusCode} for technician {TechnicianId}. Body: {Body}",
                    (int)response.StatusCode,
                    technicianId,
                    body);

                throw new InvalidOperationException($"DirectoryMS integration failed while fetching technician {technicianId}.");
            }

            var payload = await response.Content.ReadFromJsonAsync<DirectoryTechnicianProfileResponse>(cancellationToken: cancellationToken);
            if (payload is null || payload.Status != (int)DirectoryTechnicianStatus.Active)
            {
                return null;
            }

            return new TechnicianSummary(payload.Id, payload.ProviderEntityId, true);
        }
    }

    private sealed record DirectoryTechnicianProfileResponse(
        Guid Id,
        Guid AuthUserId,
        Guid ProviderEntityId,
        string Specialty,
        int Status,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc);

    private enum DirectoryTechnicianStatus
    {
        Active = 1,
        Restricted = 2,
        Inactive = 3
    }
}
