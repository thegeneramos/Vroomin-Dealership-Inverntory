using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Assignment10._3
{
    public class InventoryConnection
    {
        public static Table<Car> cocheInventory()
        {
            CarInventoryDataContext carInventoryDataContext = new CarInventoryDataContext();
            return carInventoryDataContext.GetTable<Car>();
        }
    }
}
