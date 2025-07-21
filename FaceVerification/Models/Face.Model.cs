namespace FaceVerification.Models
{
    public class Face
    {
        public string ImgData { get; set; }
        public string ImgType { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public string Score { get; set; }
        public string Result { get; set; }
        public string errorMessage { get; set; }
        public string successMessage { get; set; }
    }
}
