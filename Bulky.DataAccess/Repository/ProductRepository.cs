using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDBContext _db;
        public ProductRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            _db.Update(product);
            //this is when you want to custom update
            //but luckly efcore already handles it for yu
/*            var productFromDB = _db.Products.FirstOrDefault(u => u.Id == product.Id);
            if(productFromDB != null)
            {
                productFromDB.Id = product.Id;
                productFromDB.Title=product.Title;
                productFromDB.Description=product.Description;
                productFromDB.CategoryId=product.CategoryId;
                productFromDB.Price=product.Price;
                productFromDB.Price100=product.Price100;
                productFromDB.Price50=product.Price50;
                productFromDB.ListPrice=product.ListPrice;
                productFromDB.Author=product.Author;
                productFromDB.ISBN=product.ISBN;
                if(product.ImageUrl != null)
                {
                    productFromDB.ImageUrl=product.ImageUrl;
                }

            }*/
        }
    }
}
