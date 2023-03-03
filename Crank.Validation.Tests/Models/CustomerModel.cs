namespace Crank.Validation.Tests.Models
{

    public class CustomerModel
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public AddressModel Address { get; set; }
    }

}
