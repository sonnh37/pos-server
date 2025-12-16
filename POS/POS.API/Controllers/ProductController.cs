using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.API.Controllers.Base;
using POS.Domain.Contracts.Services;
using POS.Domain.Models.CQRS.Commands.Products;
using POS.Domain.Models.CQRS.Queries.Products;

namespace POS.API.Controllers;

// [Authorize(Roles = "Admin,Staff")]
public class ProductController : BaseController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductGetAllQuery request)
    {
        var businessResult = await _productService.GetAll(request);

        return Ok(businessResult);
    }

    [AllowAnonymous]
    [HttpGet("id")]
    public async Task<IActionResult> GetById([FromQuery] ProductGetByIdQuery request)
    {
        var businessResult = await _productService.GetById(request);
        return Ok(businessResult);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateCommand request)
    {
        var businessResult = await _productService.Create(request);

        return Ok(businessResult);
    }

    // [HttpPut]
    // public async Task<IActionResult> Update([FromBody] ProductUpdateCommand request)
    // {
    //     var businessResult = await _productService.Update(request);
    //
    //     return Ok(businessResult);
    // }
    //
    // [HttpDelete]
    // public async Task<IActionResult> Delete([FromQuery] ProductDeleteCommand request)
    // {
    //     var businessResult = await  _productService.Delete(request);
    //
    //     return Ok(businessResult);
    // }
}