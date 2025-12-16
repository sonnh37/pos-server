using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Domain.Contracts.Services;
using POS.Domain.Models.CQRS.Commands.Orders;
using POS.Domain.Models.CQRS.Queries.Orders;

namespace POS.API.Controllers;

public class OrderController : BaseController
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
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

        return Ok(businessResult);
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