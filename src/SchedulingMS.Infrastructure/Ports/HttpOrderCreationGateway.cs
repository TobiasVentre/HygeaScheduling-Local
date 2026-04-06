using System.Net.Http.Json;
using System.Text.Json;
using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Ports;

namespace SchedulingMS.Infrastructure.Ports;

public class HttpOrderCreationGateway(HttpClient httpClient) : IOrderCreationGateway
{
    public async Task<ExternalServiceOrderResponse> CreateAsync(CreateExternalServiceOrderRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            request.ReservationId,
            request.ClientId,
            request.ProviderEntityId,
            request.TechnicianId,
            request.ScheduledStartAtUtc,
            request.ScheduledEndAtUtc,
            Items = request.Items.Select(x => new
            {
                x.ServiceId,
                x.ServiceName,
                x.UnitPrice,
                x.Quantity
            })
        };

        using var response = await httpClient.PostAsJsonAsync("api/v1/service-orders", payload, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new ValidationException($"OrderMS order creation failed with status {(int)response.StatusCode}. Body: {body}");
        }

        var created = await response.Content.ReadFromJsonAsync<ServiceOrderGatewayResponse>(cancellationToken: cancellationToken)
            ?? throw new ValidationException("OrderMS returned an empty response while creating the service order.");

        return new ExternalServiceOrderResponse(created.Id, NormalizeStatus(created.Status), created.TotalAmount);
    }

    private static string NormalizeStatus(JsonElement status)
    {
        if (status.ValueKind == JsonValueKind.String)
        {
            return status.GetString() ?? "Created";
        }

        if (status.ValueKind == JsonValueKind.Number && status.TryGetInt32(out var numericStatus))
        {
            return numericStatus switch
            {
                1 => "Created",
                2 => "Confirmed",
                3 => "InProgress",
                4 => "Finalized",
                5 => "Exception",
                6 => "Closed",
                7 => "Approved",
                _ => numericStatus.ToString()
            };
        }

        return status.ToString();
    }

    private sealed record ServiceOrderGatewayResponse(Guid Id, JsonElement Status, decimal TotalAmount);
}
