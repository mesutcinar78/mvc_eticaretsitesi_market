using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCeTicaret.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required, MaxLength(50)]
        public string CategoryName { get; set; }//ana kategori

        [MaxLength(250)]
        public string Description { get; set; }

        [MaxLength(500)]

        public string Picture1 { get; set; }

        [MaxLength(500)]

        public string Picture2 { get; set; }

        public bool IsActive { get; set; }

        //category ve 3 tane alt subcategory imiz var bunlar kendi aralarinda bire çok ilişkili burda çok taraflarini yapiyoruz
        public virtual ICollection<Category> SubCategories { get; set; }

    }
}