using LicenseManager.Data;
using LicenseManager.Models;
using LicenseManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LicenseController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly LicenseService _licenseService;
    private readonly UserManager<IdentityUser> _userManager;

    public LicenseController(UserManager<IdentityUser> userManager, ApplicationDbContext context,
        LicenseService licenseService)
    {
        _userManager = userManager;
        _context = context;
        _licenseService = licenseService;
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key)
    {
        var licenses = _context.Licenses.Where(a => a.Key == key);
        var count = await licenses.CountAsync();
        if (count <= 0)
            return NotFound(new Response<string> { Status = "Error", Data = "License not found!" });
        var result = await licenses.FirstOrDefaultAsync();
        if (result is null)
            return NotFound(new Response<string> { Status = "Error", Data = "License not found!" });
        return Ok(new Response<License> { Status = "Success", Data = result });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] LicenseNew licenseNew)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return BadRequest(new Response<string> { Status = "Error", Data = "Not logged in!" });
        var id = await _userManager.GetUserIdAsync(user);
        var products = _context.Products.Where(a => a.OwnerId == id && a.Id == licenseNew.ProductId);
        var count = await products.CountAsync();
        if (count <= 0)
            return NotFound(new Response<string> { Status = "Error", Data = "Product not found!" });
        var product = await products.FirstOrDefaultAsync();
        if (product is null)
            return NotFound(new Response<string> { Status = "Error", Data = "Product not found!" });

        var key = _licenseService.GetUniqueKey(24);
        var sameKeyCount = await _context.Licenses.Where(a => a.Key == key).CountAsync();
        while (sameKeyCount > 0)
        {
            key = _licenseService.GetUniqueKey(24);
            sameKeyCount = await _context.Licenses.Where(a => a.Key == key).CountAsync();
        }

        var license = new License { Expires = licenseNew.Expires, Key = key, ProductId = licenseNew.ProductId };

        _context.Licenses.Add(license);
        await _context.SaveChangesAsync();

        return Ok(new Response<License> { Status = "Success", Data = license });
    }
}