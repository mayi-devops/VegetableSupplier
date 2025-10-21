using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using VegetableSupplier.Models;

namespace VegetableSupplier.Services;

public class GoogleService
{
    private const string AuthorizedUsersSpreadsheetId = "YOUR_SPREADSHEET_ID";
    private const string AuthorizedUsersRange = "Users!A:B"; // Column A: Email, Column B: Status
    private DriveService _driveService;
    private SheetsService _sheetsService;
    private readonly DatabaseService _database;

    public GoogleService(DatabaseService database)
    {
        _database = database;
    }

    public async Task InitializeServicesAsync(string accessToken)
    {
        var credential = GoogleCredential.FromAccessToken(accessToken);
        
        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Vegetable Supplier"
        });

        _sheetsService = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Vegetable Supplier"
        });
    }

    public async Task<bool> IsUserAuthorizedAsync(string email)
    {
        try
        {
            var request = _sheetsService.Spreadsheets.Values.Get(AuthorizedUsersSpreadsheetId, AuthorizedUsersRange);
            var response = await request.ExecuteAsync();
            
            if (response.Values != null)
            {
                foreach (var row in response.Values)
                {
                    if (row.Count > 1 && row[0].ToString() == email)
                    {
                        return row[1].ToString().ToLower() == "authorized";
                    }
                }
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> UploadInvoicePdfAsync(string filePath, string fileName)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = fileName,
            MimeType = "application/pdf"
        };

        using var stream = new FileStream(filePath, FileMode.Open);
        var request = _driveService.Files.Create(fileMetadata, stream, "application/pdf");
        request.Fields = "id";
        
        var response = await request.UploadAsync();
        if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
            throw new Exception("Failed to upload file to Google Drive");

        return request.ResponseBody.Id;
    }

    public async Task<Stream> DownloadInvoicePdfAsync(string fileId)
    {
        var request = _driveService.Files.Get(fileId);
        var stream = new MemoryStream();
        await request.DownloadAsync(stream);
        stream.Position = 0;
        return stream;
    }
}