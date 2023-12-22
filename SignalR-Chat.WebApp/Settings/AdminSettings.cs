using System.ComponentModel.DataAnnotations;

namespace NotReksaChat.Settings
{
    public class AdminSettings
    {
        public const string AdminSettingsSection = "AdminSettings";

        [Required]
        public string BanUserPasswordHash { get; set; }

        public AdminSettings()
        {
            
        }
    }
}