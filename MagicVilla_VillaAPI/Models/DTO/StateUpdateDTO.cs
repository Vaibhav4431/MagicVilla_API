using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class StateUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int CountryId { get; set; }
        public string StateName { get; set; }
    }
}
