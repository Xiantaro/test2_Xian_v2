﻿using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using test2.Models;
using test2.Models.ManagementModels.ZhongXian.Normal;
using test2.Services;

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
            if(await _timer.WaitForNextTickAsync(stoppingToken))
            {
                Debug.WriteLine("零晨排程開始!");
                using var scope = _scopeFacotry.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Test2Context>();
                
                Debug.WriteLine("逾期取書檢查開始~~~!!!");
                var result = await context.Set<MessageDTO>().FromSqlInterpolated($"EXEC OverDue").ToListAsync();
                Debug.WriteLine(result[0].Message);
                Debug.WriteLine("逾期歸還檢查開始~~~!!!");
                var result2 = await context.Set<MessageDTO>().FromSqlInterpolated($"EXEC LateReturn").ToListAsync();
                Debug.WriteLine(result2[0].Message);
                Debug.WriteLine("即將預期溫馨提醒開始~~~!!!");
                var result3 = await context.Set<MessageDTO>().FromSqlInterpolated($"EXEC LateReturnOneToThree").ToListAsync();
                Debug.WriteLine(result3[0].Message);
                        
                Debug.WriteLine("排程結束.......");
            }
            _timer.Dispose();
        }
        public override void Dispose()
        {
            _timer.Dispose();
            base.Dispose();
        }
    }
}
