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
                        logger.LogInformation("База данных пустая. Добавление тестовых товаров...");

                        context.Products.AddRange(
                            new Product
                            {
                                Name = "Футбольный мяч",
                                Description = "Профессиональный футбольный мяч FIFA",
                                Category = "Футбол",
                                Price = 2500
                            },
                            new Product
                            {
                                Name = "Баскетбольный мяч",
                                Description = "Мяч для баскетбола размер 7",
                                Category = "Баскетбол",
                                Price = 1800
                            },
                            new Product
                            {
                                Name = "Теннисная ракетка",
                                Description = "Профессиональная теннисная ракетка",
                                Category = "Теннис",
                                Price = 5500
                            },
                            new Product
                            {
                                Name = "Волейбольный мяч",
                                Description = "Мяч для волейбола",
                                Category = "Волейбол",
                                Price = 1500
                            },
                            new Product
                            {
                                Name = "Футбольные бутсы",
                                Description = "Профессиональные бутсы Nike",
                                Category = "Футбол",
                                Price = 8000
                            },
                            new Product
                            {
                                Name = "Баскетбольные кроссовки",
                                Description = "Кроссовки для баскетбола",
                                Category = "Баскетбол",
                                Price = 9500
                            },
                            new Product
                            {
                                Name = "Теннисные мячи",
                                Description = "Набор теннисных мячей 3 шт",
                                Category = "Теннис",
                                Price = 450
                            },
                            new Product
                            {
                                Name = "Волейбольная сетка",
                                Description = "Профессиональная волейбольная сетка",
                                Category = "Волейбол",
                                Price = 3500
                            },
                            new Product
                            {
                                Name = "Футбольная форма",
                                Description = "Комплект футбольной формы",
                                Category = "Футбол",
                                Price = 3200
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
