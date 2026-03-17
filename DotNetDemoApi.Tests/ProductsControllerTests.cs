using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DotNetDemoApi.Controllers;
using DotNetDemoApi.Data;
using DotNetDemoApi.Models;

namespace DotNetDemoApi.Tests
{
    public class ProductsControllerTests
    {
        private static AppDbContext CreateDb(string name = null)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(name ?? "Test_" + System.Guid.NewGuid().ToString("N"))
                .Options;
            var db = new AppDbContext(options);
            db.Products.AddRange(
                new Product { Name = "Test Product", Price = 10.00m },
                new Product { Name = "Another", Price = 5.50m });
            db.SaveChanges();
            return db;
        }

        [Fact]
        public async Task GetAll_ReturnsSeededProducts()
        {
            using var db = CreateDb();
            var controller = new ProductsController(db);

            var result = await controller.GetAll();
            var ok = result.Result as OkObjectResult;
            Assert.NotNull(ok);
            var products = ok.Value as System.Collections.Generic.IEnumerable<Product>;
            Assert.NotNull(products);
            var list = products.ToList();
            Assert.Equal(2, list.Count);
            Assert.Contains(list, p => p.Name == "Test Product" && p.Price == 10.00m);
        }

        [Fact]
        public async Task Create_ReturnsCreatedProductWithId()
        {
            using var db = CreateDb();
            var controller = new ProductsController(db);
            var request = new CreateProductRequest { Name = "New Item", Price = 99.99m };

            var result = await controller.Create(request);
            var created = result.Result as CreatedAtActionResult;
            Assert.NotNull(created);
            var product = created.Value as Product;
            Assert.NotNull(product);
            Assert.True(product.Id > 0);
            Assert.Equal("New Item", product.Name);
            Assert.Equal(99.99m, product.Price);

            Assert.Equal(3, await db.Products.CountAsync());
        }

        [Fact]
        public async Task Create_NullName_ReturnsBadRequest()
        {
            using var db = CreateDb();
            var controller = new ProductsController(db);

            var result = await controller.Create(new CreateProductRequest { Name = null, Price = 1m });
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_NegativePrice_ReturnsBadRequest()
        {
            using var db = CreateDb();
            var controller = new ProductsController(db);

            var result = await controller.Create(new CreateProductRequest { Name = "X", Price = -1m });
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
