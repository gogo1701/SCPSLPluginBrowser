using System;

namespace SCPSLPluginBrowser.Data
{ 
    public class Comment
    {
        public int Id { get; set; }              
        public string UserId { get; set; }       
        public int DllFileId { get; set; }      
        public string CommentText { get; set; }  
        public DateTime CreatedAt { get; set; }  
        public ApplicationUser User { get; set; }  
        public DllFile DllFile { get; set; }       
    }

}
