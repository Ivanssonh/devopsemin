using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Domain.Customers
{
    public class NackademinEmailDomainChecker : IEmailDomainChecker
    {
        public bool IsValidDomain(string email)
        {
            return email.EndsWith("nackademin.se");
        }
    }
}
