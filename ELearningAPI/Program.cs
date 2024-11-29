using ELearningAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ELearningAPI.Controllers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddUserSecrets<Program>();

//Thêm dịch vụ Token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
    if(!string.IsNullOrEmpty(secretKey))
    {
        secretKey = builder.Configuration["TokenSecretKey"];
    }
    if (secretKey != null)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    }
});

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
    string connectionString = $"Server={serverName};Port=22588;Database=ELearning;" +
        $"User={MySQLUserName};Password={MySQLPassword};SslMode=REQUIRED;SslCa={caCertPath};";

    // Cấu hình DbContext với chuỗi kết nối MySQL
    builder.Services.AddDbContext<ELearningDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}

// Đăng ký dịch vụ DbContext với MySQL. Sử dụng ở local
else
{
    builder.Services.AddDbContext<ELearningDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MysqlConnection"),
        new MySqlServerVersion(new Version(8, 0, 29))));
}    
// Thêm CORS vào dịch vụ
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:3000", "https://e-learning-project-alpha.vercel.app") // Cho phép các trang web được sử dụng
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
app.UseCors("AllowSpecificOrigins");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "ELearningAPI project is running.");
app.MapGet("/api", () => "ELearningAPI api project is running.");

app.Run();
