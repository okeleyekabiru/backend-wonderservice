using System;


namespace backend_wonderservice.DATA.Models
{
    public partial class Photo
    {
        public Guid Id { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }
        public DateTime? TimeUpload { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public Guid? ServicesId { get; set; }
        public string UserId { get; set; }

        public virtual Services Services { get; set; }
        public virtual User User { get; set; }
    }
}
