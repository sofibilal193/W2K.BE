using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using W2K.Files.Application.Commands;
using W2K.Files.Application.Queries;
using W2K.Common.Application.Controllers;
using W2K.Common.Application.ModelBinders;
using W2K.Files.Application.DTOs;
using W2K.Files.Application;
using W2K.Common.Application.Auth;
using W2K.Common.Application.DTOs.Files;

namespace W2K.Files.Controllers.Files;

[Route("api/v{version:apiVersion}/offices/{officeId}/files")]
[ApiController]
public class FilesController : BaseApiController
{
    /// <summary>
    /// Upserts office files.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="command">The command containing office file details.</param>
    /// <param name="documents">The uploaded files.</param>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<IEnumerable<int>>> UpsertOfficesFilesAsync(
        int officeId,
        [ModelBinder(BinderType = typeof(JsonModelBinder))] UpsertOfficeFilesCommand command,
        [FromForm] IReadOnlyCollection<IFormFile> documents)
    {
        command.SetOfficeId(officeId);
        command.Documents = documents;

        foreach (var file in command.Files)
        {
            var doc = documents.FirstOrDefault(x => x.FileName == file.Label);
            if (doc is not null)
            {
                byte[] fileContent;
                using var memoryStream = new MemoryStream();
                await doc.CopyToAsync(memoryStream);
                fileContent = memoryStream.ToArray();
                file.SetFile(doc.ContentType, fileContent);
            }
        }

        var fileIds = await Mediator.Send(command);
        return Ok(fileIds);
    }

    /// <summary>
    /// Gets files for an office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <returns>A list of files for the given office.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OfficeFilesDto>>> GetOfficeFilesAsync(int officeId)
    {
        var files = await Mediator.Send(new GetOfficeFilesQuery(officeId));
        return Ok(files);
    }

    /// <summary>
    /// Gets a specific file for an office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="fileId">The ID of the file.</param>
    /// <returns>The file with the given ID for the specified office.</returns>
    [HttpGet("{fileId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetOfficeFileAsync(int officeId, int fileId)
    {
        var file = await Mediator.Send(new GetOfficeFileQuery(officeId, fileId));

        return file?.Content is null || file.FileType is null || file.Name is null
            ? NotFound()
            : new FileStreamResult(
            new MemoryStream(file.Content),
            file.FileType)
            {
                FileDownloadName = file.Name
            };
    }

    /// <summary>
    /// Delete a specific file for an office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="fileId">The ID of the file.</param>
    /// <returns>No content if the file was successfully deleted.</returns>
    [HttpDelete("{fileId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteOfficeFileAsync(int officeId, int fileId)
    {
        await Mediator.Send(new DeleteOfficeFilesCommand(officeId, fileId));
        return NoContent();
    }

    /// <summary>
    /// Upload files to a message thread.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="threadId">The ID of the message thread.</param>
    /// <param name="files">The uploaded files.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>List of uploaded file information.</returns>
    [ProducesResponseType(typeof(MessageFileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("threads/{threadId}/files")]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.UploadOfficeFiles)]
    public async Task<ActionResult<MessageFileDto>> UploadMessageThreadFilesAsync(
        int officeId,
        int threadId,
        [FromForm] IReadOnlyCollection<IFormFile> files,
        CancellationToken cancel = default)
    {
        var fileCommands = new List<MessageFilesCommand>();

        foreach (var file in files)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancel);
            var fileContent = memoryStream.ToArray();

            fileCommands.Add(new MessageFilesCommand(
                file.FileName,
                file.ContentType,
                fileContent));
        }

        var response = await Mediator.Send(
            new UpsertMessageFilesCommand(
            officeId,
            threadId,
            false,
            fileCommands),
            cancel);

        return Ok(response);
    }

    /// <summary>
    /// Upload files to a Loan.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="loanAppId">The ID of the loan application.</param>
    /// <param name="files">The uploaded files.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>List of uploaded file IDs.</returns>
    [ProducesResponseType(typeof(IReadOnlyCollection<FileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("loanApps/{loanAppId}")]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.UploadOfficeFiles)]
    public async Task<ActionResult<IReadOnlyCollection<FileDto>>> UpsertLoanAppFilesAsync(
        int officeId,
        int loanAppId,
        [FromForm] IReadOnlyCollection<IFormFile> files,
        CancellationToken cancel = default)
    {
        var fileCommands = new List<LoanAppFileCommand>();

        foreach (var file in files)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancel);
            var fileContent = memoryStream.ToArray();

            fileCommands.Add(new LoanAppFileCommand(file.FileName, file.ContentType, fileContent));
        }

        var response = await Mediator.Send(new UpsertLoanAppFilesCommand(officeId, loanAppId, fileCommands), cancel);

        return Ok(response);
    }

    /// <summary>
    /// Gets files for a Loan.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="loanAppId">The ID of Loan App</param>
    /// <returns>A list of files for the given Loan App.</returns>
    [HttpGet("loanApps/{loanAppId}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<FileDto>), StatusCodes.Status200OK)]
    [HasPermission(Permissions.ViewOfficeFiles)]
    public async Task<ActionResult<IReadOnlyCollection<FileDto>>> GetLoanAppFilesAsync(int officeId, int loanAppId)
    {
        var files = await Mediator.Send(new GetLoanAppFilesQuery(officeId, loanAppId));
        return Ok(files);
    }

    /// <summary>
    /// Upload files to a Loan.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="loanId">The ID of the loan.</param>
    /// <param name="files">The uploaded files.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>List of uploaded file IDs.</returns>
    [ProducesResponseType(typeof(IReadOnlyCollection<FileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("loans/{loanId}")]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.UploadOfficeFiles)]
    public async Task<ActionResult<IReadOnlyCollection<FileDto>>> UpsertLoanFilesAsync(
        int officeId,
        int loanId,
        [FromForm] IReadOnlyCollection<IFormFile> files,
        CancellationToken cancel = default)
    {
        var fileCommands = new List<LoanFileCommand>();

        foreach (var file in files)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancel);
            var fileContent = memoryStream.ToArray();

            fileCommands.Add(new LoanFileCommand(file.FileName, file.ContentType, fileContent));
        }

        var response = await Mediator.Send(new UpsertLoanFilesCommand(officeId, loanId, fileCommands), cancel);

        return Ok(response);
    }

    /// <summary>
    /// Gets files for a Loan.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="loanId">The ID of the loan.</param>
    /// <returns>A list of files for the given Loan.</returns>
    [HttpGet("loans/{loanId}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<FileDto>), StatusCodes.Status200OK)]
    [HasPermission(Permissions.ViewOfficeFiles)]
    public async Task<ActionResult<IReadOnlyCollection<FileDto>>> GetLoanFilesAsync(int officeId, int loanId)
    {
        var files = await Mediator.Send(new GetLoanFilesQuery(officeId, loanId));
        return Ok(files);
    }
}
