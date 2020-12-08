﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using TaskMaster;
using Telegram.Bot.Types.ReplyMarkups;
using TaskMaster.Domain;
using System.Reflection;

namespace telBot
{
    public enum State
    {
        Nothing,
        CreateNewTask,
        ShowTask,
        EditTask,
        ChangeStatus
    }

    public enum ListTask
    {
        Owned,
        Taken,
        Done
    }

    class telegramTaskBot
    {
        private static Dictionary<long, State> usersState = new Dictionary<long, State>();
        private static Dictionary<long, ITask> usersTask = new Dictionary<long, ITask>();
        private static Dictionary<long, ListTask> listTasks = new Dictionary<long, ListTask>();

        static void Main()
        {
            var token = ""; //вставь токен
            var bot = new TelegramBotClient(token);
            bot.OnMessage += (sender, args) => RecieveMessage(args, bot);
            bot.OnCallbackQuery += (sender, args) => RecieveKeyButton(args, bot);
            bot.StartReceiving();
            Console.ReadKey();
            bot.StartReceiving();
        }

        private static async void RecieveKeyButton(CallbackQueryEventArgs args, TelegramBotClient bot)
        {
            var data = args.CallbackQuery.Data;
            var id = args.CallbackQuery.Message.Chat.Id;
            if (usersState[id] == State.ShowTask)
            {
                /* Пока так [для работоспособности] надо переписать */
                var list = args.CallbackQuery.Data;
                var numberId = 0;
                if (list == "owned")
                    numberId = 0;
                else if (list == "taken")
                    numberId = 1;
                else if (list == "done")
                    numberId = 2;
                listTasks.Add(id, (ListTask)numberId);
                var message = TaskMasters.GetTask(id, listTasks[id], numberId).Topic;
                await bot.SendTextMessageAsync(id, message);
                usersState.Remove(id);
            }

            else if (usersState[id] == State.EditTask)
            {
                var list = args.CallbackQuery.Data;
                var numberId = 0;
                if (list == "owned")
                    numberId = 0;
                else if (list == "taken")
                    numberId = 1;
                else if (list == "done")
                    numberId = 2;
                listTasks.Add(id, (ListTask)numberId);
                EditTask(args, bot, TaskMasters.GetTask(id, listTasks[id], numberId));
                usersState.Remove(id);
            }

            else if (usersState[id] == State.ChangeStatus)
            {
                //над названием подумать не id!
                var numberTask = Convert.ToInt32(args.CallbackQuery.Data);
                usersTask.Add(id, TaskMasters.GetTask(id, listTasks[id], numberTask));
                ChangeState(args, bot, id, usersTask[id].Topic);
                usersState.Remove(id);
            }

            else if (data == "Выполнено")
            {
                if (TaskMasters.TryPerformTask(usersTask[id], id))
                    await bot.SendTextMessageAsync(id, "Задача выполнена!");
                else
                    await bot.SendTextMessageAsync(id, "Задача не может быть выполнена!");
                usersTask.Remove(id);
                listTasks.Remove(id);
            }

            else if (data == "Взять себе")
            {
                if (TaskMasters.TryTakeTask(usersTask[id], id))
                    await bot.SendTextMessageAsync(id, "Задача присвоена");
                else
                    await bot.SendTextMessageAsync(id, "Что-то пошло не так");
                usersTask.Remove(id);
                listTasks.Remove(id);
            }
            else if (data == "Удалить")
            {
                TaskMasters.DeleteTask(usersTask[id], id, listTasks[id]);
                await bot.SendTextMessageAsync(id, "Задача удалена из вашего списка!");
                usersTask.Remove(id);
                listTasks.Remove(id);
            }

            else if (data == "taken")
            {
                listTasks.Add(id, ListTask.Owned);
                var tasks = TaskMasters.GetTakenTasks(id);
                ShowYourTask(bot, id, tasks);
            }
            else if (data == "owned")
            {
                listTasks.Add(id, ListTask.Owned);
                var tasks = TaskMasters.GetOwnedTasks(id);
                ShowYourTask(bot, id, tasks);
            }
            else if (data == "done")
            {
                listTasks.Add(id, ListTask.Done);
                var tasks = TaskMasters.GetDoneTasks(id);
                ShowYourTask(bot, id, tasks);
            }

            await bot.AnswerCallbackQueryAsync(args.CallbackQuery.Id);

            //else
            //    try
            //    {
            //        var task = users[id].OwnedTasks[Convert.ToInt32(args.CallbackQuery.Data)];
            //        if (change_status)
            //            ChangeState(args, bot);
            //        if (is_edit)
            //            EditTask(args, bot, task);
            //        foreach (var options in typeof(ITask).GetMethods())
            //            if (data == options.Name)
            //                options.Invoke(task, str);
            //        надо как то передать ему задачу и на что хочет поменять
            //    }
            //    catch
            //    {
            //        Console.WriteLine("smth is wrong");
            //    }

        }

        private static async void ChangeState(CallbackQueryEventArgs args, TelegramBotClient bot, long id, string taskName)
        {
            var listButtons = new List<InlineKeyboardButton>();
            listButtons.Add(InlineKeyboardButton.WithCallbackData("Удалить"));
            listButtons.Add(InlineKeyboardButton.WithCallbackData("Выполнено"));
            listButtons.Add(InlineKeyboardButton.WithCallbackData("Взять себе"));
            var keyboard = new InlineKeyboardMarkup(listButtons.ToArray());
            await bot.SendTextMessageAsync(id, "что сделать с задачей " + taskName, replyMarkup: keyboard);
        }

        private static async void ShowYourTask(TelegramBotClient bot, long id, List<ITask> tasks)
        {
            var listTasks = new List<InlineKeyboardButton>();
            foreach (var task in tasks)
                listTasks.Add(InlineKeyboardButton.WithCallbackData(task.Topic, task.Id.ToString()));
            var keyboard = new InlineKeyboardMarkup(listTasks.ToArray());
            await bot.SendTextMessageAsync(id, "список ваших задач", replyMarkup: keyboard);
        }

        private static async void EditTask(CallbackQueryEventArgs args, TelegramBotClient bot, ITask task)
        {
            var tasksOptions = new List<InlineKeyboardButton>();
            foreach (var options in typeof(ITask).GetMethods())
                tasksOptions.Add(InlineKeyboardButton.WithCallbackData(options.Name));

            var keyboard = new InlineKeyboardMarkup(tasksOptions.ToArray());
            await bot.SendTextMessageAsync(args.CallbackQuery.Message.Chat.Id, "Что сделать?", replyMarkup: keyboard);
        }

        private static async void WhatTask(MessageEventArgs args, TelegramBotClient bot)
        {
            var butons = new List<InlineKeyboardButton>();
            butons.Add(InlineKeyboardButton.WithCallbackData("owned"));
            butons.Add(InlineKeyboardButton.WithCallbackData("taken"));
            butons.Add(InlineKeyboardButton.WithCallbackData("done"));
            var keyboard = new InlineKeyboardMarkup(butons.ToArray());
            await bot.SendTextMessageAsync(args.Message.Chat.Id, "Выберите список", replyMarkup: keyboard);
        }


        private static async void RecieveMessage(MessageEventArgs args, TelegramBotClient bot)
        {
            var id = args.Message.Chat.Id;

            /* users должен быть получен из DataBase, если его там нет по id — добавить */
            if (!(TaskMasters.users.ContainsKey(id)))
            {
                var name = args.Message.Chat.FirstName;
                TaskMasters.users.Add(id, new Person(id, name));
            }
            Console.WriteLine(id);

            string message = "Введите команду";

            if (args.Message.Type is MessageType.Sticker)
                message = "Кто-то любит стикеры";

            if (args.Message.Type is MessageType.Text)
                switch (args.Message.Text)
                {
                    case "new task":
                        {
                            if(!usersState.ContainsKey(id))
                                usersState.Add(id, State.CreateNewTask);
                            await bot.SendTextMessageAsync(id, "придумай название задачи");
                            break;
                        }
                    case "edit task":
                        {
                            if (!usersState.ContainsKey(id))
                                usersState.Add(id, State.EditTask);
                            WhatTask(args, bot);
                            break;
                        }

                    case "show tasks":
                        {
                            if (!usersState.ContainsKey(id))
                                usersState.Add(id, State.ShowTask);
                            WhatTask(args, bot);
                            break;
                        }
                    case "delete/done tasks":
                        {
                            if (!usersState.ContainsKey(id))
                                usersState.Add(id, State.ChangeStatus);
                            WhatTask(args, bot);
                            break;
                        }
                    case "/start":
                        await MakeStartKeyboard(bot, id);
                        break;
                    case "/start@TaskssMasterBot":
                        await MakeStartKeyboard(bot, id);
                        break;

                    default:
                        if (usersState[id] == State.CreateNewTask)
                        {
                            TaskMasters.CreateSimpleTask(id, args.Message.Text);
                            await bot.SendTextMessageAsync(id, "Задача добавлена");
                            usersState.Remove(id);
                        }
                        else
                            await bot.SendTextMessageAsync(id, message);
                        break;
                };
        }

        private static async Task MakeStartKeyboard(TelegramBotClient bot, long id)
        {
            var keyboard = new ReplyKeyboardMarkup()
            {
                Keyboard = new[] {
                                                new[] // row 1
                                                {
                                                    new KeyboardButton("new task"),
                                                    new KeyboardButton("edit task")
                                                },
                                                new[] // row 2
                                                {
                                                    new KeyboardButton("show tasks"),
                                                    new KeyboardButton("delete/done tasks")
                                                }
                                            },
                ResizeKeyboard = true
            };
            await bot.SendTextMessageAsync(id, "Выбери команду", replyMarkup: keyboard);
        }
    }
}
