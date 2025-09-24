using car_kaashiv_web_app.Data;
using car_kaashiv_web_app.Models.DTOs;
using car_kaashiv_web_app.Models.Entities;
using car_kaashiv_web_app.Models.Enums;
using car_kaashiv_web_app.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace car_kaashiv_web_app.Controllers
{
  
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
      //Get AdminDashboard
        public IActionResult AdminDashboard()
        {
            return View();
        }      

        // GET: All Employee list
        public async Task<IActionResult> Index()
        {
            return View(await _context.tbl_emp.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableEmployee = await _context.tbl_emp
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableEmployee == null)
            {
                return NotFound();
            }

            return View(tableEmployee);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,Email,Role,EmpPasswordHash,CreatedAt")] TableEmployee tableEmployee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableEmployee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tableEmployee);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {   
            var employee = await _context.tbl_emp.FindAsync(id);
            if (employee == null)
            {
                
                TempData.setAlert("Employee Id does'nt exist!", AlertTypes.Error);
            }
            //Mapping to dto
            var dto = new EmployeeEditDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Phone = employee.Phone,
                Email = employee.Email,
                Role = employee.Role,
                CreatedAt = employee.CreatedAt

            };
            return View(dto);
        }      
        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeEditDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);
            var employee = await _context.tbl_emp.FindAsync(dto.Id);

            if (employee == null)
            {                
                TempData.setAlert("Employee Id does'nt exist!", AlertTypes.Success);
            }
            //manual mapping back to entity
            employee.Name  = dto.Name;
            employee.Phone = dto.Phone;
            employee.Email = dto.Email;
            employee.Role  = dto.Role;   
            await _context.SaveChangesAsync();
            TempData.setAlert("Updated Successfully!", AlertTypes.Success);
            return RedirectToAction("AdminDashboard", "Admin");
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tableEmployee = await _context.tbl_emp
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableEmployee == null)
            {
                return NotFound();
            }

            return View(tableEmployee);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)

        {
            TempData.setAlert("Employee deleted successfully", AlertTypes.Success);
            //TempData.setAlert("Employee deleted successfully", AlertTypes.Error);
            //var tableEmployee = await _context.tbl_emp.FindAsync(id);
            //if (tableEmployee != null)
            //{
            //    _context.tbl_emp.Remove(tableEmployee);
            //}

            //await _context.SaveChangesAsync();
            ////return View();
            ////TempData.setAlert(AlertTypes.Success, "Employee deleted successfully");
            //TempData.setAlert("Employee deleted successfully", AlertTypes.Error);
            await Task.Delay(1000);
            return RedirectToAction(nameof(Index));
        }

      
    }
}
