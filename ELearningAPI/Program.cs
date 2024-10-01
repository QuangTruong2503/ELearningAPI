using ELearningAPI.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddUserSecrets<Program>();

// Lấy chứng chỉ SSL từ biến môi trường
var sslCaCert = Environment.GetEnvironmentVariable("SSL_CA_CERT");
if (!string.IsNullOrEmpty(sslCaCert))
{
    // Tạo file tạm thời chứa nội dung chứng chỉ
    var caCertPath = "/tmp/ca.pem";  // Đường dẫn tạm thời
    System.IO.File.WriteAllText(caCertPath, sslCaCert);
    //Lấy tên máy chủ MySQL từ biến môi trường
    var serverName = Environment.GetEnvironmentVariable("MYSQL_SERVER_NAME");
    //Lấy tên tài khoản MySQL từ biến môi trường
    var MySQLUserName = Environment.GetEnvironmentVariable("MYSQL_USER_NAME");
    //Lấy mật khẩu MySQL từ biến môi trường
    var MySQLPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

    // Cập nhật chuỗi kết nối MySQL với đường dẫn chứng chỉ
    string connectionString = $"Server={serverName};Port=22588;Database=ELearning;User={MySQLUserName};Password={MySQLPassword};SslMode=REQUIRED;SslCa={caCertPath};";

    // Cấu hình DbContext với chuỗi kết nối MySQL
    builder.Services.AddDbContext<ELearningDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}

// Đăng ký dịch vụ DbContext với MySQL
else
{
    builder.Services.AddDbContext<ELearningDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MysqlConnection"),
        new MySqlServerVersion(new Version(8, 0, 29))));
}    
// Thêm CORS vào dịch vụ
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin() // Địa chỉ frontend của bạn
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Áp dụng chính sách CORS
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "ELearningAPI project is running.");
app.MapGet("/api", () => "ELearningAPI api project is running.");
app.Run();
