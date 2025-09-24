using AspNetCoreGeneratedDocument;
using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Extensions;
using car_kaashiv_web_app.Models.DTOs;
using car_kaashiv_web_app.Models.Entities;
using car_kaashiv_web_app.Models.Enums;
using car_kaashiv_web_app.Services;
using car_kaashiv_web_app.Services.Extensions;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace car_kaashiv_web_app.Controllers
{
    public class PartController : Controller
    {
        private readonly string? _connectionString;
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeeController> _logger;
      
        public PartController(IConfiguration configuration, ILogger<EmployeeController> logger, AppDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _logger = logger;
        }

        // GET: PartController
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PartAdd()
        {
            //Fetch EmployeeId from claims (cookie)
            var empId = int.Parse(User.FindFirst("EmployeeId")!.Value);          
           
            var model = new PartsAddDto //loads the form without triggering validation
            {
                PEmpId = empId,//Override the dto.PEmpId to auto populate empId on form table entity
            };
            return View(model);
        }
        public async Task <ActionResult> PartList()
        {
            var parts = await _context.tbl_part.ToListAsync();// loads data form db
            return View("ViewParts", parts); //Model is now a list, not null
        }

        // GET: PartController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PartController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PartController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPart(PartsAddDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData.setAlert("modal state false", AlertTypes.Error);
                return View("PartAdd", model);
            }

            string? fileName = null;

            if (model.ImagePath != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/parts");
                Directory.CreateDirectory(uploadsFolder); // make sure folder exists

                // get original filename safely
                fileName = Path.GetFileName(model.ImagePath.FileName);
                var filePath = Path.Combine(uploadsFolder, model.ImagePath.FileName);

                // Prevent overwrite
                if (System.IO.File.Exists(filePath))
                {

                    ModelState.AddModelError("ImagePath", "A file with this name already exists. Please rename your file and try again.");
                    TempData.setAlert("File Already Exists!", AlertTypes.Error);
                    return View("PartAdd", model);
                }

                // save the uploaded file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImagePath.CopyToAsync(stream);
                    TempData.setAlert("Part image uploaded successfully", AlertTypes.Success);

                }
            }                      

            // map DTO -> entity(C# table)
            var part = new TablePart
            {   
                PEmpId    = model.PEmpId,
                PName     = model.PName,
                PDetail   = model.PDetail,
                PPrice    = model.PPrice,
                PStock    = model.PStock,
                ImagePath = "/images/parts/" + fileName, // keep original filename
                CreatedAt = DateTime.UtcNow.ToIST()
            };         
           
            _context.tbl_part.Add(part);
            await _context.SaveChangesAsync();
            TempData.setAlert("Part added successfully!", AlertTypes.Success);            
            return RedirectToAction("Index","Home");                                
        }



        // GET: PartController/Edit/5 get edit form with data saved from db
        public async Task<ActionResult> EditPart(int id)
        {           
            // Safely fetch EmployeeId from claims
            var empId = int.Parse(User.FindFirst("EmployeeId")!.Value);
            var partEntity = await _context.tbl_part.FindAsync(id);

            if (partEntity == null)
            {
                TempData.setAlert("Part Id does'nt exist!", AlertTypes.Error);
                return RedirectToAction("ViewParts");
            }
            // Map entity to DTO
            var partDto = new PartEditDto
            {
                PEmpId       = empId,   // override with current employee ID
                PName        = partEntity.PName,
                PDetail      = partEntity.PDetail,   
                PPrice       = partEntity.PPrice,
                PStock       = partEntity.PStock, 
                ImageUrl     = partEntity.ImagePath,
                
            };        
            return View(partDto);          
        }

        // POST: PartController/Edit/5  save the edited form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPart(int id,PartEditDto model)
        {
            var part = await _context.tbl_part.FindAsync(id);
            if (!ModelState.IsValid)
            {
                TempData.setAlert("Error: Model state invalid", AlertTypes.Error);                      
                return View(model);
            }
            var partEntity = await _context.tbl_part.FindAsync(model.PartId);
            if (partEntity == null)
            {
                TempData.setAlert("Part ID doesn't exist!", AlertTypes.Error);
                return RedirectToAction("ViewParts");
            }
            // Handle image upload if provided
            if (model.ImageUpload != null)
            {
                var result = await FileHelper.SavePartImageAsync(model.ImageUpload);
                if (!result.Success)
                {
                    ModelState.AddModelError("ImageUpload", result.ErrorMessage!);
                    TempData.setAlert(result.ErrorMessage!, AlertTypes.Error);
                    return View(model);
                }
                partEntity.ImagePath = result.FilePath;
                TempData.setAlert("Image updated successfully", AlertTypes.Success);
            }

            // Update other fields
            partEntity.PName = model.PName;
            partEntity.PDetail = model.PDetail;
            partEntity.PPrice = model.PPrice;
            partEntity.PStock = model.PStock;
            partEntity.CreatedAt = DateTime.UtcNow.ToIST(); // separate column has to added in table for updation

            await _context.SaveChangesAsync();
            TempData.setAlert("Part updated successfully!", AlertTypes.Success);
            return RedirectToAction("Part","PartList");
        }

        // GET: PartController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PartController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> ViewParts(string searchBy, string searchValue)
        {
            var partQuery = _context.tbl_part.AsQueryable();
            //Filter logic
            if (string.IsNullOrEmpty(searchValue))
            {
                if(searchBy == "id" && int.TryParse(searchValue, out int id))
                {
                    partQuery = partQuery.Where(p => p.PartId == id);
                }else if (searchBy == "name")
                {
                    partQuery = partQuery.Where(p => p.PName!.Contains(searchValue));
                }
            }
            //var parts = await partQuery.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View();

        }
    }
}
