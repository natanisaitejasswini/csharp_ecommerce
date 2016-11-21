using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EcommApp.Factory;
using ecomm.Models;
using CryptoHelper;

namespace ecommerce.Controllers
{
    public class EcommController : Controller
    {
         private readonly EcommRepository ecommFactory;
         private readonly ProductsRepository productsFactory;

         public EcommController()
        {
            ecommFactory = new EcommRepository();
            productsFactory = new ProductsRepository();
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            return View("Login");
        }
// Post Methods:: Login, Registration
        [HttpPost]
        [Route("registration")]
        public IActionResult Create(User newuser)
        {   
            List<string> temp_errors = new List<string>();
            if(ModelState.IsValid)
            {
                 if(ecommFactory.FindEmail(newuser.email) == null){ // Checking email is registered previously
                    ecommFactory.Add(newuser);
                    ViewBag.User_Extracting = ecommFactory.FindByID();
                    int current_other_id = ViewBag.User_Extracting.id;
                    HttpContext.Session.SetInt32("current_id", (int) current_other_id);
                    return RedirectToAction("Dashboard");
                    }
                 else{
                    temp_errors.Add("Email is already in use");
                    TempData["errors"] = temp_errors;
                    return RedirectToAction("Index");
                }
            }
            foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("Index");
        }
        [HttpPost]
        [RouteAttribute("login")]
        public IActionResult Login(string email, string password)
        {
            List<string> temp_errors = new List<string>();
            if(email == null || password == null)
            {
                temp_errors.Add("Enter Email and Password Fields to Login");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Index");
            }
//Login User Id Extracting query
          User check_user = ecommFactory.FindEmail(email);
            if(check_user == null)
            {
                temp_errors.Add("Email is not registered");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Index");
            }
            bool correct = Crypto.VerifyHashedPassword((string) check_user.password, password);
            if(correct)
            {
                HttpContext.Session.SetInt32("current_id", check_user.id);
                return RedirectToAction("Dashboard");
            }
            else{
                temp_errors.Add("Password is not matching");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Index");
            }
        }
 //Dashboard start
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            //on refresh once after logout
            if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
            //Dashboard begins
            ViewBag.User_one = ecommFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            ViewBag.All_Users = ecommFactory.AllUsers();
            ViewBag.All_products = productsFactory.Products();
            return View("Dashboard");
        }
//View of adding new products
        [HttpGet]
        [Route("products")]
        public IActionResult Products()
        {
            //on refresh once after logout
            if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            ViewBag.All_products = productsFactory.Products();
            ViewBag.User_one = ecommFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return View("AddNewProduct");
        }
//post for products
        [HttpPost]
        [Route("addproduct")]
         public IActionResult AddProduct(Product newproduct)
        {
            List<string> temp_errors = new List<string>();
            if(ModelState.IsValid)
            {
                productsFactory.AddProduct(newproduct);
                 Console.WriteLine("product is Successfully added");
                 return RedirectToAction("Dashboard");
            }
            foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("Products");
        }
        [HttpGet]
        [Route("orders")]
        public IActionResult Orders()
        {
            //on refresh once after logout
            if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            ViewBag.User_one = ecommFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            ViewBag.All_Users = ecommFactory.AllUsers();
            ViewBag.All_products = productsFactory.Products();
            ViewBag.All_orders = productsFactory.AllOrders();
            return View("Orders");
        }
//post for orders
        [HttpPost]
        [Route("addorder")]
         public IActionResult Addorders(Order neworder)
        {
            List<string> temp_errors = new List<string>();
            if(ModelState.IsValid)
            {
                ViewBag.price = productsFactory.Extract_Price(neworder.product_id);
                int price = ViewBag.price * neworder.quantity;
                int extracted_quantity = productsFactory.Extract_Qunatity(neworder.product_id);
                if(neworder.quantity > extracted_quantity){
                    temp_errors.Add("Only " +  extracted_quantity +  " left");
                    TempData["errors"] = temp_errors;
                    return RedirectToAction("Orders");
                }
                productsFactory.AddOrder(neworder, price);
                productsFactory.Update_qty(neworder.product_id, neworder.quantity);
                return RedirectToAction("Orders");
            }
            foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("Orders");
        }
        [HttpGet]
        [Route("customers")]
        public IActionResult Customers()
        {
            //on refresh once after logout
            if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            ViewBag.User_one = ecommFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            ViewBag.All_Users = ecommFactory.AllUsers();
            ViewBag.All_products = productsFactory.Products();
            ViewBag.All_orders = productsFactory.AllOrders();
            return View("AddCustomers");
        }
        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult User_Delete(string id = "")
        {
            ViewBag.Find_Edit_User = ecommFactory.DeleteProfile(id);
            ViewBag.User_one = ecommFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return RedirectToAction("Customers");
        }
        [HttpPost]
        [Route("createNewUser")]
        public IActionResult AddnewUser(User newuser)
        {   
            List<string> temp_errors = new List<string>();
            if(ModelState.IsValid)
            {
                if(ecommFactory.FindEmail(newuser.email) == null){
                 ecommFactory.Add(newuser);
                 return RedirectToAction("Customers");
                }
                else{
                    temp_errors.Add("Email is already in use");
                    TempData["errors"] = temp_errors;
                    return RedirectToAction("Customers");
                }

            }
            foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("Customers");
        }
        [HttpGet]
        [Route("deleteorder/{id}")]
        public IActionResult Order_Delete(string id = "")
        {
            int quantity = productsFactory.Extract_DeleteQty(id);
            int productId_Increase = productsFactory.Orderprofile(id);
            productsFactory.DeleteOrder(id, quantity, productId_Increase);
            ViewBag.User_one = ecommFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return RedirectToAction("Orders");
        }
        [HttpGet]
        [Route("yourproducts")]
        public IActionResult YourProducts()
        {
            //on refresh once after logout
            if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.User_one = ecommFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            ViewBag.All_productsofUsers = productsFactory.ProductsofUsers((int)HttpContext.Session.GetInt32("current_id"));
            return View("UserProducts");
        }
        [HttpGet]
        [Route("editproduct/{id}")]
        public IActionResult Product_Edit(string id = "")
        {
             //on refresh
             if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
             if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            ViewBag.EditProduct_Profile = productsFactory.Edit_Product_Profile(id);
            ViewBag.User_one = ecommFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return View("EditProduct");
        }
        [HttpPost]
        [Route("editproduct")]
        public IActionResult Edit_Product(string product_name, string description, string image, double price, int quantity, int id)
        {   
            Console.WriteLine("editProduct::::" + price + "id is " +  id); 
            List<string> temp_errors = new List<string>();
            if(ModelState.IsValid)
            {
                productsFactory.Edit_Product(product_name, description, image, price, quantity, id);
            }
             foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("YourProducts");
        }
        [HttpGet]
        [Route("deleteproduct/{id}")]
        public IActionResult Product_Delete(string id = "")
        {
            productsFactory.DeleteProduct(id);
            ViewBag.User_one = ecommFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return RedirectToAction("YourProducts");
        }
    }
}
