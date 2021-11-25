using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Server.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CryptoDoge.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IParserService parserService;
        private readonly IImagingService imagingService;
        private readonly IdentityService identityService;

        public ImagesController(IParserService parserService, IImagingService imagingService, IdentityService identityService)
        {
            this.parserService = parserService ?? throw new ArgumentNullException(nameof(parserService));
            this.imagingService = imagingService ?? throw new ArgumentNullException(nameof(imagingService));
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CaffDto>> GetCaffByIdAsync(string id)
        {
            var caff = await imagingService.GetCaffByIdAsync(id);
            if (caff != null)
            {
                return Ok(caff);
            }
            return NotFound();
        }

        [HttpPost("upload")]
        public async Task<ActionResult<CaffDto>> Upload([FromForm] IFormFile file)
        {
            try
            {
                using var caffStream = new MemoryStream();
                await file.CopyToAsync(caffStream);
                caffStream.Position = 0;

                var caff = parserService.GetCaffFromMemoryStream(caffStream);
                var currentUser = await identityService.GetCurrentUserAsync();
                var result = await imagingService.SaveCaffImagesAsync(caff, currentUser);

                return Ok(result);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireLogin")]
        public async Task<ActionResult> Delete(string id)
        {
            var currentUser = await identityService.GetCurrentUserAsync();
            await imagingService.DeleteCaffImagesAsync(id, currentUser);
            return NoContent();
        }

        [HttpPost("comment")]
        [Authorize(Policy = "RequireLogin")]
        public async Task<ActionResult> Comment([FromBody] CaffCommentDto caffCommentDto)
        {
            try
            {
                var currentUser = await identityService.GetCurrentUserAsync();
                await imagingService.CommentOnCaff(caffCommentDto, currentUser);
                return Ok();
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
