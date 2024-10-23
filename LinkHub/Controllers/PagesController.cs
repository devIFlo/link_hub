using AspNetCoreHero.ToastNotification.Abstractions;
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
        private readonly INotyfService _notifyService;

        public PagesController(IPageRepository pageRepository,
            UserManager<ApplicationUser> userManager,
            IUserPagePermissionRepository userPagePermissionRepository,
            INotyfService notifyService)
        {
            _pageRepository = pageRepository;
            _userManager = userManager;
            _userPagePermissionRepository = userPagePermissionRepository;
            _notifyService = notifyService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var pages = _pageRepository.GetPages();
            return View(pages);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Warning("Preencha todos os campos obrigatórios.");
                return RedirectToAction("Index");
            }

            try
            {
                await _pageRepository.Add(page);
                _notifyService.Success("Página adicionada com sucesso.");
            }
            catch (Exception ex)
            {
                _notifyService.Error("Ocorreu um erro ao adicionar a página: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var page = await _pageRepository.GetPageAsync(id);
            if (page == null)
            {
                _notifyService.Error("Página não encontrada!");
            }

            return PartialView("_Edit", page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Warning("Preencha todos os campos obrigatórios.");
                return RedirectToAction("Index");
            }

            try
            {
                await _pageRepository.Update(page);
                _notifyService.Success("Página atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                _notifyService.Error("Ocorreu um erro ao adicionar a página: " + ex.Message);
            }                       

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Permission(int id)
        {
            var page = await _pageRepository.GetPageAsync(id);
            if (page == null)
            {
                _notifyService.Error("Página não encontrada!");
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
            try
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

                _notifyService.Success("Permissões atualizadas com sucesso.");
            }
            catch (Exception ex)
            {
                _notifyService.Error("Não foi possivel atualizar as permissões da página: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await _pageRepository.GetPageAsync(id);
            if(page == null)
            {
                _notifyService.Error("Página não encontrada!");
            }

            return PartialView("_Delete", page);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _pageRepository.Delete(id);
                _notifyService.Success("Página removida com sucesso.");
            }
            catch (Exception ex)
            {
                _notifyService.Success(ex.Message);
            }
            
            return RedirectToAction("Index");
        }
    }
}