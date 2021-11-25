using CryptoDoge.BLL.Dtos;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.Model.Entities;
using CryptoDoge.ParserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoDoge.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IParserService parserService;
        private readonly IImagingService imagingService;

        public ImagesController(IParserService parserService, IImagingService imagingService)
        {
            this.parserService = parserService ?? throw new ArgumentNullException(nameof(parserService));
            this.imagingService = imagingService ?? throw new ArgumentNullException(nameof(imagingService));
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
                var result = await imagingService.SaveCaffImagesAsync(caff);

                return Ok(result);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await imagingService.DeleteCaffImagesAsync(id);
            return NoContent();
        }
    }
}
