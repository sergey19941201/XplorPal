using System;
using System.Data;
using System.IO;
using SQLite;

namespace RecyclerViewSample.ORM
{
    public class DBRepository
    {
        //code to create DB
        public string CreateDB()
        {
            var output = "";
            output += "creating DB";
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            output += "\nDB created";
            return output;
        }

        //code to create table
        public string CreateUsersTable()
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                db.CreateTable<UsersDataTable>();
                string result = "table created successfully";
                return result;
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }
        //code to drop table
        public string DropUsersTable()
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                db.DropTable<UsersDataTable>();
                string result = "table dropped successfully";
                return result;
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }
        //table to insert users
        public string InsertRecord(string name, string email, byte[] avatar, string api_token, string birth_date,
            string gender, string phone_num, string biography, int country_id, int user_id, string password)
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                UsersDataTable udt = new UsersDataTable();
                udt.name = name;
                udt.email = email;
                udt.avatar = avatar;
                udt.api_token = api_token;
                udt.birth_date = birth_date;
                udt.gender = gender;
                udt.phone_num = phone_num;
                udt.biography = biography;
                udt.country_id = country_id;
                udt.user_id = user_id;
                udt.password = password;
                db.Insert(udt);
                return "Record added";
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }

        public string RemoveUsersData(int id)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var item = db.Get<ORM.UsersDataTable>(id);
            db.Delete(item);
            return "record Deleted..";
        }

        public string CreatePlacesOfInterestTable()
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                db.CreateTable<PlacesOfInterestTable>();
                string result = "table created successfully";
                return result;
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }

        public string DropPlacesOfInterestTable()
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                db.DropTable<PlacesOfInterestTable>();
                string result = "table dropped successfully";
                return result;
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }

        public string InsertPlacesOfInterestRecord(string name, string price, string lat, string lng)
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                PlacesOfInterestTable poi_table = new PlacesOfInterestTable();
                poi_table.name = name;
                poi_table.price = price;
                poi_table.lat = lat;
                poi_table.lng = lng;
                db.Insert(poi_table);
                return "Record added";
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }

        public string updatePlacesOfInterestTable(int usersId, string name, string price, string lat, string lng)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var item = db.Get<PlacesOfInterestTable>(usersId);
            try
            {
                item.name = name;
                item.price = price;
                item.lat = lat;
                item.lng = lng;
                db.Update(item);
            }
            catch { }

            return "Record Updated...";
        }

        public string RemovePlacesOfInterestRecord(int id)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var item = db.Get<ORM.PlacesOfInterestTable>(id);
            db.Delete(item);
            return "record Deleted..";
        }

        public string CreateWishlistTable()
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                db.CreateTable<Wishlist>();
                string result = "table created successfully";
                return result;
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }

        public string DropWishlistTable()
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                db.DropTable<Wishlist>();
                string result = "table dropped successfully";
                return result;
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }

        public string InsertWhishlistRecord(string name, string id_public, string price, string picData, string description,
            string location, string duration, string min_capacity, string max_capacity, string lat, string lng, bool isMyExperience)
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                Wishlist wishlist_table = new Wishlist();
                wishlist_table.name = name;
                wishlist_table.id_public = id_public;
                wishlist_table.price = price;
                wishlist_table.picData = picData;
                wishlist_table.description = description;
                wishlist_table.location = location;
                wishlist_table.duration = duration;
                wishlist_table.min_capacity = min_capacity;
                wishlist_table.max_capacity = max_capacity;
                wishlist_table.lat = lat;
                wishlist_table.lng = lng;
                wishlist_table.isMyExperience = isMyExperience;
                db.Insert(wishlist_table);
                return "Record added";
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }

        public string RemoveWishlistRecord(int id)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var item = db.Get<Wishlist>(id);
            db.Delete(item);
            return "record Deleted..";
        }
    }
}