namespace Application;

using Domain;
using MediatR;
using System.Net.Http;
public class UploadTrackingEvidenceDomainEventHandler(ITrackingRepository trackingRepository, IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork) : INotificationHandler<UploadTrackingEvidenceDomainEvent>
{
    public async Task Handle(UploadTrackingEvidenceDomainEvent notification, CancellationToken cancellationToken)
    {
        var tracking = await trackingRepository.GetTrackingByNumber(notification.TrackingNumber) ?? throw new NotFoundException($"No se encontró el tracking de número {notification.TrackingNumber}");
        var idOrderStatus = notification.OrderStatus.GetId();
        var client = httpClientFactory.CreateClient();

        foreach (var evidence in notification.Evidences)
        {
            var stream = await DownloadEvidence(evidence.Url, client);
            var pathUrl = await UploadFileToBlobStorage(stream, tracking.Id.ToString(), evidence.FileName, evidence.FileType);
            var file = File.Create(tracking.Id, idOrderStatus, evidence.Label, evidence.FileType, pathUrl);

            unitOfWork.Repository<File>().AddEntity(file);
        }

        await unitOfWork.Complete();
    }
    private async Task<MemoryStream> DownloadEvidence(string url, HttpClient client)
    {
        url = "https://static.vecteezy.com/system/resources/thumbnails/083/933/835/small/beautiful-and-inspiring-picture-detailing-a-bright-hot-air-balloon-over-river-pure-cozy-perfect-for-creatives-moods-stock-image-free-photo.jpeg";
        byte[]? pdfBytes = null;
        using var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new ExternalServiceException($"No se pudo descargar el file de la url {url}");

        pdfBytes = await response.Content.ReadAsByteArrayAsync() ?? throw new ExternalServiceException("No se pudo convertir el file en bytes");

        return new MemoryStream(pdfBytes);
    }
    private async Task<string> UploadFileToBlobStorage(MemoryStream stream, string id, string documentName, string extension)
    {
        // var documentNameWithoutExtension = documentName.Replace(extension, "");
        // var formFile = new FormFile(stream, 0, stream.Length, documentNameWithoutExtension, documentName)
        // {
        //     Headers = new HeaderDictionary(),
        //     ContentType = $"application/{extension.Replace(".", "")}"
        // };

        // var command = new UploadFileToBlobStorageCommand(formFile, BlobContainerNameAzureBlobStorageEnum.TrackingEvidence, id);
        // var result = await mediator.Send(command);
        (string BlobContainerName, string BlobName) result = ("Evidences", $"{id}/{Guid.NewGuid()}_{documentName}");
        var pathUrl = $"/api/File/BlobStorage/{result.BlobContainerName}/{result.BlobName}";

        return pathUrl;
    }
}