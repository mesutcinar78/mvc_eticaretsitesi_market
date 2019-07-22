using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCeTicaret.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryID { get; set; }

        [Required,ForeignKey("Category")]
        public int CategoryID { get; set; }

        [Required,MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string Picture1 { get; set; }

        [MaxLength(500)]
        public string Picture2 { get; set; }

        public bool IsActive { get; set; }

        //category ve 3 tane alt subcategory imiz var bunlar kendi aralarinda bire çok ilişkili burda tek taraflarini yapiyoruz

        //yukarida foreign key var ve bizde tekil yazicaz şimdi buradada

        public virtual Category Category { get; set; }

        //her bir alt kategorinin birden cok urunu olabicegi icin list seklnde yaptik
        public virtual ICollection<Product>Products { get; set; }
    }
}