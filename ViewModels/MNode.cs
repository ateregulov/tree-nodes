namespace ReactTest.Tree.Site.Model
{
    public class MNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MNode> Children { get; set; }
    }
}
