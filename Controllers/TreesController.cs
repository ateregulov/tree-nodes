using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ReactTest.Tree.Site.Model;
using TreesNodes.DAL;

namespace TreesNodes.Controllers
{
    /// <summary>
    /// Represents entire tree API
    /// </summary>
    [ApiController]
    [Tags("user.tree")]
    public class TreesController : ControllerBase
    {
        private readonly MyContext _context;
        private readonly IMapper _mapper;

        public TreesController(MyContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        /// <remarks>Returns your entire tree. If your tree doesn't exist it will be created automatically.</remarks>
        [HttpPost("/api.user.tree.get")]
        public async Task<ActionResult<MNode>> GetOrCreate([FromQuery][BindRequired] string treeName)
        {
            var tree = _context.Nodes.Where(n => n.ParentId == null & n.Name == treeName).FirstOrDefault();

            MNode result;

            if (tree == null)
            {
                tree = new Node { Name = treeName, Id = 0, ParentId = null };
                await _context.Nodes.AddAsync(tree);
                await _context.SaveChangesAsync();
            }
            else
            {
                await GetChildren(tree);
            }

            result = _mapper.Map<Node, MNode>(tree);

            return Ok(result);
        }

        private async Task GetChildren(Node node)
        {
            var children = _context.Nodes.Where(n => n.ParentId == node.Id).ToList();

            if (children.Any())
            {
                foreach (var child in children)
                {
                    await GetChildren(child);
                }
            }

            node.Children = children;
        }
    }
}
