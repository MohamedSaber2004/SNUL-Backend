using Auth.Services.API.AuthRoutes;
using Auth.Services.API.Features.Auth.Commands.ForgotPassword;
using Auth.Services.API.Features.Auth.Commands.Login;
using Auth.Services.API.Features.Auth.Commands.Logout;
using Auth.Services.API.Features.Auth.Commands.RefreshToken;
using Auth.Services.API.Features.Auth.Commands.Register;
using Auth.Services.API.Features.Auth.Commands.ResendRegisterOtp;
using Auth.Services.API.Features.Auth.Commands.ResetPassword;
using Auth.Services.API.Features.Auth.Commands.UpdateProfile;
using Auth.Services.API.Features.Auth.Commands.VerifyEmailOtp;
using Auth.Services.API.Features.Auth.Commands.VerifyPasswordOtp;
using Auth.Services.API.Features.Auth.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNUL.Shared.Common.Attributes;
using SNUL.Shared.Controllers;

namespace Auth.Services.API.Controllers
{
    [Route(AuthApiRoutes.Base)]
    public class AuthController : AppControllerBase
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Register a new user and send an OTP to their email for verification.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpPost]
        [Route(AuthApiRoutes.Authentication.Register)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Authenticate a user and return an access token and refresh token if successful.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(AuthApiRoutes.Authentication.Login)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Verify the OTP sent to the user's email during registration.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(AuthApiRoutes.Authentication.VerifyEmailOtp)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyEmailOtp([FromBody] VerifyEmailOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Resend the OTP to the user's email for registration verification.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(AuthApiRoutes.Authentication.ResendRegisterOtp)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResendRegisterOtp([FromBody] ResendRegisterOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Forget PASSWORD: Initiate the password reset process by sending an OTP to the user's email.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(AuthApiRoutes.Authentication.ForgotPassword)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Verify the OTP sent to the user's email for password reset.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(AuthApiRoutes.Authentication.VerifyPasswordOtp)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyPasswordOtp([FromBody] VerifyPasswordOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Reset the user's password using the provided OTP and new password.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(AuthApiRoutes.Authentication.ResetPassword)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Refresh the access token using a valid refresh token.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(AuthApiRoutes.Authentication.RefreshToken)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Logout the user by revoking the refresh token and optionally revoking all sessions.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(AuthApiRoutes.Authentication.Logout)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand? command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command ?? new LogoutCommand(), cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Get the profile information of the currently authenticated user.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [RoleAuthorize]
        [Route(AuthApiRoutes.Authentication.Profile)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserProfileQuery(), cancellationToken);
            return ToActionResult(result);
        }

        /// <summary>
        /// Update the profile information of the currently authenticated user.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [RoleAuthorize]
        [Route(AuthApiRoutes.Authentication.Profile)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result);
        }
    }
}
