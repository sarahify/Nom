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
    public class EmployeeController : ControllerBase
    {
        private readonly CustomerApiDbContext dbContext;

        public EmployeeController(CustomerApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployee()
        {
            return Ok(await dbContext.Employees.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetEmployee(Guid id)
        {

            var cook = await dbContext.Employees.FindAsync(id);
            if (cook == null)
            {
                return NotFound();
            }
            return Ok(cook);

        }

        [HttpGet]
        [Route("pagination")]
        public async Task<IActionResult> GetAllEmployee(int pageNumber)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var defaultPageSize = 5f;
            var allCook = await dbContext.Employees.ToListAsync();
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
        public async Task<IActionResult> AddEmployee(AddUserRequest addUserRequest)
        {
            var employee = new Employee()
            {
                EmployeeId = Guid.NewGuid(),
                FirstName = addUserRequest.FirstName,
                LastName = addUserRequest.LastName,
                EmailAddress = addUserRequest.Email,
                PhoneNumber = addUserRequest.PhoneNumber,
                HomeAddress = addUserRequest.HomeAddress,



            };
            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();

            return Ok(employee);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, UpdateUserRequest updateUserRequest)
        {
            var employee = await dbContext.Employees.FindAsync(id);

            if (employee != null)
            {
                employee.FirstName = updateUserRequest.FirstName;
                employee.LastName = updateUserRequest.LastName;
                employee.PhoneNumber = updateUserRequest.PhoneNumber;
                employee.EmailAddress = updateUserRequest.Email;
                employee.HomeAddress= updateUserRequest.HomeAddress;
                



                await dbContext.SaveChangesAsync();
                return Ok(employee);
            }

            return NotFound();
        }
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var employee = await dbContext.Employees.FindAsync(id);
            if (employee != null)
            {
                dbContext.Remove(employee);
                await dbContext.SaveChangesAsync();
                return Ok(employee);

            }
            return NotFound();
        }
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> SearchEmployee(string? email, string? firstname)
        {
            var cook = await dbContext.Employees.FindAsync(email, firstname);
            return NotFound();
        }


        [HttpPatch("Resume")]
        public async Task<IActionResult> UploadPhoto2(IFormFile file, Guid id)
        {
            var findCook = await dbContext.Employees.FindAsync(id);
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
