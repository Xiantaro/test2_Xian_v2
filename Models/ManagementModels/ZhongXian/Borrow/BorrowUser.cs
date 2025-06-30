using System.ComponentModel.DataAnnotations;

namespace test2.Models.ManagementModels.ZhongXian.Borrow
{
    public class BorrowUser
    {
        [Key]
        public int cId { get; set; }
        public string? cName { get; set; }
    }
}
