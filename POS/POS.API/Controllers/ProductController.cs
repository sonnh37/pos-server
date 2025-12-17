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
}