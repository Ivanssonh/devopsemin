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
        private readonly IEmailDomainChecker _emailDomainChecker;
        public CustomerEmailMustHaveValidDomainRule(IEmailDomainChecker emailDomainChecker,string email)
        {
            _email = email;
            _emailDomainChecker = emailDomainChecker;
        }

        public bool IsBroken() => !_emailDomainChecker.IsValidDomain(_email);

        public string Message => "You have to register an emailadress with domain 'nackademin.se";

        
    }
}
