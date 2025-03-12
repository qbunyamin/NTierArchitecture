using MediatR;
using Microsoft.AspNetCore.Mvc;
using NTierArchitecture.Business.Features.Category.Create;
using NTierArchitecture.Business.Features.Category.Get;
using NTierArchitecture.Business.Features.Category.Update;

using NTierArchitecture.WebApi.Abstractions;

namespace NTierArchitecture.WebApi.Controllers;

public sealed class CategoriesController : ApiController
{
    public CategoriesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    //[RoleFilter("Category.Add")] 
    public async Task<IActionResult> Add(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var response= await _mediator.Send(request, cancellationToken);

        // dönüş tipi yaptık o yüzden değiştirdik hata mesajı için kütüphane (erroror) kurduk o yüzden böyle yaptık....
        if (response.IsError)
        {
            return BadRequest(response.FirstError);
        }

        return NoContent();
    }

    [HttpPut]
    //[RoleFilter("Category.Update")]
    public async Task<IActionResult> Update(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    //[RoleFilter("Category.GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetCategoriesQuery request, CancellationToken cancellationToken )
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

}

