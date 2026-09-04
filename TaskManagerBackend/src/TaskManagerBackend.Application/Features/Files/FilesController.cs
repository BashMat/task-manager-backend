#region Usings

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using TaskManagerBackend.Application.Features.Files.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.DataAccess.Features.Files;

#endregion

namespace TaskManagerBackend.Application.Features.Files;

[ApiController]
[Route("api/files")]
[EnableCors]
[Authorize]
public class FileController(IFilesRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllFiles(CancellationToken cancellationToken)
    {
        ServiceResponse<GetAllFilesResponse> response = new GetAllFilesResponse
                                                        {
                                                            FileNames = repository.GetAllFiles()
                                                        };
        
        return HandleServiceResponse(response);
    }
    
    [AcceptVerbs("QUERY")]
    public async Task<IActionResult> GetUserDataById([FromBody] GetFileRequest request,
                                                     CancellationToken cancellationToken)
    {
        string path = repository.GetFilePath(request.FileName);
        
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        FileStream stream = new(path, FileMode.Open, FileAccess.Read);
        return File(stream, contentType, request.FileName);
    }
}