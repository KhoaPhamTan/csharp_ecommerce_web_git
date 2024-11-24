namespace PetStoreAPI.Entities
{
    public enum UserRole
    {
        Customer,   // Default
        Admin,      // Admin
        Employee    // Employee
    }

    public class UserEntity
    {
        public int Id { get; set; } // Ensure userId is an integer
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; } // Mark as required
        public required string Address { get; set; } // Mark as required
        public UserRole Role { get; set; } = UserRole.Customer;  // Default to Customer
    }
}
