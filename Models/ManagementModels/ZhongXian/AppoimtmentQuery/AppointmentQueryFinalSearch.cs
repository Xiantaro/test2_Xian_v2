using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery;

namespace test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery
{
    public class AppointmentQueryFinalSearch
    {
        private readonly Test2Context _context;
        public AppointmentQueryFinalSearch(Test2Context context)
        {
            _context = context;
        }
        public async Task<AppointmentQueryViewModel> AppointmentQuerySearch(AppoimtmentQueryFilter filter)
        {
            Debug.WriteLine("進入預約管理..............................");

            bool AppointmentEmptyFilter()
            {
                return filter.appointment_reservationId == null &&
                    filter.appointment_UserID == null &&
                    filter.appointment_bookCode == null &&
                    filter.appointment_initDate == null &&
                    filter.appointment_lastDate == null &&
                    filter.appointment_state == "ALL";
            }
            
            var result =   _context.Reservations.Include(x => x.Book).Include(y => y.Collection).Include(z => z.ReservationStatus).Select(final => new AppointmentQueryResultDTO()
            {
                appointmentId = final.ReservationId,
                bookCode = final.Book!.BookCode,
                title = final.Collection.Title,
                cid = final.CId,
                appointmentDate = final.ReservationDate,
                appointmentDue = final.DueDateR,
                appointmentStatus = final.ReservationStatus.ReservationStatus1
            }).AsQueryable();
            if (AppointmentEmptyFilter()) { result = result.Where(ap => ap.appointmentDate <= DateTime.Now && ap.appointmentDate >= DateTime.Now.AddMonths(-2)); }
            if (filter.appointment_reservationId != 0) { result = result.Where(x => x.appointmentId == filter.appointment_reservationId); }
            if (filter.appointment_UserID != 0) { result = result.Where(x => x.cid == filter.appointment_UserID); }
            if (filter.appointment_bookCode != null) { result = result.Where(x => x.bookCode == filter.appointment_bookCode); }
            if (filter.appointment_state != "ALL") { result = result.Where(x => x.appointmentStatus == filter.appointment_state); }
            if (filter.appointment_initDate != null || filter.appointment_lastDate != null)
            {
                if (filter.appointment_initDate <= filter.appointment_lastDate) { result = result.Where(x => filter.appointment_initDate <= x.appointmentDate && x.appointmentDate <= filter.appointment_lastDate); }
                if (filter.appointment_initDate >= filter.appointment_lastDate) { result = result.Where(x => filter.appointment_initDate >= x.appointmentDate && x.appointmentDate >= filter.appointment_lastDate); }
            }
            if (filter.appointment_orderDate == "desc") { result = result.OrderByDescending(x => x.appointmentDate); }
            else { result = result.OrderBy(x => x.appointmentDate); }

         var totalCount = await result.CountAsync();

            var finalResult = await result.Skip((filter.page  - 1) * filter.appointment_perPage ).Take(filter.appointment_perPage).ToListAsync();
            
            var AppointmentViewModels = new AppointmentQueryViewModel()
            {
                AppointmentQueryResultDTOs = finalResult,
                TotalCount = totalCount,
                CurrentPage = filter.page,
                perPage = filter.appointment_perPage
            };
            Debug.WriteLine("準備回傳.......預約查詢結果");
            return AppointmentViewModels;
        }
    }
}
