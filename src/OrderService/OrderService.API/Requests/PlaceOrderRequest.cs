namespace OrderService.API.Requests;

public record PlaceOrderRequest(Guid UserId, decimal Amount);