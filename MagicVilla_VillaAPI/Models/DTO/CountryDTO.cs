using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class CountryDTO
    {
        [Required]
        public int Id { get; set; }
        public string CountryName { get; set; }

    }
}
