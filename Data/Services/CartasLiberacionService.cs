using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UniSportUAQ_API.Data.Models;
using System.IO;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Base;

namespace UniSportUAQ_API.Data.Services
{
    public class CartasLiberacionService : EntityBaseRepository<CartaLiberacion>, ICartasLiberacionService
    {
        private readonly string? _apiKey; 
        private readonly string? _bucket;
        private readonly string? _authPassword;
        private readonly string? _authEmail;

        public CartasLiberacionService(
            AppDbContext context, 
            IConfiguration configuration) : base(context)
        {
            _apiKey = configuration["Firebase:ApiKey"];
            _bucket = configuration["Firebase:Bucket"];
            _authEmail = configuration["Firebase:AuthEmail"];
            _authPassword = configuration["Firebase:AuthPassword"];
        }

        public async Task<string?> UploadLetterAsync(Stream stream, string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(_authEmail, _authPassword);

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            var cancellation = new CancellationTokenSource();

            var storage = new FirebaseStorage(
                _bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true //manejar la cancelación adecuadamente
                }
            );

            var storageReference = storage.Child("uploads").Child("user_letters").Child(fileName);

            try
            {
                // Subir el archivo
                var uploadTask = await storageReference.PutAsync(stream, cancellation.Token);


                // Intentar obtener el enlace de descarga público
                var link = await storageReference.GetDownloadUrlAsync();
                return link; // Devuelve el enlace de descarga público
            }
            catch
            {
                
                return null; // Devolver una cadena vacía o manejar el error de manera diferente
            }
        }

    }
}
