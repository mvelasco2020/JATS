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

        public async Task<IActionResult> UpdateProfileInfo(JTUser formInput)
        {

            try
            {
                TempData["SuccessMessage"] = "Successfully saved changes to:";
                var userId = await _userManager.GetUserAsync(User);
                JTUser currentUser = _context.Users.FirstOrDefault(u => u.Id == userId.Id);
                if (currentUser.FirstName != formInput.FirstName)
                {
                    currentUser.FirstName = formInput.FirstName;
                    TempData["SuccessMessage"] += " First Name";
                }
                if (currentUser.LastName != formInput.LastName)
                {
                    currentUser.LastName = formInput.LastName;
                    TempData["SuccessMessage"] += " Last Name";
                }
                if (currentUser.PhoneNumber != formInput.PhoneNumber)
                {
                    currentUser.PhoneNumber = formInput.PhoneNumber;
                    TempData["SuccessMessage"] += " Phone Number";
                }
                await _context.SaveChangesAsync();
                return LocalRedirect("/Identity/Account/Manage");
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "Something went wrong while saving the changes, Please check verify if you First Name, Last Name and Phonenumber is valid";
                return LocalRedirect("/Identity/Account/Manage");
            }


        }

    }
}
