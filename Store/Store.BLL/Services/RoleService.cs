using Microsoft.AspNetCore.Identity;
using Store.DAL.Data.Entities;
using Store.BLL.Interfaces;
using Microsoft.Extensions.Logging;

namespace Store.BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            RoleManager<IdentityRole<int>> roleManager,
            ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task EnsureRolesExist()
        {
            try
            {
                // Get all enum values from UserRole
                var roles = Enum.GetValues(typeof(UserRole))
                              .Cast<UserRole>()
                              .Select(r => r.ToString());

                foreach (var role in roles)
                {
                    if (!await RoleExistsAsync(role))
                    {
                        await CreateRoleAsync(role);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring roles exist");
                throw;
            }
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            try
            {
                return await _roleManager.RoleExistsAsync(roleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if role exists: {RoleName}", roleName);
                throw;
            }
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            try
            {
                if (await RoleExistsAsync(roleName))
                {
                    _logger.LogWarning("Attempt to create existing role: {RoleName}", roleName);
                    return false;
                }

                var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully created role: {RoleName}", roleName);
                    return true;
                }

                _logger.LogWarning("Failed to create role {RoleName}: {Errors}", 
                    roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role: {RoleName}", roleName);
                throw;
            }
        }

        public async Task<bool> DeleteRoleAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning("Attempt to delete non-existent role: {RoleName}", roleName);
                    return false;
                }

                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully deleted role: {RoleName}", roleName);
                    return true;
                }

                _logger.LogWarning("Failed to delete role {RoleName}: {Errors}", 
                    roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role: {RoleName}", roleName);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            try
            {
                return _roleManager.Roles.Select(r => r.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all roles");
                throw;
            }
        }

        public async Task<bool> UpdateRoleAsync(string currentRoleName, string newRoleName)
        {
            try
            {
                // Check if current role exists
                var role = await _roleManager.FindByNameAsync(currentRoleName);
                if (role == null)
                {
                    _logger.LogWarning("Attempt to update non-existent role: {CurrentRoleName}", currentRoleName);
                    return false;
                }

                // Check if new role name already exists
                if (await RoleExistsAsync(newRoleName))
                {
                    _logger.LogWarning("Attempt to update role to existing name: {NewRoleName}", newRoleName);
                    return false;
                }

                // Update role name
                role.Name = newRoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully updated role from {CurrentRoleName} to {NewRoleName}", 
                        currentRoleName, newRoleName);
                    return true;
                }

                _logger.LogWarning("Failed to update role {CurrentRoleName}: {Errors}", 
                    currentRoleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role from {CurrentRoleName} to {NewRoleName}", 
                    currentRoleName, newRoleName);
                throw;
            }
        }
    }
} 
