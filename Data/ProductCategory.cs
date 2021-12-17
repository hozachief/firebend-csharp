using System;
using System.Collections.Generic;

#nullable disable

namespace Data
{
    public partial class ProductCategory
    {


        public int ProductCategoryId { get; set; }
        public string Name { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
