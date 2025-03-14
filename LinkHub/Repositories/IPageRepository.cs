﻿using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface IPageRepository
    {
        List<Page> GetPages();
        Task<Page?> GetPageAsync(int id);
        Task<Page?> GetPagePerNameAsync(string name);
        Task<List<Page>> GetPagePerUserAsync(string userId);
        Task<Page> AddAsync(Page page);
        Task<Page> Update(Page page);
        Task<bool> Delete(int id);
    }
}
