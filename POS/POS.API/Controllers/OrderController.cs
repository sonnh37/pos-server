using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using POS.API.Controllers.Base;
using POS.API.Hubs;
using POS.Domain.Contracts.Services;
using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.Orders;
using POS.Domain.Models.CQRS.Queries.Orders;
using POS.Domain.Models.Results.Bases;

namespace POS.API.Controllers;

public class OrderController : BaseController
{
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService, IHubContext<OrderHub> hubContext)
    {
        _orderService = orderService;
        _hubContext = hubContext;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrderGetAllQuery request)
    {
        var businessResult = await _orderService.GetAll(request);

        return Ok(businessResult);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderCreateCommand request)
    {
        var businessResult = await _orderService.Create(request);

        if (businessResult.Status == nameof(Status.OK))
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", businessResult.Data);
        }

        return Ok(businessResult);
    }
}