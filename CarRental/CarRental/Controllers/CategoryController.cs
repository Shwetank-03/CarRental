using CarRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarRental.Controllers
{
    [RoutePrefix("api/category")]
    public class CategoryController : ApiController
    {
        CarEntities db = new CarEntities();
        Response response = new Response();

        [HttpPost, Route("addNewCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewCategory([FromBody] Category category)
        {
            try
            {
                var token = Request.Headers.GetValues("Authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                if (tokenClaim.Role != "admin")
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                db.Categories.Add(category);
                db.SaveChanges();

                response.message = "Category Added Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpGet, Route("getAllCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllCategory()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, db.Categories.ToList());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet, Route("get")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetCategories()
        {
            try
            {
                var token = Request.Headers.GetValues("Authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                // Retrieve all categories without any filtering.
                var categories = db.Categories.ToList();

                return Request.CreateResponse(HttpStatusCode.OK, categories);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost, Route("updateCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateCategory(Category category)
        {
            try
            {
                var token = Request.Headers.GetValues("Authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                Category categoryObj = db.Categories.Find(category.id);

                if (categoryObj == null)
                {
                    response.message = "Category id does not exist";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                if (category.name != null) // Check if category.name is not null before updating
                {
                    categoryObj.name = category.name;
                    db.Entry(categoryObj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response.message = "Category Updated Successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.message = "Category name cannot be null";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
