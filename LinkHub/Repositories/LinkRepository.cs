using LinkHub.Data;
using LinkHub.Models;
using LinkHub.Services;

namespace LinkHub.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageStorage _imageStorage;

        public LinkRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ImageStorage imageStorage)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _imageStorage = imageStorage;
        }

        public Link GetLink(int id)
        {
            return _context.Links.FirstOrDefault(x => x.Id == id);
        }

        public List<Link> GetLinks()
        {
            return _context.Links.ToList();
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public async Task<Link> Add(Link link)
        {
            if (link.Image != null)
            {
                link.FileName = await _imageStorage.AddImageAsync(link.Image);
            }

            _context.Links.Add(link);
            await _context.SaveChangesAsync();

            return link;
        }

        public async Task<Link> Update(Link link)
        {
            Link linkDB = GetLink(link.Id);

            if (linkDB == null) throw new Exception("Houve um erro na atualização do Serviço!");

            if (link.Image != null)
            {
                _imageStorage.DeleteImage(link.FileName);
                link.FileName = await _imageStorage.AddImageAsync(link.Image);
            }

            linkDB.Name = link.Name;
            linkDB.Description = link.Description;
            linkDB.CategoryId = link.CategoryId;
            linkDB.Url = link.Url;
            linkDB.FileName = link.FileName;

            _context.Links.Update(linkDB);
            await _context.SaveChangesAsync();

            return linkDB;
        }

        public bool Delete(int id)
        {
            Link linkDB = GetLink(id);

            if (linkDB == null) throw new Exception("Houve um erro na deleção do Serviço!");

            _imageStorage.DeleteImage(linkDB.FileName);

            _context.Links.Remove(linkDB);
            _context.SaveChanges();

            return true;
        }
    }
}