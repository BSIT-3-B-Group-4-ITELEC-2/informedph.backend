using informedph.backend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using informedph.backend.Models;
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
        public async Task<ActionResult<List<NewsArticle>>> GetAll(int page = 1, int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var items = await _context.NewsArticles
                .OrderByDescending(n => n.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(items);
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
