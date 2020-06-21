using System;

namespace Customer.Contract
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string CityCode { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
