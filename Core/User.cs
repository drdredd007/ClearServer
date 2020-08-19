using System.Data.Linq.Mapping;
namespace ClearServer
{
    [Table(Name = "Users")]
    class User
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int uid { get; set; }
        [Column]
        public string login { get; set; }
        [Column]
        public string password { get; set; }
        [Column]
        public string name { get; set; }
        [Column]
        public string city { get; set; }
        [Column]
        public string date { get; set; }
        [Column]
        public string skills { get; set; }

    }
}