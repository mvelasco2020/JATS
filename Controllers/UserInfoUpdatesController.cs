using JATS.Data;
using JATS.Models;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JATS.Controllers
{
    public class UserInfoUpdatesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<JTUser> _userManager;
        private readonly IFileService _fileService;

        public UserInfoUpdatesController(ApplicationDbContext context,
                                        UserManager<JTUser> userManager,
                                        IFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<IActionResult> UpdateAvatar(JTUser formInput)
        {
            if (formInput.AvatarFormFile is not null)
            {
                try
                {
                    var userId = await _userManager.GetUserAsync(User);
                    JTUser currentUser = _context.Users.FirstOrDefault(u => u.Id == userId.Id);
                    currentUser.AvatarData = await _fileService
                        .ConvertFileToByteArrayAsync(formInput.AvatarFormFile);
                    currentUser.AvatarFileName = formInput.AvatarFormFile.FileName;
                    currentUser.AvatarFileContentType = formInput.AvatarFormFile.ContentType;
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return View("Error");
                }
            }

            return LocalRedirect("/Identity/Account/Manage");
        }
    }
}
