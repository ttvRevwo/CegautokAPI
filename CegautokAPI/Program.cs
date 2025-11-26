using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;



internal class Program
{
    public static string GenerateSalt()
    {
        Random random = new Random();
        string karakterek = "ABVDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        string salt = "";
        for (int i = 0; i < 64; i++)
        {
            salt += karakterek[random.Next(karakterek.Length)].ToString();
        }
        return salt;
    }

    public static string CreateSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
    private static void Main(string[] args)
    {


        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}