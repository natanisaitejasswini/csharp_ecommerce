using System.Collections.Generic;
using System;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using ecomm.Models;
using CryptoHelper;

namespace EcommApp.Factory
{
    public class ProductsRepository : IFactory<Product>
    {
        private string connectionString;
        public ProductsRepository()
        {
            connectionString = "server=localhost;userid=root;password=root;port=8889;database=ecommerce;SslMode=None";
        }

        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }
         public void AddProduct(Product product_item)
        {
             using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("INSERT INTO products (product_name, image, description, created_at, quantity, price, updated_at, user_id) VALUES (@product_name, @image, @description, NOW(), @quantity, @price, NOW(), @user_id);", product_item);
            }
        }
        public IEnumerable<Product> Products()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Product>("SELECT users.first_name, products.id, products.description, products.image, products.quantity, products.price, products.product_name , products.user_id FROM products LEFT JOIN users ON products.user_id = users.id;");
            }
        }
        public void AddOrder(Order order_item, int price)
        {
             using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute($"INSERT INTO orders (product_id, created_at, updated_at, quantity, user_id, price) VALUES (@product_id, NOW(), NOW(), @quantity, @user_id, '{price}');", order_item);
            }
        }
        public IEnumerable<Order> AllOrders()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Order>("SELECT users.first_name, products.product_name, orders.quantity, orders.price, orders.created_at, orders.id FROM orders, users, products WHERE products.id = orders.product_id AND users.id = orders.user_id");
            }
        }

        public int Orderprofile(string num)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var temp4 = dbConnection.Query<Order>($"SELECT orders.product_id FROM orders WHERE id = {num}").SingleOrDefault();
                return temp4.product_id;
            }
        }

         public int Extract_DeleteQty(string num)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var temp3 =  dbConnection.Query<Order>($"SELECT orders.quantity FROM orders WHERE id = {num}").SingleOrDefault();
                return temp3.quantity;
            }
        }
         public void DeleteOrder(string num, int quantity, int prod_num)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute($"DELETE FROM orders WHERE id = {num}");
                dbConnection.Execute($"UPDATE products SET quantity = quantity + '{quantity}' WHERE id = '{prod_num}'");
            }
        }
        public Product Update_qty(int id, int qty)
        {
             using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Product>($"UPDATE products SET quantity = quantity - '{qty}' WHERE id = '{id}'").FirstOrDefault();
            }
        }
         public Double Extract_Price(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var temp1 =  dbConnection.Query<Product>($"SELECT products.price FROM products WHERE id= '{id}'").SingleOrDefault();
                return temp1.price;
            }
        }
        public int Extract_Qunatity(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var temp2 =  dbConnection.Query<Product>($"SELECT products.quantity FROM products WHERE id= '{id}'").SingleOrDefault();
                return temp2.quantity;
            }
        }
        public IEnumerable<Product> ProductsofUsers(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Product>($"SELECT users.first_name, products.id, products.description, products.image, products.quantity, products.price, products.product_name , products.user_id FROM products,users WHERE products.user_id = users.id and products.user_id = '{id}';");
            }
        }
         public Product Edit_Product_Profile(string num)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Product>($"SELECT products.id, products.description, products.image, products.product_name, products.quantity, products.price FROM products WHERE id = {num}").FirstOrDefault();
            }
        }
        public Product Edit_Product(string name, string desc, string image, double price, int quantity, int id)
        {
             using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Product>($"UPDATE products SET product_name = '{name}', description = '{desc}', image = '{image}', quantity= '{quantity}', price = '{price}', created_at = NOW() WHERE id = '{id}';").FirstOrDefault();;
            }
        }
        public void DeleteProduct(string num)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute($"DELETE FROM products WHERE id = {num}");
            }
        }
    }
}

