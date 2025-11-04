🚀 Smart Job Scheduler
یک کتابخانه قدرتمند و انعطاف‌پذیر برای زمان‌بندی و اجرای تسک‌های پس‌زمینه در دات‌نت با پشتیبانی از سه روش مختلف زمان‌بندی.

📋 فهرست مطالب
ویژگی‌ها

نصب و راه‌اندازی

پیکربندی

انواع زمان‌بندی

آموزش کامل Cron

توسعه Jobهای جدید

مثال‌های پیشرفته

لاگ‌گیری و مانیتورینگ

تست و دیباگ

🌟 ویژگی‌ها
✅ پشتیبانی از سه روش زمان‌بندی (ساده، پیشرفته، Cron)
✅ کانفیگ متمرکز و خوانا
✅ لاگ‌گیری جامع و قابلیت رهگیری
✅ مدیریت خطا و قابلیت اطمینان بالا
✅ قابلیت توسعه آسان
✅ پشتیبانی از محدودیت‌های زمانی پیچیده
✅ فالوبک هوشمند برای زمان‌های غیرمجاز

🚀 نصب و راه‌اندازی
1. prerequisite ها
NET 8.0.

Visual Studio 2022 یا VS Code
2. اضافه کردن به پروژه
xml
<!-- SmartJobScheduler.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Worker">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="NCronTab" Version="1.3.1" />
  </ItemGroup>
</Project>
3. ثبت سرویس‌ها
csharp
// Program.cs
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSmartJobScheduler(builder.Configuration);
var host = builder.Build();
await host.RunAsync();

⚙️ پیکربندی
ساختار پایه appsettings.json
json
{
  "JobScheduler": {
    "Jobs": [
      {
        "Name": "JobName",
        "ScheduleType": "Simple|Advanced|Cron",
        "IsEnabled": true,
        // سایر تنظیمات بر اساس نوع زمان‌بندی
      }
    ]
  }
}
🕒 انواع زمان‌بندی
1. 🎯 زمان‌بندی ساده (Simple)
برای Jobهای با الگوی زمانی ساده و تکراری

json
{
  "Name": "DailyReportJob",
  "ScheduleType": "Simple",
  "IsEnabled": true,
  "Hour": 14,
  "Minute": 30,
  "Period": 1440,
  "StartImmediately": false
}
پارامترها:

Hour: ساعت اجرا (0-23)

Minute: دقیقه اجرا (0-59)

Period: دوره تناوب به دقیقه

StartImmediately: شروع فوری یا انتظار برای زمان مشخص

2. 🚀 زمان‌بندی پیشرفته (Advanced)
برای الگوهای زمانی پیچیده و business-specific

json
{
  "Name": "DataCleanupJob",
  "ScheduleType": "Advanced",
  "IsEnabled": true,
  "AllowedDays": ["Monday", "Wednesday", "Friday"],
  "TimeRestrictions": [
    {
      "DayOfWeek": "Monday",
      "StartTime": "02:00:00",
      "EndTime": "04:00:00",
      "ExcludedDates": ["2024-12-25", "2024-01-01"]
    }
  ],
  "ActiveDateRange": {
    "StartDate": "2024-01-01",
    "EndDate": "2024-12-31"
  },
  "StartImmediately": true
}
3. ⏰ زمان‌بندی Cron
برای استفاده از قدرت expressionهای استاندارد Cron

json
{
  "Name": "HealthMonitorJob",
  "ScheduleType": "Cron",
  "IsEnabled": true,
  "CronExpression": "*/5 * * * *",
  "StartImmediately": true
}
📚 آموزش کامل Cron
ساختار Cron Expression
text
┌───────────── ثانیه (0-59)
│ ┌───────────── دقیقه (0-59)  
│ │ ┌───────────── ساعت (0-23)
│ │ │ ┌───────────── روز ماه (1-31)
│ │ │ │ ┌───────────── ماه (1-12)  
│ │ │ │ │ ┌───────────── روز هفته (0-6)  
│ │ │ │ │ │
* * * * * *
عملگرهای Cron
عملگر	مثال	توضیح
*	* * * * *	هر واحد زمانی
,	0 8,12,18 * * *	مقادیر متعدد
-	0 9-17 * * *	محدوده
/	*/15 * * * *	تناوب
?	0 12 ? * MON	مقدار نامشخص
مثال‌های پرکاربرد Cron
🔹 اجرای منظم
cron
*/5 * * * *      // هر 5 دقیقه
0 * * * *        // هر ساعت
0 */2 * * *      // هر 2 ساعت
0 0 * * *        // هر روز نیمه‌شب
0 0 * * 0        // هر یکشنبه نیمه‌شب
🔹 اجرای تجاری
cron
0 9-17 * * 1-5   // ساعت کاری دوشنبه تا جمعه
0 0 1 * *        // اول هر ماه
0 12 1 1 *       // اول ژانویه ساعت 12
0 8,12,18 * * *  // 8صبح، 12ظهر، 6عصر
🔹 اجرای پیچیده
cron
0 0-7,22-23 * * 1-5    // شب‌های کاری
0 30 9,14 * * MON,WED  // دوشنبه و چهارشنبه 9:30 و 14:30
0 0 1,15 * *           // اول و پانزدهم هر ماه
روزهای هفته در Cron
عدد	روز
0	یکشنبه
1	دوشنبه
2	سه‌شنبه
3	چهارشنبه
4	پنجشنبه
5	جمعه
6	شنبه
نکته: برخی سیستم‌ها از 7 برای یکشنبه استفاده می‌کنند

💻 توسعه Jobهای جدید
1. ایجاد Command جدید
csharp
// Models/Commands/NewFeatureCommand.cs
namespace SmartJobScheduler.Models
{
    public record NewFeatureCommand : ICommand
    {
        public string Parameter1 { get; init; }
        public int Parameter2 { get; init; }
    }
}
2. ایجاد Job جدید
csharp
// Jobs/NewFeatureJob.cs
using SmartJobScheduler.Models;

namespace SmartJobScheduler.Jobs
{
    public class NewFeatureJob : BaseSmartJob<NewFeatureCommand, NewFeatureJob>
    {
        public NewFeatureJob(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<NewFeatureJob> logger,
            IOptions<JobScheduleOption> jobSchedule) 
            : base(serviceScopeFactory, logger, jobSchedule)
        {
        }
    }
}
3. ثبت Job در DI Container
csharp
// Extensions/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmartJobScheduler(this IServiceCollection services, IConfiguration configuration)
    {
        // ... کدهای موجود
        
        // اضافه کردن Job جدید
        services.AddHostedService<NewFeatureJob>();
        
        return services;
    }
}
4. اضافه کردن کانفیگ
json
{
  "Name": "NewFeatureJob",
  "ScheduleType": "Cron",
  "IsEnabled": true,
  "CronExpression": "0 2 * * *",
  "StartImmediately": false
}
5. پیاده‌سازی منطق business
csharp
// Services/CommandDispatcherService.cs
private async Task ProcessCommand<TCommand>(TCommand command) where TCommand : class, ICommand
{
    switch (command)
    {
        case NewFeatureCommand newFeature:
            await ProcessNewFeatureCommand(newFeature);
            break;
        // سایر commandها...
    }
}

private async Task ProcessNewFeatureCommand(NewFeatureCommand command)
{
    _logger.LogInformation("Processing NewFeature with params: {Param1}, {Param2}", 
        command.Parameter1, command.Parameter2);
    
    // منطق business شما
    await Task.Delay(1000);
}
🎯 مثال‌های پیشرفته
مثال ۱: Job گزارش‌دهی فصلی
json
{
  "Name": "SeasonalReportJob",
  "ScheduleType": "Advanced",
  "IsEnabled": true,
  "AllowedDays": ["Monday"],
  "TimeRestrictions": [
    {
      "DayOfWeek": "Monday",
      "StartTime": "09:00:00",
      "EndTime": "11:00:00"
    }
  ],
  "ActiveDateRange": {
    "StartDate": "2024-03-21",
    "EndDate": "2024-06-20"
  }
}
مثال ۲: Job پردازش آخر هفته
json
{
  "Name": "WeekendProcessor",
  "ScheduleType": "Cron",
  "IsEnabled": true,
  "CronExpression": "0 0,6,12,18 * * 6,0",
  "StartImmediately": true
}
مثال ۳: Job پشتیبان‌گیری هوشمند
json
{
  "Name": "SmartBackupJob",
  "ScheduleType": "Advanced",
  "IsEnabled": true,
  "AllowedDays": ["Friday"],
  "TimeRestrictions": [
    {
      "DayOfWeek": "Friday",
      "StartTime": "22:00:00",
      "EndTime": "06:00:00",
      "ExcludedDates": ["2024-12-25", "2024-01-01"]
    }
  ]
}
📊 لاگ‌گیری و مانیتورینگ
ساختار لاگ‌ها
log
info: SmartJobScheduler.Jobs.DailyReportJob[0]
      Job DailyReportJob started at 2024-01-15 14:30:00
info: SmartJobScheduler.Services.CommandDispatcherService[0]
      Dispatching command: SimpleReportCommand
info: SmartJobScheduler.Services.CommandDispatcherService[0]
      Completed processing SimpleReportCommand
info: SmartJobScheduler.Jobs.DailyReportJob[0]
      Job DailyReportJob completed at 2024-01-15 14:30:01 (Duration: 1.0s)
Correlation ID برای رهگیری
هر اجرای Job یک Correlation ID منحصر به فرد دریافت می‌کند:

csharp
using (_logger.BeginScope("JobExecution {JobId}", Guid.NewGuid().ToString()))
{
    // اجرای Job
}
🐛 تست و دیباگ
1. تست زمان‌بندی
csharp
// واحد تست نمونه
[Test]
public void Should_Calculate_Next_Run_Time_Correctly()
{
    var jobOption = new JobOption 
    { 
        ScheduleType = ScheduleType.Simple,
        Hour = 14,
        Minute = 30
    };
    
    var scheduler = new JobSchedulerService();
    var nextRun = scheduler.GetNextRunTime(jobOption);
    
    Assert.That(nextRun, Is.Not.Null);
}
2. دیباگ کانفیگ
برای دیباگ مشکل تطبیق Jobها:

csharp
// در BaseSmartJob.cs
_logger.LogInformation("Available configurations: {JobCount}", 
    _jobScheduleOptions.Value.Jobs.Count);
3. ابزارهای مفید
Cron Validator Online: crontab.guru

JSON Validator: jsonlint.com

Dotnet Watch: dotnet run watch

🤝 مشارکت در پروژه
پروژه را Fork کنید

Branch جدید ایجاد کنید: git checkout -b feature/amazing-feature

تغییرات را commit کنید: git commit -m 'Add amazing feature'

به Branch اصلی push کنید: git push origin feature/amazing-feature

Pull Request ایجاد کنید

📄 لایسنس
این پروژه تحت لایسنس MIT منتشر شده است.

🆘 پشتیبانی
اگر با مشکلی مواجه شدید:

Issue را در GitHub ایجاد کنید

لاگ‌های کامل را attach کنید

کانفیگ مربوطه را share کنید

⭐ اگر این پروژه مفید بود، حتماً Star بدید!

🎯 با این کتابخانه می‌توانید هر نوع الگوی زمان‌بندی پیچیده‌ای را پیاده‌سازی کنید!
