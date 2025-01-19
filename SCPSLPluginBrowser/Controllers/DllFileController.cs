using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SCPSLPluginBrowser.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SCPSLPluginBrowser.Controllers
{
    public class DllFileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DllFileController> _logger;


        public DllFileController(ApplicationDbContext context, ILogger<DllFileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, string fileName, string description, string iconUrl)
        {
            _logger.LogDebug("Upload action triggered.");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file selected.");
                ModelState.AddModelError("file", "Please select a file to upload.");
                return View();
            }

            var validExtensions = new[] { ".dll" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!validExtensions.Contains(fileExtension))
            {
                _logger.LogWarning($"Invalid file type: {fileExtension}. Expected .dll file.");
                ModelState.AddModelError("file", "Invalid file type. Please upload a DLL file.");
                return View();
            }

            // Get UserId from the logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not logged in.");
                return Unauthorized();
            }

            ApplicationUser currentUser = await _context.Users.OfType<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == userId);

            // Handle the file data
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] fileData = memoryStream.ToArray();

                // Create the DllFile object and assign the current user to it
                var dllFile = new DllFile
                {
                    FileName = fileName,
                    Description = description,
                    Icon = iconUrl,
                    FileData = fileData,
                    CreatedAt = DateTime.Now,
                    UserId = userId,  // Set the UserId for the DllFile (important for database relations)
                    User = currentUser
                };

                _context.DllFiles.Add(dllFile);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"File {fileName} uploaded successfully.");
                return RedirectToAction("Index");
            }
        }



        public IActionResult Index(string searchTerm = null)
        {
            var dllFiles = string.IsNullOrWhiteSpace(searchTerm)
                ? _context.DllFiles.ToList()
                : _context.DllFiles.Where(f => f.FileName.Contains(searchTerm) || f.Description.Contains(searchTerm)).ToList();

            ViewBag.SearchTerm = searchTerm;
            return View(dllFiles);
        }

        // View detailed info about a specific file
        public IActionResult Details(int id)
        {
            _logger.LogDebug($"Attempting to retrieve file with ID: {id}");

            // Test with a hardcoded id to see if files exist
            var file = _context.DllFiles.FirstOrDefault(f => f.Id == id);  // Change 1 to an actual ID from your database

            if (file == null)
            {
                _logger.LogWarning($"No file found with ID: {id}");
                return NotFound();
            }

            _logger.LogDebug($"File found: {file.FileName}");
            return View(file);
        }



        // File download
        public IActionResult Download(int id)
        {
            var file = _context.DllFiles.FirstOrDefault(f => f.Id == id);
            if (file == null)
            {
                return NotFound();
            }

            var fileName = file.FileName.EndsWith(".dll") ? file.FileName : $"{file.FileName}.dll";
            return File(file.FileData, "application/octet-stream", fileName);
        }

    }
}
