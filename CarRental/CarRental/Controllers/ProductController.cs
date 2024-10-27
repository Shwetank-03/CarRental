using CarRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarRental.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        CarEntities db = new CarEntities();
        Response response = new Response();

        [HttpPost, Route("addNewProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewProduct([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("Authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                product.status = "true";
                db.Products.Add(product);
                db.SaveChanges();

                response.message = "Product Added Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpGet, Route("getAllProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllProduct()
        {
            try
            {
                var result = from product in db.Products
                             join category in db.Categories
                             on product.categoryId equals category.id
                             select new
                             {
                                 product.id,
                                 product.name,
                                 product.description,
                                 product.price,
                                 product.status,
                                 categoryId = category.id,
                                 categoryName = category.name
                             };

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpGet, Route("getProductByCategory/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetProductByCategory(int id)
        {
            try
            {
                var result = db.Products
                    .Where(x => x.categoryId == id && x.status == "true")
                    .Select(x => new { x.id, x.name })
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpGet, Route("getProductById/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetProductById(int id)
        {
            try
            {
                Product productObj = db.Products.Find(id);
                return Request.CreateResponse(HttpStatusCode.OK, productObj);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        public HttpResponseMessage UpdateProduct([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("Authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                Product productObj = db.Products.Find(product.id);

                if (productObj == null)
                {
                    response.message = "Product id does not exist";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                productObj.name = product.name;
                productObj.categoryId = product.categoryId;
                productObj.description = product.description;
                productObj.price = product.price;
                db.Entry(productObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = "Product Updated Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost, Route("deleteProduct/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteProduct(int id)
        {
            try
            {
                var token = Request.Headers.GetValues("Authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                Product productObj = db.Products.Find(id);

                if (productObj == null)
                {
                    response.message = "Product id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                db.Products.Remove(productObj);
                db.SaveChanges();
                response.message = "Product Deleted Successfully";

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost, Route("updateProductStatus")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateProductStatus([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("Authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                Product productObj = db.Products.Find(product.id);

                if (productObj == null)
                {
                    response.message = "Product id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                productObj.status = product.status;
                db.Entry(productObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                response.message = "Product Status Updated Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }



    }
}
