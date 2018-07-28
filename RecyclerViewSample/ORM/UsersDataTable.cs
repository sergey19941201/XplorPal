using System;
using System.Data;
using System.IO;
using SQLite;


namespace RecyclerViewSample.ORM
{
    [Table("UsersDataTable")]
    public class UsersDataTable
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public byte[] avatar { get; set; }
        public string api_token { get; set; }
        public string birth_date { get; set; }
        public string gender { get; set; }
        public string phone_num { get; set; }
        public string biography { get; set; }
        public int country_id { get; set; }
        public int user_id { get; set; }
        public string password { get; set; }
    }
}