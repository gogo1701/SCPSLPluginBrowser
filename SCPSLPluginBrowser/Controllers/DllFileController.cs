using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public DllFileController(ApplicationDbContext context, ILogger<DllFileController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, string fileName, string description, string iconUrl)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please select a file to upload.");
                return View();
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".dll")
            {
                ModelState.AddModelError("file", "Invalid file type. Only .dll files are allowed.");
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var dllFile = new DllFile
            {
                FileName = fileName,
                Description = description,
                Icon = iconUrl,
                FileData = memoryStream.ToArray(),
                CreatedAt = DateTime.Now,
                UserId = userId
            };

            try
            {
                _context.DllFiles.Add(dllFile);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"File {fileName} uploaded successfully.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file.");
                ModelState.AddModelError(string.Empty, "An error occurred while uploading the file.");
                return View();
            }
        }

        // GET: Index
        public IActionResult Index(string searchTerm = null)
        {
            var dllFiles = string.IsNullOrWhiteSpace(searchTerm)
                ? _context.DllFiles.ToList()
                : _context.DllFiles
                    .Where(f => EF.Functions.Like(f.FileName, $"%{searchTerm}%") || EF.Functions.Like(f.Description, $"%{searchTerm}%"))
                    .ToList();

            ViewBag.SearchTerm = searchTerm;
            return View(dllFiles);
        }

        public async Task<IActionResult> Profile(string id)
        {
            var user = _context.Users.FirstOrDefault(f => f.Id == id);

            ApplicationUser newUser = (ApplicationUser)user;


            if (user == null)
            {
                _logger.LogWarning($"No file found with ID: {id}");
                return NotFound();
            }

            foreach (var dllfile in _context.DllFiles)
            {
                if (dllfile.UserId == id)
                {
                    newUser.DllFiles.Add(dllfile);
                }
            }

            foreach (var comment in _context.Comments)
            {
                if (comment.UserId == id)
                {
                    newUser.Comments.Add(comment);
                }
            }

            return View(newUser);
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var file = await _context.DllFiles
                .Include(f => f.Likes)
                .Include(f => f.Comments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(file.UserId);
            file.User = user;
            return View(file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmin(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.Message = "User ID cannot be empty.";
                var admins = _userManager.GetUsersInRoleAsync("Admin").Result;
                return View(admins); 
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Message = "User not found.";
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                return View(admins); 
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} has been promoted to Admin.");
                return RedirectToAction("Index", "DllFile");
            }
            else
            {
                _logger.LogWarning($"Failed to promote user {user.UserName} to Admin.");
                ViewBag.Message = "An error occurred while promoting the user to Admin.";
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                return View(admins); 
            }
        }



        [HttpGet]
        public async Task<IActionResult> AddAdmin()
        {
            var usersInRole = await new UserStore<ApplicationUser>(_context).GetUsersInRoleAsync("Admin");

            ICollection<ApplicationUser> users = usersInRole;

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.Message = "User ID cannot be empty.";
                return RedirectToAction("AddAdmin", "DllFile");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Message = "User not found.";
                return RedirectToAction("AddAdmin", "DllFile");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} has been promoted to Admin.");
                return RedirectToAction("AddAdmin", "DllFile");
            }
            else
            {
                _logger.LogWarning($"Failed to promote user {user.UserName} to Admin.");
                ViewBag.Message = "An error occurred while promoting the user to Admin.";
                return RedirectToAction("AddAdmin", "DllFile");
            }
        }

        // POST: AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int fileId, string commentText)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var dllFile = await _context.DllFiles.FindAsync(fileId);
            if (dllFile == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                UserId = userId,
                DllFileId = fileId,
                CommentText = commentText,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = fileId });
        }

        // POST: Like
        [HttpPost]
        public async Task<IActionResult> Like(int fileId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var dllFile = await _context.DllFiles.Include(f => f.Likes).FirstOrDefaultAsync(f => f.Id == fileId);
            if (dllFile == null)
            {
                return NotFound();
            }

            var existingLike = dllFile.Likes.FirstOrDefault(l => l.UserId == userId);
            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
            }
            else
            {
                var like = new Like { DllFileId = fileId, UserId = userId };
                _context.Likes.Add(like);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = fileId });
        }

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

        // POST: AdminDelete
        [Authorize(Roles = "Owner")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AdminDelete(int id)
        {
            var dllFile = await _context.DllFiles
                .Include(f => f.Likes)
                .Include(f => f.Comments)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (dllFile == null)
            {
                return NotFound();
            }

            _context.Likes.RemoveRange(dllFile.Likes);
            _context.Comments.RemoveRange(dllFile.Comments);
            _context.DllFiles.Remove(dllFile);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
