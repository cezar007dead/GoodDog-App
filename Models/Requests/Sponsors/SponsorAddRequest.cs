using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain
{
    public class SponsorAddRequest
    {

        [Required]
        [StringLength(maximumLength: 128, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [Url]
        [StringLength(maximumLength: 200)]
        public string CompanyUrl { get; set; }

        public int AddressId { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(maximumLength: 128, MinimumLength = 2)]
        public string ContactPerson { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PrimarySponsorTypeId { get; set; }

    }
}
