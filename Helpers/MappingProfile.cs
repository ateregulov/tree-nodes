using AutoMapper;
using ReactTest.Tree.Site.Model;
using TreesNodes.DAL;

namespace TreesNodes.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Node, MNode>();
        }
    }
}
