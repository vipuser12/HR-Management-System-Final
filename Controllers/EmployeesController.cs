using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HRManagementSys.Data;
using HRManagementSys.Models;

namespace HRManagementSys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [Authorize(Policy = "view_employees")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
        {
            var employees = await _context.Employees
                .Include(e => e.Salary)
                .ToListAsync();

            return Ok(employees);
        }


        [Authorize(Policy = "view_employees")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Salary)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return NotFound("الموظف غير موجود");

            return Ok(employee);
        }


        [Authorize(Policy = "create_employees")]
        [HttpPost]
        public async Task<ActionResult<Employee>> Create(Employee employee)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }


        [Authorize(Policy = "create_employees")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            if (id != employee.Id) return BadRequest("ID الموظف غير متطابق");

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }


        [Authorize(Policy = "create_employees")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الموظف بنجاح" });
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}