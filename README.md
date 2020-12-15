# Task-Master
# Бот в телеграме Планировщик

# Cтуденты-участники:
 	Колосов Дмитрий Станиславович ФТ-201
 	Шрейн Ольга Александровна ФТ-201
 	Пищулов Сергей Сергеевич ФТ-201
	Кряжев Александр Анатольевич ФТ-201

# Проект помогает решить проблему планирования проектов, целей, задач
# Сценарии:
	1.Команда хочет выполнять совместный проект для этого в беседу в телеграм добавляется бот, команда создает задачи, 
		назначает ответственных за выполнение, устанавливает сроки выполнения. 
		Приложение по запросу участника выдает участникам команды их задачи. 
		После выполнения пользователь отмечает задачу выполненной.
		Можно сделать визуализацию задач, создав отчет, который будет предоставлен в виде файла.
	2. Пользователь лично создает новые задачи, следит за их выполнением, редактирует свойства, отмечает задачу выполненной,
		программа позволяет пользователю не забыть о задаче, а так же быстро добавить ее в свой список дел. 
	3.Пользователь хочет достичь какой-то своей личной цели, для этого в приложении создает новую задачу,
		разделяет ее на подзадачи и приоритезирует их. Приложение помогает пользователю структировать информацию,
		не пропустить важные подзадачи. Следить за своим прогрессом.
		Например: пользователь хочет проследить за выполнением цели "купить машину".
		Для этого он вводит в приложение данную задачу, 
		затем разбивает ее на пункты: "выбрать марку", "накопить 1000 долларов", "найти салон, где купить", "купить машину". 
		Ставит дедлайны на каждую подзадачу. 
		Расставляет в последовательность выполнения, например: нельзя купить машину, не выбрав ее марку.
		После достижения подзадач, пользователь отмечает ее выполнение,
		видит свой прогресс и может приступать к следующей подзадаче. (не до конца реализовано)
# Основные компоненты системы:
	Связь между сервером и функциональностью бота - интерфейс
	[TelegramTaskBot] (https://github.com/dimachkaDESTROYER/Task-Master/blob/main/TaskMaster/TelegramTaskBot.cs)
	Класс, описывающие разные виды взаимодействия пользователя с ботом - слой приложения[Task-Master] (https://github.com/dimachkaDESTROYER/Task-Master/blob/main/TaskMaster/TaskMaster.cs)
	Классы, описывающие саму задачу, пользователей, команды - Предметный слой [Domain] (https://github.com/dimachkaDESTROYER/Task-Master/tree/main/TaskMaster/Domain)
	Работа с базами данных - 
	инфраструктура [DataBaseFolder] (https://github.com/dimachkaDESTROYER/Task-Master/tree/main/TaskMaster/DataBaseFolder)
	Класс для визуализации проектов [ExcelReportMaker] (https://github.com/dimachkaDESTROYER/Task-Master/blob/main/TaskMaster/Report/ExcelReportMaker.cs)
# Точки расширения:
	Добавление новых видов задач происходит через наследование от [SimpleTask] (https://github.com/dimachkaDESTROYER/Task-Master/blob/6bafb6977917291c318a1d1a5ebd66bc006fd2c3/TaskMaster/Domain/Tasks/SimpleTask.cs#L6) 
		пример такого наследования - [ветвистая задача] (https://github.com/dimachkaDESTROYER/Task-Master/blob/main/TaskMaster/Domain/Tasks/BranchedTask.cs)
       В классе Task-Master в некоторых методах реализована работа через рефлексию типов, следовательно при добавлении,
	   убавлении новых полей в описании задачи работа этих методов не нарушится, не придется заново их переписывать.
	Добавление новых видов визуализации задач (список, mind map, таблица) происходит через реализацию интерфейса [IReportMaker] (https://github.com/dimachkaDESTROYER/Task-Master/blob/main/TaskMaster/Report/IReportMaker.cs)
	
