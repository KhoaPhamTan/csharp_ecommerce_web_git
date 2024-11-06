using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStoreLibrary.DTOs;

namespace PetStoreAPI.Endpoints;

public static class UserEndpoint
{
    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users");

        // Route dành cho Admin
        group.MapGet("/admin", [Authorize(Roles = "Admin")] () =>
            Results.Ok("Welcome Admin!"));

        // Route dành cho Customer
        group.MapGet("/customer", [Authorize(Roles = "Customer")] () =>
            Results.Ok("Welcome Customer!"));

        return group;
    }
}
