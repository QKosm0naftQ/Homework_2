using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Local_Server.Controllers
{
    public class UploadImage
    {
        public string Photo { get; set; } = String.Empty;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GalleriesController : ControllerBase
    {
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImage model)
        {
            try
            {
                string fileName = $"{Guid.NewGuid()}.jpg";
                if (model.Photo.Contains(','))
                    model.Photo = model.Photo.Split(',')[1];
                byte[] byteArray = Convert.FromBase64String(model.Photo);

                using Image image = Image.Load(byteArray);
                
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                double compressionFactor = originalWidth * originalHeight > 1920 * 1080 ? 0.2 :
                                           originalWidth * originalHeight > 1280 * 720 ? 0.4 : 0.7;
                
                int newWidth = (int)(originalWidth * compressionFactor);
                int newHeight = (int)(originalHeight * compressionFactor);

                var compressedImage = image.Clone(ctx => ctx.Resize(newWidth, newHeight));

                string folderName = "uploads";
                string pathFolder = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string outputFilePath = Path.Combine(pathFolder, fileName);

                image.Save(outputFilePath);
                return Ok(new { image = $"/images/{fileName}" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }
    }
}
