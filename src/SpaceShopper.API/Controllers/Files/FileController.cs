using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceShopper.API.Controllers.Common;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Common.Errors;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Common.Settings;
using SpaceShopper.Application.Dtos.Common;
using SpaceShopper.Application.Interfaces.Iservices.Common;

namespace SpaceShopper.API.Controllers.Files
{
    [ApiController]
    [Route("api/v1/files")]
    [Authorize]
    public sealed class FileController(IFileService fileService) : BaseController
    {
        private readonly IFileService _fileService = fileService;

        /// <summary>Multipart form field name must be <c>file</c> (Postman compatibility).</summary>
        [HttpPost("upload")]
        [RequestSizeLimit(StorageOptions.MaxUploadRequestBytes)]
        public async Task<IActionResult> Upload([FromForm] IFormFile? file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                throw new ValidationException(ErrorCodes.File.MissingOrEmpty, ErrorMessages.File.MissingOrEmpty);
            }

            await using var stream = file.OpenReadStream();
            var result = await _fileService.UploadAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    file.Length,
                    GetCurrentUserId(),
                    cancellationToken)
                .ConfigureAwait(false);

            return Ok(ApiResponse<FileUploadResponse>.Ok(result));
        }
    }
}
