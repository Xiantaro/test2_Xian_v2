using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using test2.Services;
using System.Threading;
using test2.Models;
using Microsoft.EntityFrameworkCore;

namespace test2.Services
{
    public class ReservationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFacotry;
        private readonly PeriodicTimer _timer;

        public ReservationService(IServiceScopeFactory scopeFacotry)
        {
            _scopeFacotry = scopeFacotry;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {   
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _scopeFacotry.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Test2Context>();

                var pending = await context.Reservations.Where(x => x.ReservationStatusId == 2).ToListAsync();
                int count = pending.Count();
                Debug.WriteLine($"預約次數: {count}");
            }
        }
        public override void Dispose()
        {
            _timer.Dispose();
            base.Dispose();
        }
    }
}
