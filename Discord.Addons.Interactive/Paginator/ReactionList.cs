namespace Discord.Addons.Interactive.Paginator
{
    public class ReactionList
    {
        public bool First { get; set; } = true;
        public bool Last { get; set; } = true;
        public bool Forward { get; set; } = true;
        public bool Backward { get; set; } = true;
        public bool Jump { get; set; } = false;
        public bool Trash { get; set; } = false;
        public bool Info { get; set; } = false;
    }
}