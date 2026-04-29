using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAllTests();
            Console.ReadKey();
        }

        static void RunAllTests()
        {
            Console.WriteLine("ЗАПУСК 20 ЮНИТ-ТЕСТОВ\n");

            // 1. Равносторонний
            TestResult(1, "5,5,5", "равносторонний", triangle("5", "5", "5").type);

            // 2. Равнобедренный
            TestResult(2, "5,5,3", "равнобедренный", triangle("5", "5", "3").type);

            // 3. Разносторонний
            TestResult(3, "6,8,10", "разносторонний", triangle("6", "8", "10").type);

            // 4. Не треугольник (нарушение неравенства)
            TestResult(4, "1,1,3", "не треугольник", triangle("1", "1", "3").type);

            // 5. Некорректный ввод (буквы) - возвращает пустую строку
            TestResult(5, "abc,5,5", "", triangle("abc", "5", "5").type);

            // 6. Отрицательная сторона
            TestResult(6, "-5,5,5", "не треугольник", triangle("-5", "5", "5").type);

            // 7. Нулевая сторона
            TestResult(7, "0,5,5", "не треугольник", triangle("0", "5", "5").type);

            // 8. Маленькие стороны
            TestResult(8, "0.1,0.1,0.1", "равносторонний", triangle("0.1", "0.1", "0.1").type);

            // 9. Большие стороны (координаты клиппингуются)
            var result9 = triangle("100", "100", "100");
            TestResult(9, "100,100,100", "равносторонний", result9.type);
            TestCoord(9, result9.points);

            // 10. Прямоугольный треугольник
            TestResult(10, "3,4,5", "разносторонний", triangle("3", "4", "5").type);

            // 11. Плавающая точка
            TestResult(11, "5.5,5.5,6.0", "равнобедренный", triangle("5.5", "5.5", "6.0").type);

            // 12. Очень большие стороны
            var result12 = triangle("1000", "1000", "1000");
            TestResult(12, "1000,1000,1000", "равносторонний", result12.type);
            bool coordsOk = result12.points.TrueForAll(p => p.Item1 <= 100 && p.Item2 <= 100);
            TestResultExtra(12, "1000,1000,1000", "координаты в пределах 100", coordsOk ? "OK" : "FAIL");

            // 13. Пустая строка
            TestResult(13, ",5,5", "", triangle("", "5", "5").type);

            // 14. Строка с пробелами
            TestResult(14, " 5 ,5,5", "равносторонний", triangle(" 5 ", "5", "5").type);

            // 15. Проверка лога (успех)
            File.Delete("log.txt");
            triangle("5", "5", "5");
            bool logExists = File.Exists("log.txt");
            TestResultExtra(15, "5,5,5", "лог создан", logExists ? "OK" : "FAIL");

            // 16. Проверка лога (ошибка)
            triangle("abc", "5", "5");
            string logContent = File.Exists("log.txt") ? File.ReadAllText("log.txt") : "";
            bool hasError = logContent.Contains("нечисловые данные");
            TestResultExtra(16, "abc,5,5", "лог содержит ошибку", hasError ? "OK" : "FAIL");

            // 17. Треугольник с целыми и дробными
            TestResult(17, "5.0,5.00,5", "равносторонний", triangle("5.0", "5.00", "5").type);

            // 18. Предельный случай неравенства
            TestResult(18, "2,3,5", "не треугольник", triangle("2", "3", "5").type);

            // 19. Уникальность точек
            var result19 = triangle("6", "8", "10");
            bool unique = result19.points[0] != result19.points[1] &&
                          result19.points[1] != result19.points[2] &&
                          result19.points[0] != result19.points[2];
            TestResultExtra(19, "6,8,10", "точки разные", unique ? "OK" : "FAIL");

            // 20. Координаты неотрицательные
            var result20 = triangle("0.5", "0.5", "0.3");
            bool nonNegative = result20.points.TrueForAll(p => p.Item1 >= 0 && p.Item2 >= 0);
            TestResultExtra(20, "0.5,0.5,0.3", "координаты >= 0", nonNegative ? "OK" : "FAIL");
        }

        static void TestResult(int num, string input, string expected, string actual)
        {
            bool passed = expected == actual;
            Console.WriteLine($"Тест {num}: {(passed ? "Да" : "Нет")} [{input}] Ожидалось: {expected}, Получено: {actual}");
        }

        static void TestResultExtra(int num, string input, string expected, string actual)
        {
            bool passed = expected == actual;
            Console.WriteLine($"Тест {num}: {(passed ? "Да" : "Нет")} [{input}] Ожидалось: {expected}, Получено: {actual}");
        }

        static void TestCoord(int num, List<(int, int)> points)
        {
            bool ok = points.TrueForAll(p => p.Item1 >= 0 && p.Item1 <= 100 && p.Item2 >= 0 && p.Item2 <= 100);
            Console.WriteLine($"Тест {num}: {(ok ? "Да" : "Нет")} [100,100,100] Координаты в пределах [0,100]: {ok}");
        }

        static (string type, List<(int, int)> points) triangle(string s1, string s2, string s3)
        {
            if (!float.TryParse(s1, out float a) || !float.TryParse(s2, out float b) || !float.TryParse(s3, out float c))
                return ("", new List<(int, int)> { (-2, -2), (-2, -2), (-2, -2) });

            if (a <= 0 || b <= 0 || c <= 0)
                return ("не треугольник", new List<(int, int)> { (-1, -1), (-1, -1), (-1, -1) });

            if (a + b <= c || a + c <= b || b + c <= a)
                return ("не треугольник", new List<(int, int)> { (-1, -1), (-1, -1), (-1, -1) });

            string type = a == b && b == c ? "равносторонний" : (a == b || a == c || b == c ? "равнобедренный" : "разносторонний");

            return (type, Coords(a, b, c));
        }

        static List<(int, int)> Coords(float a, float b, float c)
        {
            int x1 = 10, y1 = 90;
            int x2 = x1 + (int)(c + 0.5f);
            int y2 = y1;

            double xx = (a * a - b * b + c * c) / (2 * c);
            double yy = Math.Sqrt(Math.Abs(a * a - xx * xx));

            int x3 = Math.Clamp(x1 + (int)(xx + 0.5), 0, 100);
            int y3 = Math.Clamp(y1 - (int)(yy + 0.5), 0, 100);

            return new List<(int, int)> { (x1, y1), (x2, y2), (x3, y3) };
        }
    }
}