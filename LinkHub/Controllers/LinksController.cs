﻿using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.Services;
using LinkHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkHub.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class LinksController : Controller
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IPageRepository _pageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHomePageRepository _homePageRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotyfService _notyfService;
        private readonly ImageStorage _imageStorage;
        private readonly IMapper _mapper;

        public LinksController(ILinkRepository linkRepository,
            ICategoryRepository categoryRepository,
            IPageRepository pageRepository,
            IHomePageRepository homePageRepository,
            UserManager<ApplicationUser> userManager,
            INotyfService notyfService,
            ImageStorage imageStorage,
            IMapper mapper)
        {
            _linkRepository = linkRepository;
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
            _homePageRepository = homePageRepository;
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
            catch (Exception ex)
            {
                _notyfService.Error("Ocorreu um erro ao adicionar o link: " + ex.Message);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Home(int id)
        {
            var link = await _linkRepository.GetLinkAsync(id);
            if (link == null)
            {
                return Json(new { message = "Página não encontrada!" });
            }

            var selectedPageIds = await _homePageRepository.GetPagesPerLinkAsync(id);

            var pages = _pageRepository.GetPages();

            var linkHomeView = new LinkHomeViewModel
            {
                LinkId = id,
                LinkName = link.Name,
                Pages = pages,
                SelectedPageIds = selectedPageIds
            };

            return PartialView("_Home", linkHomeView);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Home(LinkHomeViewModel linkHome)
        {
            try
            {
                var linkId = linkHome.LinkId;
                var pageIds = linkHome.SelectedPageIds;

                if (pageIds != null)
                {
                    await _homePageRepository.Update(linkId, pageIds);
                }
                else
                {
                    await _homePageRepository.DeleteAllHomePage(linkId);
                }

                _notyfService.Success("Home Page atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                _notyfService.Error("Não foi possivel atualizar a Home Page: " + ex.Message);
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
