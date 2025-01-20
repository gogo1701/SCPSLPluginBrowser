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
        private readonly UserManager<ApplicationUser> _manager;


        public DllFileController(ApplicationDbContext context, ILogger<DllFileController> logger, UserManager<ApplicationUser> manager)
        {
            _manager = manager;
            _context = context;
            _logger = logger;
        }

        public IActionResult Upload()
        {
            return View();
        }

        public async Task<IActionResult> Profile(string id)
        {

            var user = _context.Users.FirstOrDefault(f => f.Id == id);  
            

            if (user == null)
            {
                _logger.LogWarning($"No file found with ID: {id}");
                return NotFound();
            }

            return View(user);
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

            var user = User.Identity.Name;

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
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogDebug($"Attempting to retrieve file with ID: {id}");

            var file = await _context.DllFiles
                .Include(f => f.Likes) // Include Likes
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file == null)
            {
                _logger.LogWarning($"No file found with ID: {id}");
                return NotFound();
            }

            _logger.LogDebug($"File found: {file.FileName}");
            var user = await _manager.FindByIdAsync(file.UserId);
            file.User = user;

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

        [HttpPost]
        public async Task<IActionResult> Like(int fileId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Ensure the user is logged in
            }

            var dllFile = await _context.DllFiles.Include(f => f.Likes).FirstOrDefaultAsync(f => f.Id == fileId);
            if (dllFile == null)
            {
                return NotFound();
            }

            // Check if the user has already liked the file
            var existingLike = dllFile.Likes.FirstOrDefault(l => l.UserId == userId);
            if (existingLike != null)
            {
                // Remove the like
                _context.Likes.Remove(existingLike);
            }
            else
            {
                // Add a new like
                var like = new Like
                {
                    DllFileId = fileId,
                    UserId = userId,
                };

                _context.Likes.Add(like);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = fileId });
        }


    }
}
