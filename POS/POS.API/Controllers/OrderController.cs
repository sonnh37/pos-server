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
    [HttpGet("id")]
    public async Task<IActionResult> GetById([FromQuery] OrderGetByIdQuery request)
    {
        var businessResult = await _orderService.GetById(request);
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
    
    [HttpPost("test")]
    public async Task<IActionResult> CreateTestOrder()
    {
        var testOrder = new Order
        {
            OrderNumber = $"ORD-TEST-{DateTime.Now:HHmmss}",
            TotalAmount = new Random().Next(50000, 500000),
            OrderDate = DateTime.UtcNow
        };

        
        await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", testOrder);

        return Ok(new { message = "Test order created", order = testOrder });
    }

    // [HttpPut]
    // public async Task<IActionResult> Update([FromBody] OrderUpdateCommand request)
    // {
    //     var businessResult = await _orderService.Update(request);
    //
    //     return Ok(businessResult);
    // }
    //
    // [HttpDelete]
    // public async Task<IActionResult> Delete([FromQuery] OrderDeleteCommand request)
    // {
    //     var businessResult = await  _orderService.Delete(request);
    //
    //     return Ok(businessResult);
    // }
}