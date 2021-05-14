using Microsoft.AspNetCore.Mvc.Rendering;
using StoneWorld_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneWorld_DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<InquiryHeader>
    {
        void Update(OrderHeader obj);
    }
}
