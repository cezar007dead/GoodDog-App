using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain
{
    public class Sponsor
    {
        public int Id { get; set; }

        public User User { get; set; }

        public string Name { get; set; }

        public Uri CompanyUrl { get; set; }

        public Address Address { get; set; }

        public string PhoneNumber { get; set; }

        public string ContactPerson { get; set; }

        public SponsorType SponsorType { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
