using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NomRentals.Api.Data;
using NomRentals.Api.Entities;
using System.Security.Principal;

namespace NomRentals.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerApiDbContext dbContext;

        public CustomerController(CustomerApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try 
            {
                return Ok(await dbContext.Customers.ToListAsync());
            }
           catch(Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            try 
            {
                var customer = await dbContext.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
                
       
        }

        [HttpGet]
        [Route("pagination")]
        public async Task<IActionResult> GetAllCustomer(int pageNumber) 
        {
            try 
            {
                if (pageNumber < 1)
                {
                    pageNumber = 1;
                }
                var defaultPageSize = 5f;
                var allCustomer = await dbContext.Customers.ToListAsync();
                var totalItems = allCustomer.Count();
                var pagecount = Math.Ceiling(totalItems / defaultPageSize);
                var item = allCustomer.Skip((pageNumber - 1) * (int)defaultPageSize)
                    .Take((int)defaultPageSize).ToList();
                var result = new
                {
                    totalItems = totalItems,
                    Data = item,
                    CurrentPage = pageNumber,
                    PageSize = pagecount
                };
                return Ok(result);
            }
            catch(Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(AddUserRequest addUserRequest) 
        {
            try 
            {
                var customer = new Customer()
                {
                    Id = Guid.NewGuid(),
                    FirstName = addUserRequest.FirstName,
                    LastName = addUserRequest.LastName,
                    Email = addUserRequest.Email,
                    PhoneNumber = addUserRequest.PhoneNumber,
                    Address = addUserRequest.Address,

                };
                await dbContext.Customers.AddAsync(customer);
                await dbContext.SaveChangesAsync();

                return Ok(customer);
            }
            catch(Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
          
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, UpdateUserRequest updateUserRequest)
        {
            try 
            {
                var customer = await dbContext.Customers.FindAsync(id);

                if (customer != null)
                {
                    customer.FirstName = updateUserRequest.FirstName;
                    customer.LastName = updateUserRequest.LastName;
                    customer.PhoneNumber = updateUserRequest.PhoneNumber;
                    customer.Email = updateUserRequest.Email;
                    customer.Address = updateUserRequest.Address;

                    await dbContext.SaveChangesAsync();
                    return Ok(customer);
                }

                return NotFound();
            }
            catch(Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);

            }
          
        }
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try 
            {
                var customer = await dbContext.Customers.FindAsync(id);
                if (customer != null)
                {
                    dbContext.Remove(customer);
                    await dbContext.SaveChangesAsync();
                    return Ok(customer);

                }
                return NotFound();
            }
            catch(Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> SearchCustomer(string? email, string? username) 
        {
            try 
            {
                var customer = await dbContext.Customers.FindAsync(email, username);
                
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return NotFound();
            
            
        }


        [HttpPatch("Photo")]
        public async Task<IActionResult> UploadPhoto2(IFormFile file, Guid id)
        {
            var findCustomer = await dbContext.Customers.FindAsync(id);
            if (findCustomer == null)
            {
                return NotFound("Customer to upload picture to not available");
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    Status = "No Image Uploaded"
                });
            }
            var cloudinary = new Cloudinary(new Account("do9gjpyic", "166866251324575", "aNFlYziiNePXWkRE45Fi2dIVqOU"));
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"{id}"
            };
            var result = cloudinary.Upload(uploadParams);
            if (result == null)
            {
                return BadRequest(new
                {
                    Status = "Image not upload successfully"
                });
            }

            return Ok(new
            {
                PublicId = result.PublicId,
                Url = result.SecureUrl.ToString(),
                Status = "Uploaded Succefully"
            });
        }
    }

    
}
