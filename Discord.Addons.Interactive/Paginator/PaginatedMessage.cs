using System;
using System.Collections.Generic;

namespace Discord.Addons.Interactive.Paginator
{
    public class PaginatedMessage
    {
        public IEnumerable<Page> Pages { get; set; } = new List<Page>();

        public string Content { get; set; } = string.Empty;
        
        public EmbedAuthorBuilder Author { get; set; } = null;

        public Color Color { get; set; } = Color.Default;
        public string Title { get; set; } = null;

        public string Url { get; set; } = null;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = null;

        public string ThumbnailUrl { get; set; } = null; 
        
        public List<EmbedFieldBuilder> Fields { get; set; } = new List<EmbedFieldBuilder>();
        
        public EmbedFooterBuilder FooterOverride { get; set; } = null;
        
        public DateTimeOffset? TimeStamp { get; set; } = null;
        public PaginatedAppearanceOptions Options { get; set; } = PaginatedAppearanceOptions.Default;
    }
    public abstract class Page { }
    public class MessagePage : Page
    {
        public string Content { get; set; }
    }
    public class EmbedPage : Page
    {
        public string AlternateAuthorTitle { get; set; }
        
        public string AlteranteAuthorIcon { get; set; }
        
        public bool DisplayTotalFieldsCount { get; set; } = false;
        
        public string TotalFieldsMessage { get; set; } = null;
        
        public double TotalFieldsCountConstant { get; set; } = 1;
        
        public string Title { get; set; }
        
        public string Url { get; set; } = null;
        
        public string Description { get; set; }
        
        public string ImageUrl { get; set; }
        
        public string ThumbnailUrl { get; set; } = null;
        
        public List<EmbedFieldBuilder> Fields { get; set; } = new List<EmbedFieldBuilder>();
        
        public EmbedFooterBuilder FooterOverride { get; set; } = null;
        
        public DateTimeOffset? TimeStamp { get; set; } = null;
        
        public Color? Color { get; set; } = null;
    }
}