using SampleProject.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Domain.Customers.Rules
{
    public class CustomerEmailMustHaveValidDomainRule : IBusinessRule
    {
        private readonly string _email;
        public CustomerEmailMustHaveValidDomainRule(string email)
        {
            _email = email;
        }

        public bool IsBroken() => !_email.EndsWith("nackademin.se");

        public string Message => "You have to register an emailadress with domain 'nackademin.se";

        
    }
}
