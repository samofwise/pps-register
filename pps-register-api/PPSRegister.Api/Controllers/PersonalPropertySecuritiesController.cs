using Microsoft.AspNetCore.Mvc;
using PPSRegister.Api.Services;
using PPSRegister.Data.Models;

namespace PPSRegister.Api.Controllers;

[ApiController]
[Route("personal-property-securities")]
public class PersonalPropertySecuritiesController(
    IPersonalPropertySecurityUploadService _personalPropertySecurityUploadService
) : BaseController
{

    
    [HttpGet("uploads")]
    [ProducesResponseType(typeof(List<PersonalPropertySecurityUpload>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        var uploads = await _personalPropertySecurityUploadService.GetUploads(ClientId);

        return Ok(uploads);
    }


    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            var upload = await _personalPropertySecurityUploadService.UploadFile(file, ClientId);
            return Ok(upload);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
