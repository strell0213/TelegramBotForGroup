using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Types;



namespace ConsoleApp3
{

    class Program
    {
        public static AppC ac;
        public static int isglobalchat = 0;
        public static string textglobal = "";
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [STAThread]
        static void Main(string[] args)
        {
            ShowWindow(GetConsoleWindow(), 1);
            string key = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите файл с ключом от телеграмм бота";
            if(openFileDialog.ShowDialog() == DialogResult.OK) { 
                StreamReader reader = new StreamReader(openFileDialog.FileName);
                key = reader.ReadToEnd();
                reader.Close();
            }
            
            ac = new AppC();
            var botClient = new TelegramBotClient(key);
            Console.WriteLine($"{botClient.ToString()} успешно подключен!\n\n\n");
            botClient.StartReceiving(Update, Error, null);
            Program.sys();
            Console.ReadLine();
        }
        public static void sys()
        {
            int w = 0;
            while (w < 1){
                string zap = Console.ReadLine();
                switch (zap)
                {
                    case "1":
                        Console.WriteLine("\nКлавиши для консоли: \n1 - Помощник команд\n2 - выдача очков по никнейму\n3 - вычет очков по никнейму\n4 - текст в чат от бота");
                        break;
                    case "2":
                        Console.WriteLine("Введите никнейм:");
                        string nik = Console.ReadLine();
                        var ew = ac.Users.Where(c => c.name == nik).FirstOrDefault();
                        if (ew != null)
                        {
                            Console.WriteLine("Введите сумму выдачи");
                            int sum = Convert.ToInt32(Console.ReadLine());
                            ew.lvl += sum;
                            ac.SaveChanges();
                            Console.WriteLine($"Успешно! У {ew.name} {ew.lvl} очков");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Такого ника не существует");
                            break;
                        }
                    case "3":
                        Console.WriteLine("Введите никнейм:");
                        string nik1 = Console.ReadLine();
                        var ew1 = ac.Users.Where(c => c.name == nik1).FirstOrDefault();
                        if (ew1 != null)
                        {
                            Console.WriteLine("Введите сумму вычета");
                            int sum1 = Convert.ToInt32(Console.ReadLine());
                            ew1.lvl -= sum1;
                            ac.SaveChanges();
                            Console.WriteLine($"Успешно! У {ew1.name} {ew1.lvl} очков");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Такого ника не существует");
                            break;
                        }
                     case "4":
                        Console.WriteLine("Введите текст");
                        textglobal = Console.ReadLine();
                        isglobalchat = 1;
                        break;
                }
            }
        }
        async static Task Error(ITelegramBotClient botClient, Exception error, CancellationToken token)
        {
            var ex = error.Message;
            Console.WriteLine("Ошибка: " + ex);
        }
        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            try
            {
                var message = update.Message;
                Console.WriteLine(message.From.Username + " отправил сообщение\nДата отправки: "+DateTime.Now+"\n"+message.Text+"\n");
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Бот для беседы ВАТНЫЙ СБ!\nСуть тупо активничай и за это получаешь уровень по приколу\n" +
                        "Команды бота:\n/mylvl - ваш уровень и звание\n/commands - существующие команды\n/table - таблица участников\n/ra - рандом ответ да или нет" +
                        "\n/roul - рулетка, при выграше добавятся очки (после ввода списываются 50 очков)");
                    return;
                }
                if (message.Text.ToLower() == "/commands")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Команды бота:\n/mylvl - ваш уровень и звание\n" +
                        "/commands - существующие команды\n/table - таблица участников\n/ra - рандом ответ да или нет\n" +
                        "/roul - рулетка, при выграше добавятся очки (после ввода списываются 50 очков)");
                    return;
                }
                if (message.Text.ToLower() == "/roul")
                {
                    var w = ac.Users.Where(c => c.name == message.From.Username).FirstOrDefault();
                    if(w.lvl < 50)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{w.name} у вас нет 50 очков для рулетки!");
                        return;
                    }
                    else
                    {
                        w.lvl -= 50;
                        ac.SaveChanges();
                        Random rand = new Random();
                        int ra = rand.Next(1, 100);
                        if(ra >= 0 && ra < 20)
                        {
                            w.lvl -= 20;
                            ac.SaveChanges();
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{w.name} у вас отняли 20 очков.\nКоличество очков - {w.lvl}");
                            return;
                        }
                        else if(ra >= 20 && ra < 60)
                        {
                            w.lvl += 50;
                            ac.SaveChanges();
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{w.name} вы ничего не выиграли...\nКоличество очков - {w.lvl}");
                            return;
                        }
                        else if(ra >= 60 && ra <= 80)
                        {
                            w.lvl += 100;
                            ac.SaveChanges();
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{w.name} вы выиграли 50 очков...\nКоличество очков - {w.lvl}");
                            return;
                        }
                        else if(ra == 81)
                        {
                            w.lvl += 1050;
                            ac.SaveChanges();
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Джекпот!!! {w.name} вы выиграли 1000 очков...\nКоличество очков - {w.lvl}");
                            return;
                        }
                        else if (ra >= 82 && ra <= 100)
                        {
                            w.lvl += 150;
                            ac.SaveChanges();
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"{w.name} вы выиграли 100 очков...\nКоличество очков - {w.lvl}");
                            return;
                        }
                    }
                    

                }
                if (message.Text.Contains("/game"))
                {
                    var w = ac.Users.Where(c => c.name == message.From.Username).FirstOrDefault();


                    string[] mas = message.Text.Split(' ');
                    string nick = mas[2].Remove(0,1);



                }
                if (message.Text.ToLower() == "/mylvl")
                {
                    var w = ac.Users.Where(c => c.name == message.From.Username).FirstOrDefault();
                    if (w == null)
                    {
                        User user = new User(message.From.Username, 0, "LOX");
                        ac.Users.Add(user);
                        ac.SaveChanges();
                        var w1 = ac.Users.Where(c => c.name == message.From.Username).FirstOrDefault();
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{w1.name}\nУровень: {w1.lvl} очков\nРанг: {w1.rank}");
                        return;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{w.name}\nУровень: {w.lvl} очков\nРанг: {w.rank}");
                        return;
                    }

                }
                if (message.Text.ToLower() == "/table")
                {
                    string all = "";
                    var w = ac.Users.ToList();
                    for (int i = 0; i <= w.Count - 1; i++) {
                        all += w[i].name + " (" + w[i].rank + ") - " + w[i].lvl + " очков!\n";
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, all);
                    return;
                }
                if(message.Text.ToLower() == "/ra")
                {
                    Random rand = new Random();
                    int ra = rand.Next(1, 4);
                    if(ra < 2)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Да");
                        return;
                    }
                    else if (ra > 2)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Нет");
                        return;
                    }
                }
                if (message.Text.ToLower() != null)
                {
                    var w = ac.Users.Where(c => c.name == message.From.Username).FirstOrDefault();
                    if (w != null)
                    {

                        if (w.lvl > 0 && w.lvl < 100)
                        {
                            w.rank = "Noob";
                        }
                        else if (w.lvl > 100 && w.lvl < 400)
                        {
                            w.rank = "Noob+";
                        }
                        else if (w.lvl > 400 && w.lvl < 800)
                        {
                            w.rank = "XuJIuGan";
                        }
                        else if (w.lvl > 800 && w.lvl < 1200)
                        {
                            w.rank = "Ăďΐďάś";
                        }
                        else if (w.lvl > 1200 && w.lvl < 1600)
                        {
                            w.rank = "†боJlьHoй†";
                        }
                        else if (w.lvl > 1600)
                        {
                            w.rank = "BOSS";
                        }
                        w.lvl += 10;
                        ac.SaveChanges();
                    }
                    else
                    {
                        User user = new User(message.From.Username, 0, "LOX");
                        ac.Users.Add(user);
                        ac.SaveChanges();
                    }
                }
                if(message.Document != null)
                {
                    var w = ac.Users.Where(c => c.name == message.From.Username).FirstOrDefault();
                    if (w != null)
                    {

                        if (w.lvl > 0 && w.lvl < 100)
                        {
                            w.rank = "Noob";
                        }
                        else if (w.lvl > 100 && w.lvl < 400)
                        {
                            w.rank = "Noob+";
                        }
                        else if (w.lvl > 400 && w.lvl < 800)
                        {
                            w.rank = "XuJIuGan";
                        }
                        else if (w.lvl > 800 && w.lvl < 1200)
                        {
                            w.rank = "Ăďΐďάś";
                        }
                        else if (w.lvl > 1200 && w.lvl < 1600)
                        {
                            w.rank = "†боJlьHoй†";
                        }
                        else if (w.lvl > 1600)
                        {
                            w.rank = "BOSS";
                        }
                        w.lvl += 30;
                        ac.SaveChanges();
                    }
                    else
                    {
                        User user = new User(message.From.Username, 0, "LOX");
                        ac.Users.Add(user);
                        ac.SaveChanges();
                    }
                }
                if(message.Video != null)
                {
                    var w = ac.Users.Where(c => c.name == message.From.Username).FirstOrDefault();
                    if (w != null)
                    {

                        if (w.lvl > 0 && w.lvl < 100)
                        {
                            w.rank = "Noob";
                        }
                        else if (w.lvl > 100 && w.lvl < 400)
                        {
                            w.rank = "Noob+";
                        }
                        else if (w.lvl > 400 && w.lvl < 800)
                        {
                            w.rank = "XuJIuGan";
                        }
                        else if (w.lvl > 800 && w.lvl < 1200)
                        {
                            w.rank = "Ăďΐďάś";
                        }
                        else if (w.lvl > 1200 && w.lvl < 1600)
                        {
                            w.rank = "†боJlьHoй†";
                        }
                        else if (w.lvl > 1600)
                        {
                            w.rank = "BOSS";
                        }
                        w.lvl += 70;
                        ac.SaveChanges();
                    }
                    else
                    {
                        User user = new User(message.From.Username, 0, "LOX");
                        ac.Users.Add(user);
                        ac.SaveChanges();
                    }
                }
                if(message.Photo != null)
                {
                    var w = ac.Users.Where(c => c.name == message.From.Username).FirstOrDefault();
                    if (w != null)
                    {

                        if (w.lvl > 0 && w.lvl < 100)
                        {
                            w.rank = "Noob";
                        }
                        else if (w.lvl > 100 && w.lvl < 400)
                        {
                            w.rank = "Noob+";
                        }
                        else if (w.lvl > 400 && w.lvl < 800)
                        {
                            w.rank = "XuJIuGan";
                        }
                        else if (w.lvl > 800 && w.lvl < 1200)
                        {
                            w.rank = "Ăďΐďάś";
                        }
                        else if (w.lvl > 1200 && w.lvl < 1600)
                        {
                            w.rank = "†боJlьHoй†";
                        }
                        else if (w.lvl > 1600)
                        {
                            w.rank = "BOSS";
                        }
                        w.lvl += 25;
                        ac.SaveChanges();
                    }
                    else
                    {
                        User user = new User(message.From.Username, 0, "LOX");
                        ac.Users.Add(user);
                        ac.SaveChanges();
                    }
                }
                if(isglobalchat == 1)
                {
                    isglobalchat = 0;
                    await botClient.SendTextMessageAsync(-731948319, textglobal);
                    textglobal = "";
                    return;
                }


            }
            catch
            {
                Console.WriteLine("Ошибка приложения");
            }
        }
    }
}