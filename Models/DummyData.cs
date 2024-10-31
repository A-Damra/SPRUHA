using System.ComponentModel.DataAnnotations;

namespace SPRUHA.Models
{
    public class DummyData
    {
        [Key]
        public int NameId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public int Password {  get; set; }
    }
}
