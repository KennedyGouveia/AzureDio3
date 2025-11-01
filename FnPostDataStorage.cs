using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CatalogoAzure;

public class FnPostDataStorage
{
    private readonly ILogger<FnPostDataStorage> _logger;

    public FnPostDataStorage(ILogger<FnPostDataStorage> logger)
    {
        _logger = logger;
    }

    [Function("dataStorage")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("Preocessando a imagem no storage...");
       
        if (!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
        {
            _logger.LogWarning("Cabeçalho 'file-type' ausente na requisição.");
            return new BadRequestObjectResult("Cabeçalho 'file-type' é obrigatório.");
        }
        var fileType = fileTypeHeader.ToString();
        var form = await req.ReadFormAsync();
        var file = form.Files.GetFile("file");
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("Nenhum arquivo foi enviado na requisição.");
            return new BadRequestObjectResult("Arquivo é obrigatório.");
        }
        string connectionstring = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        string containerName = fileType;

        BlobClient blobClient = new BlobClient(connectionstring, containerName, file.FileName);
        BlobContainerClient containerClient = new BlobContainerClient(connectionstring, containerName);
        
        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
        
        string blobName = file. FileName;
        var blob = containerClient. GetBlobClient(blobName);
        
        using (var stream = file.OpenReadStream())
        {
            await blob. UploadAsync(stream, true);
        }
        _logger. LogInformation($"Arquivo {file. FileName} armazenado com sucesso");
        return new OkObjectResult(new
        {
            Message = "Arquivo armazenado com sucesso",
            blobUri = blob.Uri.ToString()
        });
    }
}
