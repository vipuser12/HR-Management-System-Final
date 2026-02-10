using HRManagementSys.Data;
using HRManagementSys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HRManagementSys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("seed-data")]
        public async Task<IActionResult> SeedTestData()
        {
            try
            {
                
                await SeedRolesAndPermissions();

                
                var existingUsers = await _context.Users.AnyAsync();
                if (existingUsers)
                {
                    return BadRequest(new { message = "Test data already exists" });
                }

                // Create test users
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@hr.com",
                    PasswordHash = HashPassword("admin123"),
                    FirstName = "Admin",
                    LastName = "User",
                    IsActive = true
                };

                var hrManagerUser = new User
                {
                    Username = "hrmanager",
                    Email = "hrmanager@hr.com",
                    PasswordHash = HashPassword("hr123"),
                    FirstName = "HR",
                    LastName = "Manager",
                    IsActive = true
                };

                var hrFinancialUser = new User
                {
                    Username = "hrfinancial",
                    Email = "hrfinancial@hr.com",
                    PasswordHash = HashPassword("finance123"),
                    FirstName = "HR",
                    LastName = "Financial",
                    IsActive = true
                };

                _context.Users.AddRange(adminUser, hrManagerUser, hrFinancialUser);
                await _context.SaveChangesAsync();

                // Get roles from database 
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                var hrManagerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "HR Manager");
                var hrFinancialRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "HR Financial");

                // Assign roles to users
                if (adminRole != null)
                {
                    _context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
                }

                if (hrManagerRole != null)
                {
                    _context.UserRoles.Add(new UserRole { UserId = hrManagerUser.Id, RoleId = hrManagerRole.Id });
                }

                if (hrFinancialRole != null)
                {
                    _context.UserRoles.Add(new UserRole { UserId = hrFinancialUser.Id, RoleId = hrFinancialRole.Id });
                }

                await _context.SaveChangesAsync();

                // Create test employees
                var adminEmployee = new Employee
                {
                    UserId = adminUser.Id,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@hr.com",
                    PhoneNumber = "123-456-7890",
                    Department = "IT",
                    JobTitle = "System Administrator",
                    HireDate = DateTime.Now.AddMonths(-12),
                    IsActive = true
                };

                var hrManagerEmployee = new Employee
                {
                    UserId = hrManagerUser.Id,
                    FirstName = "HR",
                    LastName = "Manager",
                    Email = "hrmanager@hr.com",
                    PhoneNumber = "123-456-7891",
                    Department = "Human Resources",
                    JobTitle = "HR Manager",
                    HireDate = DateTime.Now.AddMonths(-6),
                    IsActive = true
                };

                var hrFinancialEmployee = new Employee
                {
                    UserId = hrFinancialUser.Id,
                    FirstName = "HR",
                    LastName = "Financial",
                    Email = "hrfinancial@hr.com",
                    PhoneNumber = "123-456-7892",
                    Department = "Finance",
                    JobTitle = "Payroll Specialist",
                    HireDate = DateTime.Now.AddMonths(-3),
                    IsActive = true
                };

                _context.Employees.AddRange(adminEmployee, hrManagerEmployee, hrFinancialEmployee);
                await _context.SaveChangesAsync();

                // Create test salaries
                _context.Salaries.AddRange(
                    new Salary { EmployeeId = adminEmployee.Id, BaseSalary = 80000, EffectiveDate = DateTime.Now.AddMonths(-12) },
                    new Salary { EmployeeId = hrManagerEmployee.Id, BaseSalary = 70000, EffectiveDate = DateTime.Now.AddMonths(-6) },
                    new Salary { EmployeeId = hrFinancialEmployee.Id, BaseSalary = 65000, EffectiveDate = DateTime.Now.AddMonths(-3) }
                );

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Test data seeded successfully",
                    users = new[] {
                        new { username = "admin", password = "admin123", role = "Admin" },
                        new { username = "hrmanager", password = "hr123", role = "HR Manager" },
                        new { username = "hrfinancial", password = "finance123", role = "HR Financial" }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error seeding test data", error = ex.Message });
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private async Task SeedRolesAndPermissions()
        {
            // Check if permissions already exist
            if (!await _context.Permissions.AnyAsync())
            {
                var permissions = new List<Permission>
                {
                    new Permission { Name = "view_employees", Description = "View employee information" },
                    new Permission { Name = "create_employee", Description = "Create new employee" },
                    new Permission { Name = "edit_employee", Description = "Edit employee information" },
                    new Permission { Name = "view_payroll", Description = "View salary information" },
                    new Permission { Name = "set_salary", Description = "Set and manage employee salary" },
                    new Permission { Name = "manage_users", Description = "Create and manage user accounts" },
                    new Permission { Name = "manage_roles", Description = "Manage roles and permissions" }
                };

                _context.Permissions.AddRange(permissions);
                await _context.SaveChangesAsync();
            }

            // Check if roles already exist
            if (!await _context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Admin", Description = "Full system access" },
                    new Role { Name = "HR Manager", Description = "HR Operations - view and manage employees" },
                    new Role { Name = "HR Financial", Description = "Payroll management - set salaries" }
                };

                _context.Roles.AddRange(roles);
                await _context.SaveChangesAsync();

                // Assign permissions to roles
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                var hrManagerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "HR Manager");
                var hrFinancialRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "HR Financial");

                if (adminRole != null)
                {
                    // Admin gets all permissions
                    var allPermissions = await _context.Permissions.ToListAsync();
                    foreach (var permission in allPermissions)
                    {
                        _context.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = permission.Id });
                    }
                }

                if (hrManagerRole != null)
                {
                    var hrManagerPermissions = await _context.Permissions
                        .Where(p => new[] { "view_employees", "create_employee", "edit_employee", "manage_users" }.Contains(p.Name))
                        .ToListAsync();
                    
                    foreach (var permission in hrManagerPermissions)
                    {
                        _context.RolePermissions.Add(new RolePermission { RoleId = hrManagerRole.Id, PermissionId = permission.Id });
                    }
                }

                if (hrFinancialRole != null)
                {
                    var hrFinancialPermissions = await _context.Permissions
                        .Where(p => new[] { "view_payroll", "set_salary" }.Contains(p.Name))
                        .ToListAsync();
                    
                    foreach (var permission in hrFinancialPermissions)
                    {
                        _context.RolePermissions.Add(new RolePermission { RoleId = hrFinancialRole.Id, PermissionId = permission.Id });
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
