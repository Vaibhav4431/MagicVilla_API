using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class CountryUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        public string CountryName { get; set; }

    }
}
