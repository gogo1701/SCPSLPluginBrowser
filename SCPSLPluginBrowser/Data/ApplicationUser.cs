using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SCPSLPluginBrowser.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<DllFile> DllFiles { get; set; }   // User's uploaded files
        public ICollection<Like> Likes { get; set; }         // User's likes on files
        public ICollection<Comment> Comments { get; set; }   // User's comments on files
        public ICollection<Flag> Flags { get; set; }         // User's flags on files
    }

}
