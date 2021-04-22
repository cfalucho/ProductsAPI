using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    public class ProductsController : ApiController
    {
        //GET - Retrieve data
        // api/products
        public IHttpActionResult GetAllProducts()
        {
            IList<ProductViewModel> products = null;
            using(var x = new ProductsDBContext())
            {
                products = x.Products.Select(p => new ProductViewModel()
                {
                    Id = p.ID,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    Price = (int)p.Price,
                    Featured = p.Featured
                }).ToList<ProductViewModel>();
            }

            if (products.Count == 0)
                return NotFound();

            return Ok(products);
        }


        //POST - Insert new record
        public IHttpActionResult PostNewProducts(ProductViewModel products)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid Data");

                using (var x = new ProductsDBContext())
                {
                    x.Products.Add(new Product()
                    {
                        ProductName = products.ProductName,
                        ProductDescription = products.ProductDescription,
                        Price = products.Price,
                        Featured = products.Featured
                    });

                    x.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

      

            return Ok();
        }

        //PUT - Update data base on id

        public IHttpActionResult PutProducts(ProductViewModel products)
        {
            if (!ModelState.IsValid)
                return BadRequest("This is invalid model.");

            using(var x = new ProductsDBContext())
            {
                var checkExistingProduct = x.Products.Where(p => p.ID == products.Id).FirstOrDefault<Product>();

                if (checkExistingProduct != null)
                {
                    checkExistingProduct.ProductName = products.ProductName;
                    checkExistingProduct.ProductDescription = products.ProductDescription;
                    checkExistingProduct.Price = products.Price;

                    x.SaveChanges();

                }
                else
                    return NotFound();
            }

            return Ok();
        }

        //DELETE - Delete a record based on ID
        //api/products/1
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Enter valid Customer Id");

            using (var x = new ProductsDBContext())
            {
                var product = x.Products.Where(p => p.ID == id).FirstOrDefault();

                x.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                x.SaveChanges();
            }

            return Ok();
        }
    }
}
