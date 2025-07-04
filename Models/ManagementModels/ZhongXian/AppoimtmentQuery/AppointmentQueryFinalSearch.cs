using Microsoft.EntityFrameworkCore;

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
            bool AppointmentEmptyFilter() 
            {
                return filter.appointment_reservationId == null &&
                    filter.appointment_UserID == null &&
                    filter.appointment_bookCode == null &&
                    filter.appointment_initDate == null &&
                    filter.appointment_lastDate == null &&
                    filter.appointment_state == "ALL";
            }

            var result = _context.Reservations.Include(x => x.Book).Include(y => y.Collection).Include(z => z.ReservationStatus).Select(final => new appointmentQueryResultDTO
            {
                appointmentId = final.ReservationId,
                bookCode = final.Book.BookCode,
                title = final.Collection.Title,
                cid = final.CId,
                appointmentDate = final.ReservationDate,
                appointmentDue = final.DueDateR,
                appointmentStatus = final.ReservationStatus.ReservationStatus1
            }).AsQueryable();
            //if (AppointmentEmptyFilter()) { result = result.Where(ap => ap.appointmentDate <= DateTime.Now && ap.appointmentDate >= DateTime.Now.AddMonths(-2)); }
            //if (filter.appointment_reservationId != null)




            var totalCount = await result.CountAsync();
            //var finalResult = await result.Skip(filter.page - 1)

            var AppointmentViewModels = new AppointmentQueryViewModel()
            {
                AppointmentQueryResultDTOs = result,
                TotalCount = totalCount,
                Currenp
            }


            return
        }
    }
}
