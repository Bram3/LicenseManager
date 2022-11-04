using LicenseManager.Data;
using LicenseManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LicenseManager.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ProductController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return BadRequest(new Response<string> { Status = "Error", Data = "Not logged in!" });
        var id = await _userManager.GetUserIdAsync(user);
        var products = await _context.Products.Where(a => a.OwnerId == id).ToArrayAsync();

        return Ok(new Response<Product[]> { Status = "Success", Data = products });
    }

    // GET api/product/5
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> Get(Guid productId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return BadRequest(new Response<string> { Status = "Error", Data = "Not logged in!" });
        var id = await _userManager.GetUserIdAsync(user);
        var products = _context.Products.Where(a => a.OwnerId == id && a.Id == productId);
        var count = await products.CountAsync();
        if (count <= 0)
            return NotFound(new Response<string> { Status = "Error", Data = "Product not found!" });
        var product = await products.FirstOrDefaultAsync();
        if (product is null)
            return NotFound(new Response<string> { Status = "Error", Data = "Product not found!" });
        return Ok(new Response<Product> { Status = "Success", Data = product });
    }

    // POST api/product
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Product product)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return BadRequest(new Response<string> { Status = "Error", Data = "Not logged in!" });
        var id = await _userManager.GetUserIdAsync(user);
        product.OwnerId = id;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(new Response<Product> { Status = "Success", Data = product });
    }

    // PUT api/product/5
    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> Put(Guid productId, [FromBody] Product product)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return BadRequest(new Response<string> { Status = "Error", Data = "Not logged in!" });
        var id = await _userManager.GetUserIdAsync(user);
        var products = _context.Products.Where(a => a.OwnerId == id && a.Id == productId);
        var count = await products.CountAsync();
        if (count <= 0)
            return NotFound(new Response<string> { Status = "Error", Data = "Product not found!" });
        var result = await products.FirstOrDefaultAsync();
        if (result is null)
            return NotFound(new Response<string> { Status = "Error", Data = "Product not found!" });
        result.Name = product.Name;
        _context.Products.Update(result);
        await _context.SaveChangesAsync();
        return Ok(new Response<Product> { Status = "Success", Data = result });
    }

    // DELETE api/product/5
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Delete(Guid productId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return BadRequest(new Response<string> { Status = "Error", Data = "Not logged in!" });
        var id = await _userManager.GetUserIdAsync(user);
        var products = _context.Products.Where(a => a.OwnerId == id && a.Id == productId);
        var count = await products.CountAsync();
        if (count <= 0)
            return NotFound(new Response<string> { Status = "Error", Data = "Product not found!" });
        var result = await products.FirstOrDefaultAsync();
        if (result is null)
            return NotFound(new Response<string> { Status = "Error", Data = "Product not found!" });
        _context.Products.Remove(result);
        await _context.SaveChangesAsync();
        return Ok(new Response<string> { Status = "Success", Data = "Successfully deleted the product!" });
    }
}