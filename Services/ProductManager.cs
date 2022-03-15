using DataAccess;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductManager
    {
        private readonly AgencyContext _context;

        public ProductManager(AgencyContext context)
        {
            _context = context;
        }
        public async Task Add(Product product)
        {
            _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var selecetedProduct = await _context.Products.FirstOrDefaultAsync(x=>x.Id==id && !x.IsDeleted);
            if (selecetedProduct == null) return;
            selecetedProduct.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public List<Product> GetAll(Expression<Func<Product,bool>> filter=null)
        {
            return _context.Products
                .Where(c => !c.IsDeleted)
                .Include(c => c.Category)
                .Include(c => c.ProductPictures).ThenInclude(c => c.Picture)
                .Include(c => c.ProductRecords)
                .Where(filter)
                 .OrderByDescending(c=>c.ModifiedOn).ToList();
        }
        public async Task<List<Product>> GetAllAdmin()
        {
            return await _context.Products
                .Where(c => c.IsDeleted)
                .Include(c => c.Category)
                .Include(c => c.ProductRecords).
                OrderByDescending(c => c.ModifiedOn).ToListAsync();
        }
        public async Task<List<Product>> SearchProduct(string? q, int? categoryId, decimal? minPrice, decimal? maxPrice, int? sortBy)
        {
            var products = _context.Products
                .Where(c => !c.IsDeleted)
                .Include(c => c.Category)
                .Include(c => c.ProductRecords)
                .Include(c => c.ProductPictures).ThenInclude(c => c.Picture)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                products = products.Where(c => c.ProductRecords.Any(c=>c.LanguageId==1 && c.Name.ToLower().Contains(q.ToLower())));
            }
            if (categoryId.HasValue)
            {
                products = products.Where(c => c.CategoryId == categoryId);
            }
           
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                products = products.Where(c => c.Price >= minPrice && c.Price <= maxPrice);
            }
            if (sortBy != null)
            {
                switch (sortBy)
                {
                    case 1:
                        products = products.OrderByDescending(c => c.Price);
                        break;
                    case 2:
                        products = products.OrderBy(c => c.Price);
                        break;
                    default:
                        products = products.OrderByDescending(c => c.PublishDate);

                        break;
                }
            }

            return await products.ToListAsync();
        }

        public async Task<Product?> GetById(int id)
        {
            var selectedProduct = await _context.Products
                .Include(c => c.ProductRecords).Include(c => c.ProductPictures).ThenInclude(c => c.Picture)
                .FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);
            if (selectedProduct == null) return null;
            return selectedProduct;
        }
    }
}
