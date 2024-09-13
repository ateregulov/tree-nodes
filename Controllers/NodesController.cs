using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ReactTest.Tree.Site.Model;
using System.Xml.Linq;
using TreesNodes.DAL;
using TreesNodes.Helpers;

namespace TreesNodes.Controllers
{
    /// <summary>
    /// Represents tree node API
    /// </summary>
    [ApiController]
    [Tags("user.tree.node")]
    public class NodesController : ControllerBase
    {
        private readonly MyContext _context;
        private readonly IMapper _mapper;

        public NodesController(MyContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        /// <remarks>Create a new node in your tree. You must to specify a parent node ID that belongs to your tree. A new node name must be unique across all siblings.</remarks>
        [HttpPost("/api.user.tree.node.create")]
        public async Task<ActionResult> Create([FromQuery][BindRequired] string treeName, [BindRequired] int parentNodeId, [BindRequired] string nodeName)
        {
            var parent = await _context.Nodes.Where(x => x.Id == parentNodeId).FirstOrDefaultAsync();

            if (parent == null)
                throw new SecureException($"Parent node not found");

            var tree = await _context.Nodes.Where(x => x.ParentId == null & x.Name == treeName).FirstOrDefaultAsync();

            if (tree == null)
                throw new SecureException($"Tree not found");

            // check if tree contains parent node
            var contains = await CheckTreeContainsParent(tree, parent);
            if (!contains)
                throw new SecureException($"Parent node not found in the tree");

            // check node name is unique across all siblings
            var siblings = await _context.Nodes.Where(x => x.ParentId == parent.Id).ToListAsync();
            if (siblings.Any(x => x.Name == nodeName))
                throw new SecureException($"Node name must be unique across all siblings");

            var node = new Node { Name = nodeName, ParentId = parent.Id };
            await _context.Nodes.AddAsync(node);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<bool> CheckTreeContainsParent(Node tree, Node parent)
        {
            if (tree.Id == parent.Id)
                return true;

            var children = _context.Nodes.Where(n => n.ParentId == tree.Id).ToList();

            if (children.Any())
            {
                foreach (var child in children)
                {
                    if (child.Id == parent.Id)
                        return true;

                    await CheckTreeContainsParent(child, parent);
                }
            }

            return false;
        }

        /// <remarks>Delete an existing node in your tree. You must specify a node ID that belongs your tree.</remarks>
        [HttpPost("/api.user.tree.node.delete")]
        public async Task<ActionResult> Delete([FromQuery][BindRequired] string treeName, [BindRequired] int nodeId)
        {
            // check tree exists
            var tree = await _context.Nodes.Where(x => x.ParentId == null & x.Name == treeName).FirstOrDefaultAsync();

            if (tree == null)
                throw new SecureException($"Tree not found");

            // check node exists
            var node = await _context.Nodes.Where(x => x.Id == nodeId).FirstOrDefaultAsync();

            if ( node == null)
                throw new SecureException($"Node not found");

            // check node belongs to the tree
            var contains = await CheckTreeContainsParent(tree, node);
            if (!contains)
                throw new SecureException($"Requested node was found, but it doesn't belong your tree");

            // check node has no children
            var children = await _context.Nodes.Where(x => x.ParentId == node.Id).ToListAsync();
            if (children.Any())
                throw new SecureException($"You have to delete all children nodes first");

            _context.Nodes.Remove(node);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <remarks>Rename an existing node in your tree. You must specify a node ID that belongs your tree. A new name of the node must be unique across all siblings.</remarks>
        [HttpPost("/api.user.tree.node.rename")]
        public async Task<ActionResult> Rename([FromQuery][BindRequired] string treeName, [BindRequired] int nodeId, [BindRequired] string newNodeName)
        {
            // check tree exists
            var tree = await _context.Nodes.Where(x => x.ParentId == null & x.Name == treeName).FirstOrDefaultAsync();

            if (tree == null)
                throw new SecureException($"Tree not found");

            // check node exists
            var node = await _context.Nodes.Where(x => x.Id == nodeId).FirstOrDefaultAsync();

            if (node == null)
                throw new SecureException($"Node not found");

            // check node belongs to the tree
            var contains = await CheckTreeContainsParent(tree, node);
            if (!contains)
                throw new SecureException($"Requested node was found, but it doesn't belong your tree");

            if (newNodeName == node.Name)
                return Ok();

            // check node has no siblings with the same name
            var duplicateName = await _context.Nodes.Where(x => x.ParentId == node.ParentId && x.Name == newNodeName).AnyAsync();
            if (duplicateName)
                throw new SecureException($"Duplicate name");

            _context.Nodes.Remove(node);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
