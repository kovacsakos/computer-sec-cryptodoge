using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Server.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoDoge.Server.Controllers
{
    [Route("api/images")]
    [ApiController]
    [Produces("application/json")]
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CaffDto>>> GetCaffs()
        {
            return Ok(await imagingService.GetCaffsAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CaffDto>> GetCaffByIdAsync(string id)
        {
            var caff = await imagingService.GetCaffByIdAsync(id);
            if (caff != null)
            {
                return Ok(caff);
            }
            return NotFound();
        }

        [HttpPost("searchByCaption")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CaffDto>>> SearchCaffsByCaption([FromBody] SearchByCaptionDto searchByCaptionDto)
        {
            if (searchByCaptionDto is null || string.IsNullOrEmpty(searchByCaptionDto.Query))
            {
                return Ok(await imagingService.GetCaffsAsync());
            }

            return Ok(await imagingService.SearchCaffsByCaption(searchByCaptionDto.Query));
        }

        [HttpPost("searchByTags")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CaffDto>>> SearchCaffsByTags([FromBody] SearchByTagsDto searchByTagsDto)
        {
            if (searchByTagsDto is null || !searchByTagsDto.QueryTags.Any())
            {
                return Ok(await imagingService.GetCaffsAsync());
            }

            return Ok(await imagingService.SearchCaffsByTags(searchByTagsDto.QueryTags));
        }

        [HttpPost("upload")]
        [Authorize(Policy = "RequireLogin", Roles = "Admin, User")]
        public async Task<ActionResult<CaffDto>> UploadCaff([FromForm] IFormFile file)
        {
            try
            {
                using var caffStream = new MemoryStream();
                await file.CopyToAsync(caffStream);
                caffStream.Position = 0;

                var caff = parserService.GetCaffFromMemoryStream(caffStream);
                var user = await identityService.GetCurrentUserAsync();
                var result = await imagingService.SaveCaffImagesAsync(caff, user);

                return Ok(result);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireLogin", Roles = "Admin")]
        public async Task<ActionResult> DeleteCaff(string id)
        {
            await imagingService.DeleteCaffImagesAsync(id);
            return NoContent();
        }

        [HttpPost("comment")]
        [Authorize(Policy = "RequireLogin", Roles = "Admin, User")]
        public async Task<ActionResult> PostComment([FromBody] CaffCommentDto caffCommentDto)
        {
            try
            {
                await imagingService.AddCaffCommentAsync(caffCommentDto);
                return Ok();
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("comment/{id}")]
        [Authorize(Policy = "RequireLogin", Roles = "Admin")]
        public async Task<ActionResult> UpdateComment(string id, [FromBody] CaffCommentUpdateDto caffCommentUpdateDto)
        {
            try
            {
                await imagingService.UpdateCommentOnCaffAsync(id, caffCommentUpdateDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("comment/{id}")]
        [Authorize(Policy = "RequireLogin", Roles = "Admin")]
        public async Task<ActionResult> DeleteComment(string id)
        {
            await imagingService.DeleteCaffCommentAsync(id);
            return NoContent();
        }
    }
}
