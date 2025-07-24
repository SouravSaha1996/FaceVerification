using FaceAiSharp;
using FaceVerification.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace FaceVerification.Controllers
{
    public class FaceController : Controller
    {
        private readonly IFaceDetector _detector = FaceAiSharpBundleFactory.CreateFaceDetector();
        private readonly IFaceEmbeddingsGenerator _recognizer = FaceAiSharpBundleFactory.CreateFaceEmbeddingsGenerator();

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ValidateFace(string modelParameter)
        {
            string data = string.Empty;
            FaceModal Model = JsonConvert.DeserializeObject<FaceModal>(modelParameter) ?? new FaceModal();

            if (String.IsNullOrEmpty(Model.ImgData))
                return View((object)"No file selected");

            var knownPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/faces/user{Model.UserId}.jpg");
            if (!System.IO.File.Exists(knownPath))
                return View((object)"Known image missing");
            IFormFile uploadedPhoto = Base64ToIFormFile(Model.ImgData);

            using var knownStream = System.IO.File.OpenRead(knownPath);
            using var testStream = uploadedPhoto.OpenReadStream();

            using var knownImg = Image.Load<Rgb24>(knownStream);
            using var testImg = Image.Load<Rgb24>(testStream);

            var knownFaces = _detector.DetectFaces(knownImg);
            var testFaces = _detector.DetectFaces(testImg);

            if (!knownFaces.Any() || !testFaces.Any())
                return View((object)"Face not found in one or both images");

            // Align faces in-place
            _recognizer.AlignFaceUsingLandmarks(knownImg, knownFaces.First().Landmarks);
            _recognizer.AlignFaceUsingLandmarks(testImg, testFaces.First().Landmarks);

            var embed1 = _recognizer.GenerateEmbedding(knownImg);
            var embed2 = _recognizer.GenerateEmbedding(testImg);

            if (embed1 == null || embed2 == null)
                return View((object)"Failed to extract embeddings");

            var similarity = CosineSimilarity(embed1, embed2);

            Model.Score = similarity.ToString("F4");
            if (similarity > 0.6) // Adjust threshold as needed
            {
                Model.Result = "1"; // Match
                Model.successMessage = "Face matched successfully";
            }
            else
            {
                Model.Result = "0"; // No match
                Model.errorMessage = "Face does not match";
            }
            data = JsonConvert.SerializeObject(Model);
            return Json(data);
        }

        private static double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            double dot = 0.0;
            double magA = 0.0;
            double magB = 0.0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dot += vectorA[i] * vectorB[i];
                magA += vectorA[i] * vectorA[i];
                magB += vectorB[i] * vectorB[i];
            }

            return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
        }
        private IFormFile Base64ToIFormFile(string base64String, string fileName = "upload.jpg")
        {
            // Remove data:image/...;base64, if present
            var base64Parts = base64String.Split(',');
            var actualBase64 = base64Parts.Length > 1 ? base64Parts[1] : base64Parts[0];

            byte[] bytes = Convert.FromBase64String(actualBase64);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }
}
