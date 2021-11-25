using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.ParserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoDoge.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthAppService authService;
        private readonly IImagingService imagingService;
        private readonly IParserService parserService;

        public AuthController(IAuthAppService authService, IImagingService imagingService, IParserService parserService)
		{
			this.authService = authService;
            this.imagingService = imagingService;
            this.parserService = parserService;
        }

		[AllowAnonymous]
		[HttpGet("Upload")]
		public async Task<ActionResult<IEnumerable<string>>> Upload()
        {
			var caff = parserService.GetCaffFromFile(@"C:\Users\Akos\Desktop\Suli\MSC_Felev2\computer-sec\computer-sec-cryptodoge\src\backend\CryptoDoge.ParserService.UnitTests\TestData\1.caff");
			var paths = imagingService.SaveCaffImagesAsync(caff);
			return Ok(paths);
        }

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto loginDto)
		{
			try
			{
				return Ok(await authService.LoginAsync(loginDto));
			}
			catch (AuthException authEx)
			{
				return Unauthorized(authEx.Message);
			}
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<ActionResult<TokenDto>> Register([FromBody] RegisterDto registerDto)
		{
			try
			{
				return Ok(await authService.RegisterAsync(registerDto));
			}
			catch (AuthException authEx)
			{
				return BadRequest(authEx.Message);
			}
		}

		[Authorize(Policy = "RequireLogin")]
		//[Authorize]
		[HttpPost("logout")]
		public async Task<ActionResult> Logout()
		{
			await authService.LogoutAsync();
			return Ok();
		}

		[AllowAnonymous]
		[HttpPost("renew")]
		public async Task<ActionResult<TokenDto>> RenewToken([FromQuery] string refreshToken)
		{
			try
			{
				return Ok(await authService.RenewTokenAsync(refreshToken));
			}
			catch (AuthException authEx)
			{
				return BadRequest(authEx.Message);
			}
		}
	}
}
