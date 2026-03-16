using Microsoft.EntityFrameworkCore;
using Order_Management_System.Services.Interfaces;
using Order_Management_System.Serviecs; // Lưu ý check lại chính tả namespace này (Services hay Serviecs)
using Order_Management_System.Data;
using Order_Management_System.Middlewares;
using Order_Management_System.Services;

namespace Order_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. CẤU HÌNH CORS: Cho phép Frontend (VueJS) truy cập API
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") // Port mặc định của Vite/Vue
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Add Controllers
            builder.Services.AddControllers();

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 2. DATABASE: Kết nối MySQL Docker
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                ));

            // 3. DEPENDENCY INJECTION: Đăng ký các Service
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IReportService, ReportService>();

            var app = builder.Build();

            // 4. MIDDLEWARE: Xử lý lỗi toàn cục (Nên đặt đầu tiên)
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // 5. SWAGGER: Chỉ chạy trong môi trường Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 6. KÍCH HOẠT CORS: Phải đặt TRƯỚC HttpsRedirection và Authorization
            app.UseCors("AllowVueApp");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}