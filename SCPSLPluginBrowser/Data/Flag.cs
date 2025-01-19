using System;

namespace SCPSLPluginBrowser.Data
{
    public class Flag
    {
        public int Id { get; set; }              
        public string UserId { get; set; }       
        public int DllFileId { get; set; }      
        public string Reason { get; set; }       
        public bool IsReviewed { get; set; }     
        public DateTime CreatedAt { get; set; }  

        public ApplicationUser User { get; set; }   
        public DllFile DllFile { get; set; }       
    }

}
