namespace TreesNodes.Helpers
{
    public class ExceptionMessage
    {
        public string Type { get; set; }
        public long Id { get; set; }
        public Data Data { get; set; }
    }
    public class Data
    {
        public string Message { get; set; }
    }
}
