using informedph.backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using informedph.backend.Models;
using informedph.backend.Dtos;
using Microsoft.EntityFrameworkCore;

namespace informedph.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<NewsArticle>>> GetAll(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var totalItems = await _context.NewsArticles.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await _context.NewsArticles
                .OrderByDescending(n => n.PublishedAt)
                .ThenByDescending(n => n.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PagedResponse<NewsArticle>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NewsArticle>> GetById(int id)
        {
            var article = await _context.NewsArticles.FindAsync(id);
            if (article is null) return NotFound();

            return Ok(article);
        }
    }
}
