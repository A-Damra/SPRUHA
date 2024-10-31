using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SPRUHA.Models;
using System.IO;



namespace SPRUHA.Controllers
{
    public class UserModelsController : Controller
    {
        private readonly myDbContext dbContext;

        public UserModelsController(myDbContext context)
        {
            dbContext = context;
        }

        //log in page
        public IActionResult index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UploadMillion()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UploadMillion(IFormFile file)
        {
            Random rand = new Random();

            // Check if the file is null or empty
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("empty File");
                return View();
            }

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(); // Get the first worksheet

                    // Check if any images are present in the worksheet
                    if (worksheet.Drawings.Count > 0)
                    {
                        Console.WriteLine("The file contains images and cannot be processed.");
                        return View();
                    }

                    var rowCount = worksheet.Dimension.Rows;



                    for (int row = 2; row <= rowCount; row++)
                    {
                        var dummydata = new DummyData
                        {
                            NameId = Convert.ToInt32(worksheet.Cells[row, 1].Text),
                            Name = worksheet.Cells[row, 2].Text,
                            Email = worksheet.Cells[row, 3].Text,
                            MobileNo = worksheet.Cells[row, 4].Text,
                            Password = rand.Next(100, 1000)
                        };

                        dbContext.DummyData.Add(dummydata);
                    }

                    dbContext.SaveChanges();
                }
            }

            return RedirectToAction("UploadMillion");
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User userModel, IFormFile photo)
        {
            User user = new User();

            user.Name = userModel.Name;
            user.Mobile = userModel.Mobile;
            user.Password = userModel.Password;
            user.Email = userModel.Email;


            if (photo != null && photo.Length > 0)
            {
                // Generate a unique filename to avoid conflicts
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                // Save the file to the wwwroot/images directory
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                // Store the path in the database
                user.Photo = $"/images/{fileName}";
            }

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            return RedirectToAction("GetAllUsers");
        }


        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = dbContext.Users.ToList();
            return View(users);
        }
    }
}
