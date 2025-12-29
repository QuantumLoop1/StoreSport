using Microsoft.EntityFrameworkCore;

namespace StoreSport.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    logger.LogInformation("Проверка миграций...");
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        logger.LogInformation("Применение миграций...");
                        context.Database.Migrate();
                    }

                    logger.LogInformation("Проверка товаров в БД...");
                    var productCount = context.Products.Count();
                    logger.LogInformation($"Найдено товаров: {productCount}");

                    if (!context.Products.Any())
                    {
                        logger.LogInformation("База данных пустая. Добавление начальных данных...");

                        context.Products.AddRange(
                            new Product
                            {
                                Name = "Kayak",
                                Description = "A boat for one person",
                                Category = "Watersports",
                                Price = 275
                            },
                            new Product
                            {
                                Name = "Lifejacket",
                                Description = "Protective and fashionable",
                                Category = "Watersports",
                                Price = 48.95m
                            },
                            new Product
                            {
                                Name = "Soccer Ball",
                                Description = "FIFA-approved size and weight",
                                Category = "Soccer",
                                Price = 19.50m
                            },
                            new Product
                            {
                                Name = "Corner Flags",
                                Description = "Give your playing field a professional touch",
                                Category = "Soccer",
                                Price = 34.95m
                            },
                            new Product
                            {
                                Name = "Stadium",
                                Description = "Flat-packed 35,000-seat stadium",
                                Category = "Soccer",
                                Price = 79500
                            },
                            new Product
                            {
                                Name = "Thinking Cap",
                                Description = "Improve brain efficiency by 75%",
                                Category = "Chess",
                                Price = 16
                            },
                            new Product
                            {
                                Name = "Unsteady Chair",
                                Description = "Secretly give your opponent a disadvantage",
                                Category = "Chess",
                                Price = 29.95m
                            },
                            new Product
                            {
                                Name = "Human Chess Board",
                                Description = "A fun game for the family",
                                Category = "Chess",
                                Price = 75
                            },
                            new Product
                            {
                                Name = "Bling-Bling King",
                                Description = "Gold-plated, diamond-studded King",
                                Category = "Chess",
                                Price = 1200
                            }
                        );

                        context.SaveChanges();
                        logger.LogInformation($"Добавлено {context.Products.Count()} товаров в базу данных");
                    }
                    else
                    {
                        logger.LogInformation($"В базе данных уже есть {productCount} товаров");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при инициализации базы данных");
                    throw;
                }
            }
        }
    }
}
