using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.Services;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class LinksController : Controller
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IPageRepository _pageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotyfService _notyfService;
        private readonly ImageStorage _imageStorage;
        private readonly IMapper _mapper;

        public LinksController(ILinkRepository linkRepository,
            ICategoryRepository categoryRepository,
            IPageRepository pageRepository,
            UserManager<ApplicationUser> userManager,
            INotyfService notyfService,
            ImageStorage imageStorage,
            IMapper mapper)
        {
            _linkRepository = linkRepository;
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
            _userManager = userManager;
            _notyfService = notyfService;
            _imageStorage = imageStorage;
            _mapper = mapper;
            
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Link> links = await _linkRepository.GetLinksPerUserAsync(userId);

            return View(links);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var link = await _linkRepository.GetLinkAsync(id);
            if (link == null)
            {
                return Json(new { message = "Página não encontrada!" });
            }

            return PartialView("_Details", link);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var pages = await _pageRepository.GetPagePerUserAsync(userId);            

            var linkView = new LinkViewModel
            {
                Pages = pages,
                Categories = new List<Category>()
            };
                        
            return PartialView("_Create", linkView);
        }

        [HttpGet]
        public async Task<IActionResult> FilterCategories(int pageId)
        {
            var categories = await _categoryRepository.GetCategoriesPerPageAsync(pageId);

            return Json(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LinkViewModel linkViewModel)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Warning("Preencha todos os campos obrigatórios.");
                return RedirectToAction("Index");
            }

            try
            {
                var link = _mapper.Map<Link>(linkViewModel);

                if (linkViewModel.Image != null)
                {
                    link.FileName = await _imageStorage.AddImageAsync(linkViewModel.Image);
                }

                await _linkRepository.Add(link);
                _notyfService.Success("Link adicionado com sucesso.");
            }
            catch (UnknownImageFormatException)
            {
                _notyfService.Error("Imagem não suportada! Adicione imagens do tipo JPEG, PNG, BMP ou Webp.");
            }
            catch (Exception ex)
            {
                _notyfService.Error("Ocorreu um erro ao adicionar o link: " + ex.Message);
            }

            return RedirectToAction("Index");
        }
                
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var link = await _linkRepository.GetLinkAsync(id);
            if (link == null)
            {
                return Json(new { message = "Link não encontrado!" });
            }

            if (link.Category == null)
            {
                return Json(new { message = "Categoria do link não encontrada!" });
            }

            var pageId = link.Category.PageId;

            var pages = await _pageRepository.GetPagePerUserAsync(userId);
            var categories = await _categoryRepository.GetCategoriesPerPageAsync(pageId);

            var linkViewModel = _mapper.Map<LinkViewModel>(link);

            linkViewModel.Pages = pages;
            linkViewModel.PageId = pageId;
            linkViewModel.Categories = categories;

            return PartialView("_Edit", linkViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LinkViewModel linkViewModel)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Warning("Preencha todos os campos obrigatórios.");
                return RedirectToAction("Index");
            }

            try
            {
                var link = _mapper.Map<Link>(linkViewModel);

                if (linkViewModel.Image == null && linkViewModel.FileName != null)
                {
                    link.FileName = linkViewModel.FileName;
                } 
                else if (linkViewModel.Image != null && linkViewModel.FileName != null)
                {
                    _imageStorage.DeleteImage(linkViewModel.FileName);
                    link.FileName = await _imageStorage.AddImageAsync(linkViewModel.Image);
                }

                await _linkRepository.Update(link);
                _notyfService.Success("Link atualizado com sucesso.");
            }
            catch (UnknownImageFormatException)
            {
                _notyfService.Error("Imagem não suportada! Adicione imagens do tipo JPEG, PNG, BMP ou Webp.");
            }
            catch (Exception ex) 
            {
                _notyfService.Error("Ocorreu um erro ao adicionar o link: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var link = await _linkRepository.GetLinkAsync(id);
            if (link == null)
            {
                return Json(new { message = "Link não encontrado!" });
            }

            return PartialView("_Delete", link);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var link = await _linkRepository.GetLinkAsync(id);

                if (link == null)
                {
                    _notyfService.Error("link não encontrado!");
                    return RedirectToAction("Index");
                }

                _imageStorage.DeleteImage(link.FileName);

                await _linkRepository.DeleteAsync(link);
                _notyfService.Success("Link removido com sucesso.");
            }
            catch (Exception ex)
            {
                _notyfService.Error("Ocorreu um erro ao remover o link: " + ex.Message);
            }

            return RedirectToAction("Index");
        }
    }
}
