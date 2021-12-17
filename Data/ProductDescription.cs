using System;
using System.Collections.Generic;

#nullable disable

namespace Data
{
    public partial class ProductDescription
    {


        public int ProductDescriptionId { get; set; }
        public string Description { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
