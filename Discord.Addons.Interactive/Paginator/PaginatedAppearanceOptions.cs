using System;

namespace Discord.Addons.Interactive.Paginator
{
    public class PaginatedAppearanceOptions
    {
        public IEmote First { get; set; } = new Emoji("⏮");
        
        public IEmote Back { get; set; } = new Emoji("◀");
        
        public IEmote Next { get; set; } = new Emoji("▶");
        
        public IEmote Last { get; set; } = new Emoji("⏭");
        
        public IEmote Stop { get; set; } = new Emoji("⏹");
        
        public IEmote Jump { get; set; } = new Emoji("🔢");
        
        public IEmote Info { get; set; } = new Emoji("ℹ");

        public string FooterFormat { get; set; } = "Page {0}/{1}";

        public string FooterFormat = "Page {0}/{1}";
        public string InformationText = "This is a paginator. React with the respective icons to change page.";

        public JumpDisplayOptions JumpDisplayOptions = JumpDisplayOptions.WithManageMessages;
        public bool DisplayInformationIcon = true;

        public TimeSpan? Timeout = null;
        public TimeSpan InfoTimeout = TimeSpan.FromSeconds(30);

        public int FieldsPerPage = 6;
    }

    public enum JumpDisplayOptions
    {
        Never,
        WithManageMessages,
        Always
    }
}