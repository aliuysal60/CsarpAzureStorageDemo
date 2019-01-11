using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageCSharpDemo
{
    public static class Program
    {
        // Create a folder and add some images to it 
        const string FolderPath = @"C:\testStorage";

        // this is the conn string found within your storage aggaount under blob storage access keys
        // TODO - move this to azure keyvault in next steps
        private const string connstring = "<YourConnectionString>";

        public static void Main()
        {
            Console.WriteLine("Azure Blob Storage Resim Yükleme Demosu");
            Console.WriteLine();
            ProcessAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit the application.");
            Console.ReadLine();
        }

        private static async Task ProcessAsync()
        {
            string storageConnectionString = connstring;

            if (CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount))
            {
                try
                {
                    var blobClient = storageAccount.CreateCloudBlobClient();

                    var cloudBlobContainer = blobClient.GetContainerReference("xmasimages-container");
                    await cloudBlobContainer.CreateIfNotExistsAsync();

                    var permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    foreach (var filePath in Directory.GetFiles(FolderPath, "*.*", SearchOption.AllDirectories))
                    {
                        var blob = cloudBlobContainer.GetBlockBlobReference(filePath);
                        await blob.UploadFromFileAsync(filePath);

                        Console.WriteLine("Uploaded {0}", filePath);
                    }

                    Console.WriteLine("Yükleme işlemi tamamlandı");
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Servisden hata döndü: {0}", ex.Message);
                }
            }
        }
    }
}
