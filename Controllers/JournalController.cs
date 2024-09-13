using AutoMapper;
using FxNet.Web.Def.Api.Diagnostic.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ReactTest.Tree.Site.Model;
using TreesNodes.DAL;
using TreesNodes.Helpers;
using TreesNodes.ViewModels;

namespace TreesNodes.Controllers
{
    /// <summary>
    /// Represents entire tree API
    /// </summary>
    [ApiController]
    [Tags("user.journal")]
    [CustomExceptionFilter]
    public class JournalController : ControllerBase
    {
        private readonly MyContext _context;
        private readonly IMapper _mapper;

        public JournalController(MyContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        /// <remarks>Provides the pagination API. Skip means the number of items should be skipped by server. Take means the maximum number items should be returned by server. All fields of the filter are optional.</remarks>
        [HttpPost("/api.user.journal.getRange")]
        public async Task<ActionResult<MJournalData>> GetRange([BindRequired][FromQuery]int skip, [BindRequired][FromQuery]int take, [BindRequired][FromBody] VJournalFilter filter)
        {
            if (take > 50)
                take = 50;

            IQueryable<JournalItem> query = _context.Journal;

            if (filter.From != null)
                query = query.Where(x => x.CreatedAt >= filter.From);

            if (filter.To != null)
                query = query.Where(x => x.CreatedAt <= filter.To);

            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(x => x.Message.Contains(filter.Search));

            query = query.Skip(skip).Take(take);

            var items = await query.ToListAsync();

            var result = new MJournalData {
                Skip = skip,
                Count = items.Count,
                Items = items.Select(i => new MJournalInfo
                {
                    CreatedAt = i.CreatedAt,
                    EventId = i.Id,
                    Id = i.Id
                }).ToList()
            };

            return Ok(result);
        }

        /// <remarks>Returns the information about an particular event by ID.</remarks>
        [HttpPost("/api.user.journal.getSingle")]
        public async Task<ActionResult<MJournalData>> GetSingle([BindRequired][FromQuery] int id)
        {
            var item = _context.Journal.FirstOrDefault(x => x.Id == id);

            if (item == null)
                return NotFound();

            var result = new MJournal
            {
                CreatedAt = item.CreatedAt,
                Id = item.Id,
                EventId = item.Id,
                Text = item.Message
            };

            return Ok(result);
        }

    }
}
