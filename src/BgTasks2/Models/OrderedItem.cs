using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BgTasks2.Models;
public class OrderedItem
{
    public int OrderId { get; set; }
    public int CatalogItemId { get; set; }
    public int Units { get; set; }
}
