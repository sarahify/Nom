using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NomRentals.Api.Data;
using NomRentals.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace NomRentals.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesBoyController : ControllerBase
    {
        private readonly CustomerApiDbContext dbContext;

        public ServicesBoyController(CustomerApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetServicesBoy()
        {
            return Ok(await dbContext.ServiceBoys.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetServicesBoy(Guid id)
        {

            var serviceBoy = await dbContext.ServiceBoys.FindAsync(id);
            if (serviceBoy == null)
            {
                return NotFound();
            }
            return Ok(serviceBoy);

        }

        [HttpGet]
        [Route("pagination")]
        public async Task<IActionResult> GetAllServicesBoy(int pageNumber)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var defaultPageSize = 5f;
            var allServiceBoy = await dbContext.ServiceBoys.ToListAsync();
            var totalItems = allServiceBoy.Count();
            var pagecount = Math.Ceiling(totalItems / defaultPageSize);
            var item = allServiceBoy.Skip((pageNumber - 1) * (int)defaultPageSize)
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

        [HttpPost]
        public async Task<IActionResult> AddServiceBoy(AddUserRequest addUserRequest)
        {
            var ServiceBoy = new ServiceBoy()
            {
                Id = Guid.NewGuid(),
                FirstName = addUserRequest.FirstName,
                LastName = addUserRequest.LastName,
                Email = addUserRequest.Email,
                PhoneNumber = addUserRequest.PhoneNumber,
                Address = addUserRequest.Address,

            };
            
            
            await dbContext.ServiceBoys.AddAsync(ServiceBoy);
            await dbContext.SaveChangesAsync();

            return Ok(ServiceBoy);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateServiceBoy(Guid id, UpdateUserRequest updateUserRequest)
        {
            var serviceBoy = await dbContext.ServiceBoys.FindAsync(id);

            if (serviceBoy != null)
            {
                serviceBoy.FirstName = updateUserRequest.FirstName;
                serviceBoy.LastName = updateUserRequest.LastName;
                serviceBoy.PhoneNumber = updateUserRequest.PhoneNumber;
                serviceBoy.Email = updateUserRequest.Email;
                serviceBoy.Address = updateUserRequest.Address;

                await dbContext.SaveChangesAsync();
                return Ok(serviceBoy);
            }

            return NotFound();
        }
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteServiceBoy(Guid id)
        {
            var serviceBoy = await dbContext.ServiceBoys.FindAsync(id);
            if (serviceBoy != null)
            {
                dbContext.Remove(serviceBoy);
                await dbContext.SaveChangesAsync();
                return Ok(serviceBoy);

            }
            return NotFound();
        }
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> SearchServiceBoy(string? email, string? phonenumber)
        {
            var serviceBoy = await dbContext.ServiceBoys.FindAsync(email, phonenumber);
            return NotFound();
        }


        [HttpPatch("Resume")]
        public async Task<IActionResult> UploadPhoto2(IFormFile file, Guid id)
        {
            var findServiceBoy = await dbContext.ServiceBoys.FindAsync(id);
            if (findServiceBoy == null)
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
