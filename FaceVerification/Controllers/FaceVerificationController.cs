using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace FaceVerification.Controllers
{
    public class FaceVerificationController : Controller
    {
        private readonly string _modelPath = "wwwroot/models"; // models folder path
        private readonly string _storedImagePath = "wwwroot/faces/user1.jpg"; // stored user face

        public IActionResult Index() => View(); // Upload form

        [HttpPost]
        public IActionResult Login(IFormFile uploadedImage)
        {
                return BadRequest("No image uploaded");
            //if (uploadedImage == null || uploadedImage.Length == 0)

            // Create face recognizer
            //using var faceRecognition = FaceRecognition.Create(_modelPath);

            //// Load stored image (registered face)
            //using var knownImage = FaceRecognition.LoadImageFile(_storedImagePath);
            //var knownEncoding = faceRecognition.FaceEncodings(knownImage).FirstOrDefault();
            //if (knownEncoding == null)
            //    return Unauthorized("No face found in stored image");

            //// Load uploaded image
            //using var uploadedStream = uploadedImage.OpenReadStream();

            //using Bitmap bitmap = new Bitmap(uploadedStream);
            //using var unknownImage = FaceRecognition.LoadImage(bitmap);
            //var unknownEncoding = faceRecognition.FaceEncodings(unknownImage).FirstOrDefault();
            //if (unknownEncoding == null)
            //    return Unauthorized("No face found in uploaded image");

            //if (FaceRecognition.CompareFaces(new[] { knownEncoding }, unknownEncoding, tolerance: 0.4).First())
            //{
            //    // TODO: Set login session or redirect
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    return Unauthorized("Face does not match");
            //}
        }
    }
}