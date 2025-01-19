namespace SCPSLPluginBrowser.Data
{
    public class Like
    {
        public int Id { get; set; }           
        public string UserId { get; set; }    
        public int DllFileId { get; set; }   
        public ApplicationUser User { get; set; }  
        public DllFile DllFile { get; set; } 
    }
}
