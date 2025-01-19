using System;
using System.Collections.Generic;

namespace SCPSLPluginBrowser.Data
{
    public class DllFile
    {
        public int Id { get; set; }               
        public string FileName { get; set; }      
        public string Description { get; set; }   
        public string Icon { get; set; }          
        public byte[] FileData { get; set; }      
        public DateTime CreatedAt { get; set; }   
        public string UserId { get; set; }        
        public ApplicationUser User { get; set; } 

        public ICollection<Like> Likes { get; set; }      
        public ICollection<Comment> Comments { get; set; } 
        public ICollection<Flag> Flags { get; set; }       // Flags on this file
    }
}
