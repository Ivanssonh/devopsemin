using NSubstitute;
using SampleProject.Domain.Customers;
using SampleProject.Domain.Customers.Rules;
using SampleProject.Domain.SeedWork;
using SampleProject.UnitTests.SeedWork;
using Xunit;

namespace SampleProject.UnitTests.Customers
{
    
    public class CustomerRegistrationTests : TestBase
    {
        [Fact]
        public void GivenCustomerEmailIsUnique_WhenCustomerIsRegistering_IsSuccessful()
        {
            // Arrange
            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            const string email = "testEmail@email.com";
            customerUniquenessChecker.IsUnique(email).Returns(true);

            // Act
            var customer = Customer.CreateRegistered(email, "Sample name", customerUniquenessChecker);

            // Assert
            AssertPublishedDomainEvent<CustomerRegisteredEvent>(customer);
        }

        [Fact]
        public void GivenCustomerEmailIsNotUnique_WhenCustomerIsRegistering_BreaksCustomerEmailMustBeUniqueRule()
        {
            // Arrange
            var customerUniquenessChecker = Substitute.For<ICustomerUniquenessChecker>();
            const string email = "testEmail@email.com";
            customerUniquenessChecker.IsUnique(email).Returns(false);

            // Assert
            AssertBrokenRule<CustomerEmailMustBeUniqueRule>(() =>
            {
                // Act
                Customer.CreateRegistered(email, "Sample name", customerUniquenessChecker);
            });
        }


        [Fact]
        public void GivenCustomerEmailHasInvalidDomain_WhenCustomerIsRegistering_BreaksCustomerEmailMustHaveNackademinDomain()
        {

            // Arrange
            var email = "testEmail@test.se";

            // Act & Assert
            AssertBrokenRule<CustomerEmailMustHaveValidDomainRule>(() =>
            {
                Customer.CreateRegistered(email, "Sample Name");
            });
        }

    } 

}