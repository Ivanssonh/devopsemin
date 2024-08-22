using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Customers;
using SampleProject.Application.Customers.RegisterCustomer;

namespace SampleProject.API.Customers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        /// <summary>
        /// Register customer.
        /// </summary>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> RegisterCustomer([FromBody]RegisterCustomerRequest request)
        {
           var customer = await _mediator.Send(new RegisterCustomerCommand(request.Email, request.Name));

           return Created(string.Empty, customer);
        }

        [Route("")]
        [HttpGet]
        
        public async Task<IActionResult> GetCustomerDto()
        {
            var customerDto = await Task.Run(() => new CustomerDto { Id = Guid.NewGuid() });
            return Ok(customerDto);
        }
    }
}
