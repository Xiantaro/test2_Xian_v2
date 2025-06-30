using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using test2.Models;

namespace test2.Models.ManagementModels.ZhongXian.BorrowQuery
{
    public class BorrowQueryFinalSearch
    {
        private readonly Test2Context _context;
        public BorrowQueryFinalSearch(Test2Context context)
        {
            _context = context;
        }

        //public async Task<BorrowQueryViewModel> BorrowQuerySeach(BorrowQueryFilter filter)
        //{

        //    return 1;
        //}



    }
}
