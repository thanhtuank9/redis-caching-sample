using RedisCaching.Service.ViewModels;
using System.Collections.Generic;

namespace RedisCaching.Service.Services
{
    public static class SamplePropertyData
    {
        public static List<PropertyItemModel> GetProperties()
        {
            System.Threading.Thread.Sleep(3000);

            return new List<PropertyItemModel>
                   {
                       new PropertyItemModel{ Id = 1, Name = "Property 1", Address = "NY 1"},
                       new PropertyItemModel{ Id = 2, Name = "Property 2", Address = "NY 2"},
                       new PropertyItemModel{ Id = 3, Name = "Property 3", Address = "NY 3"},
                       new PropertyItemModel{ Id = 4, Name = "Property 4", Address = "NY 4"},
                       new PropertyItemModel{ Id = 5, Name = "Property 4", Address = "NY 5"},
                       new PropertyItemModel{ Id = 6, Name = "Property 6", Address = "NY 6"},
                       new PropertyItemModel{ Id = 7, Name = "Property 7", Address = "NY 7"}
                   };
        }
    }
}
