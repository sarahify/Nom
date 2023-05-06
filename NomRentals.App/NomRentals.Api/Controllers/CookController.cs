using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NomRentals.Api.Data;
using NomRentals.Api.Entities;

namespace NomRentals.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CookController : ControllerBase
    {
        private readonly CustomerApiDbContext dbContext;

        public CookController(CustomerApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetCook()
        {
            return Ok(await dbContext.Cooks.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetCook(Guid id)
        {

            var cook = await dbContext.Cooks.FindAsync(id);
            if (cook == null)
            {
                return NotFound();
            }
            return Ok(cook);

        }

        [HttpGet]
        [Route("pagination")]
        public async Task<IActionResult> GetAllCook(int pageNumber)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var defaultPageSize = 5f;
            var allCook = await dbContext.Cooks.ToListAsync();
            var totalItems = allCook.Count();
            var pagecount = Math.Ceiling(totalItems / defaultPageSize);
            var item = allCook.Skip((pageNumber - 1) * (int)defaultPageSize)
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
        public async Task<IActionResult> AddCook(AddUserRequest addUserRequest)
        {
            var cooks = new Cook()
            {
                Id = Guid.NewGuid(),
                FirstName = addUserRequest.FirstName,
                LastName = addUserRequest.LastName,
                MiddleName= addUserRequest.MiddleName,
                Email = addUserRequest.Email,
                PhoneNumber = addUserRequest.PhoneNumber,
                Address = addUserRequest.Address,

            };
            await dbContext.Cooks.AddAsync(cooks);
            await dbContext.SaveChangesAsync();

            return Ok(cooks);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateCook(Guid id, UpdateUserRequest updateUserRequest)
        {
            var cook = await dbContext.Cooks.FindAsync(id);
            
            if (cook != null)
            {
                cook.FirstName = updateUserRequest.FirstName;
                cook.LastName = updateUserRequest.LastName;
                cook.MiddleName = updateUserRequest.MiddleName;
                cook.PhoneNumber = updateUserRequest.PhoneNumber;
                cook.Email = updateUserRequest.Email;
                cook.Address = updateUserRequest.Address;

                await dbContext.SaveChangesAsync();
                return Ok(cook);
            }

            return NotFound();
        }
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCook(Guid id)
        {
            var cook = await dbContext.Cooks.FindAsync(id);
            if (cook != null)
            {
                dbContext.Remove(cook);
                await dbContext.SaveChangesAsync();
                return Ok(cook);

            }
            return NotFound();
        }
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> SearchCook(string? email, string? firstname)
        {
            var cook = await dbContext.Cooks.FindAsync(email, firstname);
            return NotFound();
        }


        [HttpPatch("Resume")]
        public async Task<IActionResult> UploadPhoto2(IFormFile file, Guid id)
        {
            var findCook = await dbContext.Cooks.FindAsync(id);
            if (findCook == null)
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
