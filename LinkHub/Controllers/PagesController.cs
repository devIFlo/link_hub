using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        private readonly IPageRepository _pageRepository;
        private readonly IUserPagePermissionRepository _userPagePermissionRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PagesController(IPageRepository pageRepository,
            UserManager<ApplicationUser> userManager,
            IUserPagePermissionRepository userPagePermissionRepository)
        {
            _pageRepository = pageRepository;
            _userManager = userManager;
            _userPagePermissionRepository = userPagePermissionRepository;
        }

        public IActionResult Index()
        {
            var pages = _pageRepository.GetPages();
            return View(pages);
        }

        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                await _pageRepository.Add(page);
                return RedirectToAction("Index");
            }

            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var page = await _pageRepository.GetPageAsync(id);
            if (page == null)
            {
                return NotFound();
            }

            return PartialView("_Edit", page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            await _pageRepository.Update(page);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Permission(int id)
        {
            var page = await _pageRepository.GetPageAsync(id);
            if (page == null)
            {
                return NotFound();
            }

            var selectedUserIds = await _userPagePermissionRepository.GetUsersPerPageAsync(id);

            var users = await _userManager.Users.ToListAsync();

            var permissionView = new PermissionViewModel
            {
                PageId = page.Id,
                PageName = page.Name,
                Users = users,
                SelectedUserIds = selectedUserIds
            };

            return PartialView("_Permission", permissionView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Permission(PermissionViewModel permission)
        {
            var pageId = permission.PageId;
            var userIds = permission.SelectedUserIds;

            if (userIds != null)
            {
                await _userPagePermissionRepository.Update(pageId, userIds);
            } 
            else
            {
                await _userPagePermissionRepository.DeleteAllPagePermissions(pageId);
            }                       

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Page page = await _pageRepository.GetPageAsync(id);
            if(page == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", page);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _pageRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}