using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using TaskOneDraft.Areas.Identity.Data;
using TaskOneDraft.Models;

namespace TaskOneDraft.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClaimsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this._webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Claims()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Claims(Claims claims)
        {
            if (ModelState.IsValid)
            {
                if (claims.SupportingDocuments != null && claims.SupportingDocuments.Any())
                    {
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    foreach (var file in claims.SupportingDocuments)
                    {
                        if(file.Length > 0)
                        {
                            string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                            string filePath = Path.Combine(uploadPath, fileName);   
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                        }
                    }
                }




                //calculate total hours 
                claims.TotalAmount = claims.HoursWorked * claims.RatePerHour;
                _context.Add(claims);
                await _context.SaveChangesAsync();
                ViewBag.Message = "Your claim has been successfully submitted.";
                return View(claims);
            }
            return View(claims);
        }

        public async Task<IActionResult> List()
        {
            var claims = await _context.Claims.ToListAsync();
            return View(claims);//Ensure this the name of the view file 
        }

        
    }
}
