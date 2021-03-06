using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Exceptions;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Server.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
        private readonly IIdentityService identityService;

        public ImagesController(IParserService parserService, IImagingService imagingService, IIdentityService identityService)
        {
            this.parserService = parserService ?? throw new ArgumentNullException(nameof(parserService));
            this.imagingService = imagingService ?? throw new ArgumentNullException(nameof(imagingService));
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        [Authorize(Policy = "RequireLogin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaffDto>>> GetCaffs()
        {
            return Ok(await imagingService.GetCaffsAsync());
        }

        [Authorize(Policy = "RequireLogin")]
        [HttpGet("{caffId}")]
        public async Task<ActionResult<CaffDto>> GetCaffByIdAsync(string caffId)
        {
            var caff = await imagingService.GetCaffByIdAsync(caffId);
            if (caff != null)
            {
                return Ok(caff);
            }
            return NotFound();
        }

        [Authorize(Policy = "RequireLogin")]
        [HttpPost("searchByCaption")]
        public async Task<ActionResult<IEnumerable<CaffDto>>> SearchCaffsByCaption([FromBody] SearchByCaptionDto searchByCaptionDto)
        {
            if (searchByCaptionDto is null || string.IsNullOrEmpty(searchByCaptionDto.Query))
            {
                return Ok(await imagingService.GetCaffsAsync());
            }

            return Ok(await imagingService.SearchCaffsByCaption(searchByCaptionDto.Query));
        }

        [Authorize(Policy = "RequireLogin")]
        [HttpPost("searchByTags")]
        public async Task<ActionResult<IEnumerable<CaffDto>>> SearchCaffsByTags([FromBody] SearchByTagsDto searchByTagsDto)
        {
            if (searchByTagsDto is null || !searchByTagsDto.QueryTags.Any())
            {
                return Ok(await imagingService.GetCaffsAsync());
            }

            return Ok(await imagingService.SearchCaffsByTags(searchByTagsDto.QueryTags));
        }

        [Authorize(Policy = "RequireLogin")]
        [HttpPost("upload")]
        public async Task<ActionResult<CaffDto>> UploadCaff([FromForm] IFormFile file)
        {
            try
            {
                var extension = Path.GetExtension(file.FileName);
                if (extension != ".caff")
                {
                    throw new Exception("Invalid file extension.");
                }

                using var caffStream = new MemoryStream();
                await file.CopyToAsync(caffStream);
                caffStream.Position = 0;

                var caff = parserService.GetCaffFromMemoryStream(caffStream);
                var user = await identityService.GetCurrentUserAsync();
                var result = await imagingService.SaveCaffAsync(caff, user, file);


                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireLogin")]
        [HttpGet("{caffId}/download")]
        public async Task<IActionResult> DownloadCaff(string caffId)
        {
            try
            {
                var fileBytes = await imagingService.GetRawCaffByIdAsync(caffId);
                return File(fileBytes, MediaTypeNames.Application.Octet, $"{caffId}.caff");
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{caffId}")]
        public async Task<ActionResult> DeleteCaff(string caffId)
        {
            await imagingService.DeleteCaffImagesAsync(caffId);
            return NoContent();
        }

        [Authorize(Policy = "RequireLogin")]
        [HttpGet("comment/{id}")]
        public async Task<ActionResult<CaffCommentReturnDto>> GetComment(string id)
        {
            try
            {
                return Ok(await imagingService.GetCaffCommentByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireLogin")]
        [HttpPost("comment/{caffId}")]
        public async Task<ActionResult<string>> PostComment(string caffId, [FromBody] CaffCommentDto caffCommentDto)
        {
            try
            {
                var user = await identityService.GetCurrentUserAsync();

                return Ok(await imagingService.AddCaffCommentAsync(caffId, caffCommentDto.Comment, user));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("comment/{caffCommentId}")]
        public async Task<ActionResult> UpdateComment(string caffCommentId, [FromBody] CaffCommentDto caffCommentDto)
        {
            try
            {
                await imagingService.UpdateCommentOnCaffAsync(caffCommentId, caffCommentDto.Comment);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("comment/{caffCommentId}")]
        public async Task<ActionResult> DeleteComment(string caffCommentId)
        {
            await imagingService.DeleteCaffCommentAsync(caffCommentId);
            return NoContent();
        }
    }
}
