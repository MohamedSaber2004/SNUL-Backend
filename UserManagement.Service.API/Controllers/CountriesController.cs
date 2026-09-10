using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Service.API.Features.Countries.Commands.CreateCountry;
using UserManagement.Service.API.Features.Countries.Commands.DeleteCountry;
using UserManagement.Service.API.Features.Countries.Commands.UpdateCountry;
using UserManagement.Service.API.Features.Countries.Queries.GetCountries;
using UserManagement.Service.API.Features.Countries.Queries.GetCountryById;
using UserManagement.Service.API.UserManagementRoutes;
using Microsoft.AspNetCore.Authorization;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Controllers;
using SNUL.Shared.Enums;

namespace UserManagement.Service.API.Controllers
{
    [RoleAuthorize]
    [Route(UserManagementApiRoutes.Countries.Base)]
    public class CountriesController : AppControllerBase
    {
        public CountriesController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(UserManagementApiRoutes.Countries.GetAll)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCountriesQuery(), cancellationToken);
            return ToActionResult(result);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(UserManagementApiRoutes.Countries.GetById)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCountryByIdQuery { Id = id }, cancellationToken);
            return ToActionResult(result);
        }

        [HttpPost]
        [Route(UserManagementApiRoutes.Countries.Create)]
        [RoleAuthorize(UserType.Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateCountryCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        [HttpPut]
        [Route(UserManagementApiRoutes.Countries.Update)]
        [RoleAuthorize(UserType.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCountryCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        [HttpDelete]
        [Route(UserManagementApiRoutes.Countries.Delete)]
        [RoleAuthorize(UserType.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteCountryCommand { Id = id }, cancellationToken);
            return ToActionResult(result);
        }
    }
}
