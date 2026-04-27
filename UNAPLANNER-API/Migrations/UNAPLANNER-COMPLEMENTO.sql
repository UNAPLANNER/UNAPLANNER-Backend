-- ============================================================
--  UNAPLANNER - Script Complementario
--  Se ejecuta DESPUÉS de: dotnet ef database update
--  Agrega: Índices | Vistas | SPs | Triggers | Seeds
-- ============================================================

USE UNAPLANNER;
GO

-- ============================================================
-- PART 1: ÍNDICES (19 + 4 compuestos)
-- ============================================================

-- Users
CREATE INDEX IX_User_RoleId     ON Users(RoleId);
CREATE INDEX IX_User_IsStatus   ON Users(IsStatus);

-- Campuses
CREATE INDEX IX_Campus_IsStatus ON Campuses(IsStatus);

-- Careers
CREATE INDEX IX_Career_CampusId ON Careers(CampusId);
CREATE INDEX IX_Career_IsStatus ON Careers(IsStatus);

-- StudyPlans
CREATE INDEX IX_StudyPlan_CareerId  ON StudyPlans(CareerId);
CREATE INDEX IX_StudyPlan_IsStatus  ON StudyPlans(IsStatus);

-- Students
CREATE INDEX IX_Student_UserId      ON Students(UserId);
CREATE INDEX IX_Student_CareerId    ON Students(CareerId);
CREATE INDEX IX_Student_StudyPlanId ON Students(StudyPlanId);
CREATE INDEX IX_Student_IsStatus    ON Students(IsStatus);

-- Courses
CREATE INDEX IX_Course_IsStatus ON Courses(IsStatus);

-- StudyPlanCourses
CREATE INDEX IX_StudyPlanCourse_StudyPlanId ON StudyPlanCourses(StudyPlanId);
CREATE INDEX IX_StudyPlanCourse_CourseId    ON StudyPlanCourses(CourseId);
CREATE INDEX IX_StudyPlanCourse_LevelsTerm  ON StudyPlanCourses(Levels, Term);

-- Requirements
CREATE INDEX IX_Requirement_CourseId         ON Requirements(CourseId);
CREATE INDEX IX_Requirement_RequiredCourseId ON Requirements(RequiredCourseId);

-- StudentProgress  ← tabla más consultada del sistema
CREATE INDEX IX_StudentProgress_StudentId   ON StudentProgress(StudentId);
CREATE INDEX IX_StudentProgress_CourseId    ON StudentProgress(CourseId);
CREATE INDEX IX_StudentProgress_Status      ON StudentProgress(Status);
-- Índice compuesto: malla curricular + promedio  [NEW-3]
CREATE INDEX IX_StudentProgress_StudentStatus
    ON StudentProgress(StudentId, Status)
    INCLUDE (FinalGrade, CourseId);

-- StudentCourseDetails
CREATE INDEX IX_StudentCourseDetail_ProgressId ON StudentCourseDetails(StudentProgressId);

-- Evaluations
CREATE INDEX IX_Evaluation_ProgressId  ON Evaluations(StudentProgressId);
CREATE INDEX IX_Evaluation_Date        ON Evaluations(Date);

-- Calendars
CREATE INDEX IX_Calendar_UserId         ON Calendars(UserId);
CREATE INDEX IX_Calendar_CourseId       ON Calendars(CourseId);
CREATE INDEX IX_Calendar_ActivityDate   ON Calendars(ActivityDate);
-- Índice compuesto: próximas actividades pendientes  [NEW-3]
CREATE INDEX IX_Calendar_UserPending
    ON Calendars(UserId, IsCompleted, ActivityDate)
    INCLUDE (Title, ActivityType, CourseId);

-- Files
CREATE INDEX IX_File_UserId     ON Files(UserId);
CREATE INDEX IX_File_CourseId   ON Files(CourseId);

-- Notes
CREATE INDEX IX_Note_UserId     ON Notes(UserId);
CREATE INDEX IX_Note_CourseId   ON Notes(CourseId);
-- Índice compuesto para listar notas por curso recientes [NEW-3]
CREATE INDEX IX_Note_UserCourse
    ON Notes(UserId, CourseId)
    INCLUDE (Title, LastUpdated);

-- CampusContacts
CREATE INDEX IX_CampusContact_CampusId  ON CampusContacts(CampusId);
CREATE INDEX IX_CampusContact_IsStatus  ON CampusContacts(IsStatus);

-- NotificationTokens
CREATE INDEX IX_NotificationToken_UserId    ON NotificationTokens(UserId);
CREATE INDEX IX_NotificationToken_IsActive  ON NotificationTokens(IsActive);

-- Notifications
CREATE INDEX IX_Notification_UserId     ON Notifications(UserId);
CREATE INDEX IX_Notification_IsRead     ON Notifications(IsRead);
CREATE INDEX IX_Notification_CreatedDate ON Notifications(CreatedDate DESC);
-- Índice compuesto: notificaciones no leídas  [NEW-3]
CREATE INDEX IX_Notification_UserUnread
    ON Notifications(UserId, IsRead)
    INCLUDE (Title, Message, Type, CreatedDate);

-- AdminLogs
CREATE INDEX IX_AdminLog_UserId     ON AdminLogs(UserId);
CREATE INDEX IX_AdminLog_Entity     ON AdminLogs(Entity, EntityId);
CREATE INDEX IX_AdminLog_Date       ON AdminLogs(CreatedDate DESC);

GO

-- ============================================================
-- PART 2: SEED DATA (INSERTS)
-- ============================================================

-- Roles
INSERT INTO Roles (TypeRole) VALUES ('Estudiante'), ('Administrador');

-- Campuses
INSERT INTO Campuses (Name, Code) VALUES
    ('Campus Sarapiquí', 'SAR'),
    ('Campus Liberia',   'LIB'),
    ('Campus Nicoya',    'NIC'),
    ('Sede Central',     'CEN');

-- Carreras del Campus Sarapiquí (Id = 1)
INSERT INTO Careers (CampusId, Name, Code, Description, TotalCredits) VALUES
    (1, 'Ingeniería en Sistemas de Información',   'ISI', 'Formación en desarrollo de software, bases de datos y gestión de proyectos', 140),
    (1, 'Ingeniería en Ciencia de Datos',          'ICD', 'Formación en análisis de datos, machine learning y big data',                140),
    (1, 'Administración',                          'ADM', 'Formación en gestión empresarial y administración de organizaciones',         138),
    (1, 'Administración de Oficinas',              'ADO', 'Formación en gestión administrativa y tecnologías de oficina',                136),
    (1, 'Educación Comercial',                     'ECO', 'Formación en enseñanza comercial y gestión educativa',                       130),
    (1, 'Comercio y Negocios Internacionales',     'CNI', 'Formación en comercio exterior y negocios globales',                         130),
    (1, 'Inglés',                                  'ING', 'Formación en lengua inglesa y literatura',                                   130);

-- Plan de Estudio – ISI (CareerId = 1)
INSERT INTO StudyPlans (CareerId, Name, Code, ValidYear) VALUES
    (1, 'Bachillerato en Ingeniería en Sistemas de Información', 'BA-ISI-2012', 2012);

-- ── CURSOS ───────────────────────────────────────────────────
-- Nivel 1 Ciclo 1  (I Ciclo)
INSERT INTO Courses (Code, Name, Credits, TheoryHours, PracticeHours, LabHours) VALUES
    ('EIF200', 'Fundamentos de Informática',    3, 2, 2, 0),
    ('MAT030', 'Matemática para Informática',   4, 3, 2, 0),
    ('LIX410', 'Inglés Integrado I',            4, 3, 3, 1);

-- Nivel 1 Ciclo 2  (II Ciclo)
INSERT INTO Courses (Code, Name, Credits, TheoryHours, PracticeHours, LabHours) VALUES
    ('EIF201', 'Programación I',                4, 2, 0, 2),
    ('MAT002', 'Cálculo I',                     4, 3, 2, 0),
    ('LIX411', 'Inglés Integrado II',           4, 3, 3, 1);

-- Nivel 2 Ciclo 1  (III Ciclo)
INSERT INTO Courses (Code, Name, Credits, TheoryHours, PracticeHours, LabHours) VALUES
    ('EIF202', 'Soporte Técnico',                                   3, 2, 1, 0),
    ('EIF203', 'Estructuras Discretas para Informática',            3, 3, 0, 0),
    ('EIF204', 'Programación II',                                   4, 2, 0, 2),
    ('MAT005', 'Álgebra Lineal',                                    4, 3, 2, 0),
    ('LIX412', 'Inglés Integrado III',                              4, 3, 3, 1);

-- Nivel 2 Ciclo 2  (IV Ciclo)
INSERT INTO Courses (Code, Name, Credits, TheoryHours, PracticeHours, LabHours) VALUES
    ('EIF205', 'Arquitectura de Computadoras',                      3, 2, 1, 0),
    ('EIF206', 'Programación III',                                  4, 2, 0, 2),
    ('EIF207', 'Estructuras de Datos',                              4, 2, 0, 2),
    ('EIF404', 'La Organización y su Entorno',                      3, 3, 0, 0),
    ('MAT006', 'Probabilidad y Estadística para Informática',       3, 3, 0, 0);

-- Nivel 3 Ciclo 1  (V Ciclo)
INSERT INTO Courses (Code, Name, Credits, TheoryHours, PracticeHours, LabHours) VALUES
    ('EIF208', 'Comunicaciones y Redes de Computadores',            3, 2, 1, 0),
    ('EIF209', 'Programación IV',                                   4, 2, 0, 2),
    ('EIF210', 'Ingeniería de Sistemas I',                          4, 3, 1, 0),
    ('EIF211', 'Diseño e Implementación de Bases de Datos',         4, 2, 0, 2),
    ('EIF212', 'Sistemas Operativos',                               3, 2, 1, 0);

-- Nivel 3 Ciclo 2  (VI Ciclo)
INSERT INTO Courses (Code, Name, Credits, TheoryHours, PracticeHours, LabHours) VALUES
    ('EIF400', 'Paradigmas de Programación',                        4, 2, 0, 2),
    ('EIF401', 'Ingeniería de Sistemas II',                         4, 3, 1, 0),
    ('EIF402', 'Administración de Bases de Datos',                  4, 2, 0, 2),
    ('EIF407', 'Liderazgo y Organización',                          3, 3, 0, 0),
    ('EIF412', 'Investigación de Operaciones y sus Aplicaciones',   3, 3, 0, 0);

-- Nivel 4 Ciclo 1  (VII Ciclo)
INSERT INTO Courses (Code, Name, Credits, TheoryHours, PracticeHours, LabHours) VALUES
    ('EIF406', 'Ingeniería de Sistemas III',                            4, 3, 1, 0),
    ('EIF411', 'Diseño y Programación de Plataformas Móviles',          4, 2, 0, 2),
    ('EIF413', 'Métodos de Investigación Científica en Informática',    3, 3, 0, 0);

-- Nivel 4 Ciclo 2  (VIII Ciclo)
INSERT INTO Courses (Code, Name, Credits, TheoryHours, PracticeHours, LabHours) VALUES
    ('EIF408', 'Proyectos y su Aplicación en la Organización (PPS)',    5, 2, 3, 0),
    ('EIF409', 'Aplicaciones Informáticas Globales',                    4, 2, 2, 0),
    ('EIF410', 'Informática y Sociedad',                                2, 2, 0, 0);

-- Cursos Optativos
INSERT INTO Courses (Code, Name, Credits, TheoryHours, PracticeHours, LabHours) VALUES
    ('EIF1000', 'Dispositivos para Comunicaciones de Datos',                                    3, 2, 1, 0),
    ('EIF4200', 'Inteligencia Artificial',                                                      3, 2, 1, 0),
    ('EIF4210', 'Análisis de Algoritmos',                                                       3, 2, 1, 0),
    ('EIF4220', 'Diseño de Interfaces de Usuario',                                              3, 2, 1, 0),
    ('EIF4230', 'Introducción a la Programación de Dispositivos Móviles',                       3, 2, 1, 0),
    ('EIF4240', 'Desarrollo de Aplicaciones Educativas',                                        3, 2, 1, 0),
    ('EIF4250', 'Diseño de Ambientes Multimediales',                                            3, 2, 1, 0),
    ('EIF4260', 'Diseño de Ambientes de Aprendizaje',                                           3, 2, 1, 0),
    ('EIF4270', 'Robótica',                                                                     3, 2, 1, 0),
    ('EIF4280', 'Fundamentos de Programación Web',                                              3, 2, 1, 0),
    ('EIF4290', 'Introducción a la Creación de Empresas',                                       3, 3, 0, 0),
    ('EIF4300', 'Las TIC''s en el Ámbito Jurídico de Costa Rica',                               3, 3, 0, 0),
    ('EIF4310', 'Administración de Servidores Basados en Software Libre',                       3, 2, 1, 0),
    ('EIF4320', 'Introducción a la Programación Paralela en Arquitecturas Multinúcleo',         3, 2, 1, 0),
    ('EIF4330', 'Contexto, Desarrollo y Aplicación de Software Libre y de Código Abierto',      3, 2, 1, 0),
    ('EIF4340', 'Minería de Datos I',                                                           3, 2, 1, 0),
    ('EIF4350', 'Minería de Datos II',                                                          3, 2, 1, 0),
    ('EIF4360', 'Gráficos por Computadora',                                                     3, 2, 1, 0),
    ('EIF4370', 'Propiedad Intelectual y su Aplicación Práctica',                               3, 3, 0, 0),
    ('EIF4380', 'Gobierno Electrónico y su Regulación en Costa Rica',                           3, 3, 0, 0),
    ('EIF4390', 'Uso Estratégico del Comercio Electrónico',                                     3, 3, 0, 0),
    ('EIF4400', 'Seguridad Informática',                                                        3, 2, 1, 0),
    ('EIF4410', 'Soporte Técnico Avanzado',                                                     3, 2, 1, 0),
    ('EIF4420', 'Entornos de Programación para Videojuegos',                                    3, 2, 1, 0),
    ('EIF4430', 'Diseño y Programación de Videojuegos en 3D',                                   3, 2, 1, 0),
    ('EIF4440', 'Introducción a la Innovación Tecnológica',                                     3, 3, 0, 0),
    ('EIF1010', 'Redes Avanzadas I',                                                            3, 2, 1, 0),
    ('EIF1020', 'Introducción al Análisis de Datos para Otras Carreras',                        3, 2, 1, 0),
    ('EIG4160', 'Gestión de Tecnología Educativa',                                              3, 2, 1, 0),
    ('EIG4170', 'Liderazgo y Organizaciones',                                                   3, 3, 0, 0),
    ('EIG4180', 'La Organización y su Entorno',                                                 3, 3, 0, 0);

-- ── STUDY PLAN COURSES ────────────────────────────────────────
INSERT INTO StudyPlanCourses (StudyPlanId, CourseId, Levels, Term, IsElective)
SELECT 1, Id, 1, 1, 0 FROM Courses WHERE Code IN ('EIF200','MAT030','LIX410');

INSERT INTO StudyPlanCourses (StudyPlanId, CourseId, Levels, Term, IsElective)
SELECT 1, Id, 1, 2, 0 FROM Courses WHERE Code IN ('EIF201','MAT002','LIX411');

INSERT INTO StudyPlanCourses (StudyPlanId, CourseId, Levels, Term, IsElective)
SELECT 1, Id, 2, 1, 0 FROM Courses WHERE Code IN ('EIF202','EIF203','EIF204','MAT005','LIX412');

INSERT INTO StudyPlanCourses (StudyPlanId, CourseId, Levels, Term, IsElective)
SELECT 1, Id, 2, 2, 0 FROM Courses WHERE Code IN ('EIF205','EIF206','EIF207','EIF404','MAT006');

INSERT INTO StudyPlanCourses (StudyPlanId, CourseId, Levels, Term, IsElective)
SELECT 1, Id, 3, 1, 0 FROM Courses WHERE Code IN ('EIF208','EIF209','EIF210','EIF211','EIF212');

INSERT INTO StudyPlanCourses (StudyPlanId, CourseId, Levels, Term, IsElective)
SELECT 1, Id, 3, 2, 0 FROM Courses WHERE Code IN ('EIF400','EIF401','EIF402','EIF407','EIF412');

INSERT INTO StudyPlanCourses (StudyPlanId, CourseId, Levels, Term, IsElective)
SELECT 1, Id, 4, 1, 0 FROM Courses WHERE Code IN ('EIF406','EIF411','EIF413');

INSERT INTO StudyPlanCourses (StudyPlanId, CourseId, Levels, Term, IsElective)
SELECT 1, Id, 4, 2, 0 FROM Courses WHERE Code IN ('EIF408','EIF409','EIF410');

INSERT INTO StudyPlanCourses (StudyPlanId, CourseId, Levels, Term, IsElective)
SELECT 1, Id, 5, 1, 1 FROM Courses WHERE Code IN (
    'EIF1000','EIF4200','EIF4210','EIF4220','EIF4230','EIF4240','EIF4250','EIF4260',
    'EIF4270','EIF4280','EIF4290','EIF4300','EIF4310','EIF4320','EIF4330','EIF4340',
    'EIF4350','EIF4360','EIF4370','EIF4380','EIF4390','EIF4400','EIF4410','EIF4420',
    'EIF4430','EIF4440','EIF1010','EIF1020','EIG4160','EIG4170','EIG4180');

-- ── REQUIREMENTS ─────────────────────────────────────────────
INSERT INTO Requirements (CourseId, RequiredCourseId, RequirementType) VALUES
    -- Programación II ← Programación I + Fundamentos
    ((SELECT Id FROM Courses WHERE Code='EIF204'),(SELECT Id FROM Courses WHERE Code='EIF201'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF204'),(SELECT Id FROM Courses WHERE Code='EIF200'),'Prerequisite'),
    -- Programación III ← Programación II + Cálculo I
    ((SELECT Id FROM Courses WHERE Code='EIF206'),(SELECT Id FROM Courses WHERE Code='EIF204'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF206'),(SELECT Id FROM Courses WHERE Code='MAT002'),'Prerequisite'),
    -- Estructuras de Datos ← Estructuras Discretas + Programación II
    ((SELECT Id FROM Courses WHERE Code='EIF207'),(SELECT Id FROM Courses WHERE Code='EIF203'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF207'),(SELECT Id FROM Courses WHERE Code='EIF204'),'Prerequisite'),
    -- Diseño e Impl. BD ← Programación III + Estructuras de Datos
    ((SELECT Id FROM Courses WHERE Code='EIF211'),(SELECT Id FROM Courses WHERE Code='EIF206'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF211'),(SELECT Id FROM Courses WHERE Code='EIF207'),'Prerequisite'),
    -- Sistemas Operativos ← Programación II + Arquitectura
    ((SELECT Id FROM Courses WHERE Code='EIF212'),(SELECT Id FROM Courses WHERE Code='EIF204'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF212'),(SELECT Id FROM Courses WHERE Code='EIF205'),'Prerequisite'),
    -- Programación IV ← Programación III
    ((SELECT Id FROM Courses WHERE Code='EIF209'),(SELECT Id FROM Courses WHERE Code='EIF206'),'Prerequisite'),
    -- Ingeniería de Sistemas I ← Programación III
    ((SELECT Id FROM Courses WHERE Code='EIF210'),(SELECT Id FROM Courses WHERE Code='EIF206'),'Prerequisite'),
    -- Comunicaciones y Redes ← Arquitectura
    ((SELECT Id FROM Courses WHERE Code='EIF208'),(SELECT Id FROM Courses WHERE Code='EIF205'),'Prerequisite'),
    -- Ingeniería de Sistemas II ← IS I
    ((SELECT Id FROM Courses WHERE Code='EIF401'),(SELECT Id FROM Courses WHERE Code='EIF210'),'Prerequisite'),
    -- Administración BD ← SO + Diseño BD
    ((SELECT Id FROM Courses WHERE Code='EIF402'),(SELECT Id FROM Courses WHERE Code='EIF212'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF402'),(SELECT Id FROM Courses WHERE Code='EIF211'),'Prerequisite'),
    -- Liderazgo ← Org. y su Entorno
    ((SELECT Id FROM Courses WHERE Code='EIF407'),(SELECT Id FROM Courses WHERE Code='EIF404'),'Prerequisite'),
    -- Invest. de Operaciones ← Prog III + Álgebra + Probabilidad
    ((SELECT Id FROM Courses WHERE Code='EIF412'),(SELECT Id FROM Courses WHERE Code='EIF206'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF412'),(SELECT Id FROM Courses WHERE Code='MAT005'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF412'),(SELECT Id FROM Courses WHERE Code='MAT006'),'Prerequisite'),
    -- Ingeniería de Sistemas III ← IS II
    ((SELECT Id FROM Courses WHERE Code='EIF406'),(SELECT Id FROM Courses WHERE Code='EIF401'),'Prerequisite'),
    -- Plataformas Móviles ← Prog IV
    ((SELECT Id FROM Courses WHERE Code='EIF411'),(SELECT Id FROM Courses WHERE Code='EIF209'),'Prerequisite'),
    -- Métodos de Investigación ← Probabilidad
    ((SELECT Id FROM Courses WHERE Code='EIF413'),(SELECT Id FROM Courses WHERE Code='MAT006'),'Prerequisite'),
    -- PPS ← Prog IV + IS II + Admin BD
    ((SELECT Id FROM Courses WHERE Code='EIF408'),(SELECT Id FROM Courses WHERE Code='EIF209'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF408'),(SELECT Id FROM Courses WHERE Code='EIF401'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF408'),(SELECT Id FROM Courses WHERE Code='EIF402'),'Prerequisite'),
    -- Aplicaciones Globales ← Prog IV + IS II + Admin BD
    ((SELECT Id FROM Courses WHERE Code='EIF409'),(SELECT Id FROM Courses WHERE Code='EIF209'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF409'),(SELECT Id FROM Courses WHERE Code='EIF401'),'Prerequisite'),
    ((SELECT Id FROM Courses WHERE Code='EIF409'),(SELECT Id FROM Courses WHERE Code='EIF402'),'Prerequisite'),
    -- Informática y Sociedad ← IS II
    ((SELECT Id FROM Courses WHERE Code='EIF410'),(SELECT Id FROM Courses WHERE Code='EIF401'),'Prerequisite');

-- ── CAMPUS CONTACTS ──────────────────────────────────────────
INSERT INTO CampusContacts (CampusId, DepartamentName, Phone, Email, Description) VALUES
    (1, 'Coordinación Informática',   '27646050', 'info.sarapiqui@una.ac.cr',       'Coordinación de carreras de informática'),
    (1, 'Registro',                   '27646051', 'registro.sarapiqui@una.ac.cr',   'Trámites de matrícula y registro académico'),
    (1, 'Biblioteca',                 '27646052', 'biblio.sarapiqui@una.ac.cr',      'Servicio de biblioteca y recursos digitales'),
    (1, 'Bienestar Estudiantil',      '27646053', 'bienestar.sarapiqui@una.ac.cr',  'Apoyo psicológico y social al estudiante'),
    (1, 'Administración del Campus',  '27646054', 'admin.sarapiqui@una.ac.cr',       'Administración general del campus'),
    (1, 'Seguridad Universitaria',    '27646055', 'seguridad.sarapiqui@una.ac.cr',  'Servicio de seguridad del campus');

-- ── TEST USERS ────────────────────────────────────────────────
INSERT INTO Users (RoleId, Email, Password) VALUES
    (1, 'estudiante.prueba@una.ac.cr', '$2y$12$/10iUHJSvyQ9sGRs6FSe1OO7iKHs0RWw8HVu1eXhGFx3UKjafwh/S'),
    (2, 'admin@unaplanner.com',        '$2y$12$/10iUHJSvyQ9sGRs6FSe1OO7iKHs0RWw8HVu1eXhGFx3UKjafwh/S');

INSERT INTO Students (UserId, CareerId, StudyPlanId, FullName, EnterYear) VALUES
    (1, 1, 1, 'Estudiante Prueba', 2024);

-- Progreso del estudiante de prueba
INSERT INTO StudentProgress (StudentId, CourseId, Status, FinalGrade, TermYear, AcademicTerm)
SELECT s.StudentId, c.Id, 'Aprobado', 85.5, 2024, 1
FROM Students s CROSS JOIN Courses c
WHERE s.StudentId = 1 AND c.Code IN ('EIF200','MAT030','LIX410');

INSERT INTO StudentProgress (StudentId, CourseId, Status, TermYear, AcademicTerm)
SELECT s.StudentId, c.Id, 'EnCurso', 2024, 2
FROM Students s CROSS JOIN Courses c
WHERE s.StudentId = 1 AND c.Code = 'EIF201';

INSERT INTO StudentCourseDetails (StudentProgressId, ProfessorName, Classroom, Schedule)
SELECT sp.StudentProgressId, 'Dr. Carlos Soto', 'Aula 3B', 'Lunes y Miércoles 07:00–09:00'
FROM StudentProgress sp
INNER JOIN Courses c ON c.Id = sp.CourseId
WHERE sp.StudentId = 1 AND c.Code = 'EIF201';

-- Evaluaciones de ejemplo
DECLARE @PId INT = (
    SELECT TOP 1 sp.StudentProgressId
    FROM StudentProgress sp
    INNER JOIN Courses c ON c.Id = sp.CourseId
    WHERE sp.StudentId = 1 AND c.Code = 'EIF201'
);
IF @PId IS NOT NULL
BEGIN
    INSERT INTO Evaluations (StudentProgressId, Name, Percentage, Score, Date, EvaluationType) VALUES
        (@PId, 'Examen Parcial I',  25.0, NULL, '2025-03-10', 'Examen'),
        (@PId, 'Examen Parcial II', 25.0, NULL, '2025-05-12', 'Examen'),
        (@PId, 'Proyecto Final',    30.0, NULL, '2025-06-15', 'Proyecto'),
        (@PId, 'Tareas Cortas',     20.0, NULL, NULL,         'Tarea');
END

-- Actividades de calendario de ejemplo
INSERT INTO Calendars (UserId, CourseId, Title, Description, ActivityDate, ActivityType, HasReminder, ReminderDate)
SELECT u.UserId, c.Id,
    'Examen Parcial I - Programación I',
    'Revisar temas: variables, condicionales y ciclos en Kotlin',
    '2025-03-10 07:00:00', 'Examen', 1, '2025-03-09 20:00:00'
FROM Users u CROSS JOIN Courses c
WHERE u.Email = 'estudiante.prueba@una.ac.cr' AND c.Code = 'EIF201';

INSERT INTO Calendars (UserId, CourseId, Title, Description, ActivityDate, ActivityType, HasReminder, ReminderDate)
SELECT u.UserId, c.Id,
    'Entrega Proyecto Final',
    'Subir el proyecto completo al campus virtual',
    '2025-06-15 23:59:00', 'Proyecto', 1, '2025-06-14 08:00:00'
FROM Users u CROSS JOIN Courses c
WHERE u.Email = 'estudiante.prueba@una.ac.cr' AND c.Code = 'EIF201';

-- Notas de ejemplo
INSERT INTO Notes (UserId, CourseId, Title, Content)
SELECT u.UserId, c.Id,
    'Apuntes Clase 1 - Programación I',
    'Conceptos de variables y tipos de datos primitivos en Kotlin. val (inmutable) vs var (mutable).'
FROM Users u CROSS JOIN Courses c
WHERE u.Email = 'estudiante.prueba@una.ac.cr' AND c.Code = 'EIF201';

INSERT INTO Notes (UserId, CourseId, Title, Content)
SELECT u.UserId, c.Id,
    'Estructuras de control',
    'If-else, when (switch en Kotlin), for, while con ejemplos prácticos.'
FROM Users u CROSS JOIN Courses c
WHERE u.Email = 'estudiante.prueba@una.ac.cr' AND c.Code = 'EIF201';

GO

-- ============================================================
-- PART 3: VISTAS
-- ============================================================

-- Vista: Malla curricular completa del estudiante con estado
CREATE VIEW vw_StudentFullCurriculum AS
SELECT
    s.StudentId,
    s.FullName,
    cr.Name                                     AS CareerName,
    c.Code                                      AS CourseCode,
    c.Name                                      AS CourseName,
    c.Credits,
    spc.Levels,
    spc.Term,
    spc.IsElective,
    ISNULL(sp.Status, 'Pendiente')              AS Status,
    sp.FinalGrade,
    sp.TermYear,
    sp.AcademicTerm
FROM Students s
INNER JOIN Careers            cr  ON cr.Id             = s.CareerId
INNER JOIN StudyPlans         pl  ON pl.StudyPlanId     = s.StudyPlanId
INNER JOIN StudyPlanCourses   spc ON spc.StudyPlanId    = pl.StudyPlanId
INNER JOIN Courses            c   ON c.Id               = spc.CourseId
LEFT  JOIN StudentProgress    sp  ON sp.StudentId       = s.StudentId
                                 AND sp.CourseId        = c.Id
WHERE s.IsStatus = 1;
GO

-- Vista: Plan de estudios completo (catálogo)
CREATE VIEW vw_PlanEstudiosCompleto AS
SELECT
    c.Code          AS Codigo,
    c.Name          AS Nombre,
    c.Credits       AS Creditos,
    spc.Levels      AS Nivel,
    spc.Term        AS Ciclo,
    CASE WHEN spc.IsElective = 1 THEN 'Optativo' ELSE 'Obligatorio' END AS Tipo,
    STRING_AGG(req.Code, ', ') AS Requisitos
FROM Courses c
INNER JOIN StudyPlanCourses spc  ON spc.CourseId     = c.Id
LEFT  JOIN Requirements     r    ON r.CourseId        = c.Id AND r.RequirementType = 'Prerequisite'
LEFT  JOIN Courses          req  ON req.Id            = r.RequiredCourseId
WHERE spc.StudyPlanId = 1 AND spc.IsStatus = 1
GROUP BY c.Code, c.Name, c.Credits, spc.Levels, spc.Term, spc.IsElective;
GO

-- Vista: Resumen académico por estudiante
CREATE VIEW vw_ResumenAcademicoEstudiante AS
SELECT
    s.StudentId,
    s.FullName                                                                              AS NombreEstudiante,
    c.Name                                                                                  AS Carrera,
    sp.Name                                                                                 AS PlanEstudio,
    COUNT(DISTINCT spc.CourseId)                                                            AS TotalCursos,
    SUM(CASE WHEN spro.Status = 'Aprobado'  THEN 1 ELSE 0 END)                            AS CursosAprobados,
    SUM(CASE WHEN spro.Status = 'EnCurso'   THEN 1 ELSE 0 END)                            AS CursosEnCurso,
    SUM(CASE WHEN spro.Status = 'Reprobado' THEN 1 ELSE 0 END)                            AS CursosReprobados,
    COUNT(DISTINCT spc.CourseId)
        - SUM(CASE WHEN spro.Status IS NOT NULL THEN 1 ELSE 0 END)                        AS CursosPendientes,
    ROUND(
        CAST(SUM(CASE WHEN spro.Status = 'Aprobado' THEN 1 ELSE 0 END) AS FLOAT)
        / NULLIF(COUNT(DISTINCT spc.CourseId), 0) * 100, 1
    )                                                                                       AS PorcentajeCompletado,
    SUM(CASE WHEN spro.Status = 'Aprobado' THEN co.Credits ELSE 0 END)                    AS CreditosGanados,
    AVG(CASE WHEN spro.Status = 'Aprobado' THEN spro.FinalGrade END)                      AS PromedioGeneral
FROM Students s
INNER JOIN Careers              c    ON c.Id           = s.CareerId
INNER JOIN StudyPlans           sp   ON sp.StudyPlanId  = s.StudyPlanId
INNER JOIN StudyPlanCourses     spc  ON spc.StudyPlanId = sp.StudyPlanId
INNER JOIN Courses              co   ON co.Id           = spc.CourseId
LEFT  JOIN StudentProgress      spro ON spro.StudentId  = s.StudentId AND spro.CourseId = spc.CourseId
WHERE s.IsStatus = 1
GROUP BY s.StudentId, s.FullName, c.Name, sp.Name;
GO

-- Vista: Progreso por nivel y ciclo
CREATE VIEW vw_StudentProgressByLevel AS
SELECT
    s.StudentId,
    s.FullName,
    spc.Levels,
    spc.Term,
    COUNT(DISTINCT spc.CourseId)                                            AS TotalCourses,
    SUM(CASE WHEN sp.Status = 'Aprobado'  THEN 1 ELSE 0 END)              AS Approved,
    SUM(CASE WHEN sp.Status = 'EnCurso'   THEN 1 ELSE 0 END)              AS InProgress,
    SUM(CASE WHEN sp.Status = 'Reprobado' THEN 1 ELSE 0 END)              AS Failed,
    SUM(CASE WHEN sp.Status IS NULL       THEN 1 ELSE 0 END)              AS Pending
FROM Students s
INNER JOIN StudyPlans        spl ON spl.StudyPlanId = s.StudyPlanId
INNER JOIN StudyPlanCourses  spc ON spc.StudyPlanId = spl.StudyPlanId
LEFT  JOIN StudentProgress   sp  ON sp.StudentId    = s.StudentId AND sp.CourseId = spc.CourseId
GROUP BY s.StudentId, s.FullName, spc.Levels, spc.Term;
GO

-- Vista: Próximas actividades (activas, no completadas)
CREATE VIEW vw_UpcomingActivities AS
SELECT
    u.UserId,
    u.Email,
    ac.Id                                               AS ActivityId,
    ac.Title,
    ac.Description,
    ac.ActivityDate,
    ac.ActivityType,
    ac.HasReminder,
    ac.ReminderDate,
    c.Code                                              AS CourseCode,
    c.Name                                              AS CourseName,
    DATEDIFF(DAY, GETDATE(), ac.ActivityDate)           AS DaysRemaining
FROM Calendars  ac
INNER JOIN Users       u  ON u.UserId    = ac.UserId
LEFT  JOIN Courses     c  ON c.Id        = ac.CourseId
WHERE ac.ActivityDate >= GETDATE()
  AND ac.IsCompleted  = 0
  AND u.IsStatus      = 1;
GO

-- Vista: Resumen de evaluaciones por curso
CREATE VIEW vw_CourseEvaluationsSummary AS
SELECT
    sp.StudentId,
    sp.CourseId,
    c.Code,
    c.Name,
    COUNT(e.Id)                                                                     AS TotalEvaluations,
    SUM(e.Percentage)                                                               AS TotalPercentage,
    SUM(CASE WHEN e.Score IS NOT NULL THEN (e.Score * e.Percentage / 100) ELSE 0 END) AS CurrentScore
FROM StudentProgress sp
INNER JOIN Courses       c ON c.Id                   = sp.CourseId
LEFT  JOIN Evaluations   e ON e.StudentProgressId    = sp.StudentProgressId
WHERE sp.Status IN ('EnCurso', 'Pendiente')
GROUP BY sp.StudentId, sp.CourseId, c.Code, c.Name;
GO

-- Vista: Cursos bloqueados por requisitos
CREATE VIEW vw_BlockedCourses AS
SELECT
    s.StudentId,
    s.FullName,
    c.Id        AS CourseId,
    c.Code,
    c.Name,
    STRING_AGG(req.Code + ' (' + req.Name + ')', ' | ') AS MissingPrerequisites
FROM Students s
INNER JOIN StudyPlanCourses  spc ON spc.StudyPlanId  = s.StudyPlanId
INNER JOIN Courses           c   ON c.Id             = spc.CourseId
INNER JOIN Requirements      r   ON r.CourseId       = c.Id AND r.RequirementType = 'Prerequisite'
INNER JOIN Courses           req ON req.Id           = r.RequiredCourseId
WHERE
    -- El requisito NO está aprobado
    NOT EXISTS (
        SELECT 1 FROM StudentProgress sp1
        WHERE sp1.StudentId  = s.StudentId
          AND sp1.CourseId   = r.RequiredCourseId
          AND sp1.Status     = 'Aprobado'
    )
    -- El curso aún no está en progreso o aprobado
    AND NOT EXISTS (
        SELECT 1 FROM StudentProgress sp2
        WHERE sp2.StudentId  = s.StudentId
          AND sp2.CourseId   = c.Id
          AND sp2.Status    IN ('Aprobado', 'EnCurso')
    )
GROUP BY s.StudentId, s.FullName, c.Id, c.Code, c.Name;
GO

-- ============================================================
-- PART 4: STORED PROCEDURES
-- ============================================================

-- SP: Verificar si un estudiante puede matricular un curso
CREATE PROCEDURE sp_PuedeMatricularCurso
    @StudentId              INT,
    @CourseId               INT,
    @PuedeMatricular        BIT OUTPUT,
    @RequisitosIncumplidos  NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @RequisitosIncumplidos = '';

    IF NOT EXISTS (SELECT 1 FROM Requirements WHERE CourseId = @CourseId)
    BEGIN
        SET @PuedeMatricular = 1;
        RETURN;
    END

    SELECT @RequisitosIncumplidos = STRING_AGG(c.Code + ' - ' + c.Name, ', ')
    FROM Requirements r
    INNER JOIN Courses c ON c.Id = r.RequiredCourseId
    WHERE r.CourseId        = @CourseId
      AND r.RequirementType = 'Prerequisite'
      AND r.RequiredCourseId NOT IN (
          SELECT CourseId FROM StudentProgress
          WHERE StudentId = @StudentId AND Status = 'Aprobado'
      );

    SET @PuedeMatricular = CASE WHEN LEN(ISNULL(@RequisitosIncumplidos,'')) = 0 THEN 1 ELSE 0 END;
END;
GO

-- SP: Calcular progreso del estudiante
CREATE PROCEDURE sp_CalcularProgresoEstudiante
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.FullName                                                                              AS NombreEstudiante,
        COUNT(DISTINCT spc.CourseId)                                                            AS TotalCursos,
        SUM(CASE WHEN sp.Status = 'Aprobado'  THEN 1 ELSE 0 END)                              AS Aprobados,
        SUM(CASE WHEN sp.Status = 'EnCurso'   THEN 1 ELSE 0 END)                              AS EnCurso,
        SUM(CASE WHEN sp.Status = 'Reprobado' THEN 1 ELSE 0 END)                              AS Reprobados,
        ROUND(
            CAST(SUM(CASE WHEN sp.Status = 'Aprobado' THEN 1 ELSE 0 END) AS FLOAT)
            / NULLIF(COUNT(DISTINCT spc.CourseId), 0) * 100, 1
        )                                                                                       AS PorcentajeCompletado,
        SUM(CASE WHEN sp.Status = 'Aprobado' THEN c.Credits ELSE 0 END)                       AS CreditosGanados,
        AVG(CASE WHEN sp.Status = 'Aprobado' THEN sp.FinalGrade END)                          AS PromedioGeneral
    FROM Students s
    INNER JOIN StudyPlans       spl ON spl.StudyPlanId  = s.StudyPlanId
    INNER JOIN StudyPlanCourses spc ON spc.StudyPlanId  = spl.StudyPlanId
    INNER JOIN Courses          c   ON c.Id             = spc.CourseId
    LEFT  JOIN StudentProgress sp  ON sp.StudentId     = s.StudentId AND sp.CourseId = c.Id
    WHERE s.StudentId = @StudentId
    GROUP BY s.FullName;
END;
GO

-- SP: Obtener malla curricular completa de un estudiante
CREATE PROCEDURE sp_GetStudentCurriculum
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        spc.Levels,
        spc.Term,
        c.Id            AS CourseId,
        c.Code,
        c.Name,
        c.Credits,
        spc.IsElective,
        ISNULL(sp.Status, 'Pendiente') AS Status,
        sp.FinalGrade,
        -- Está bloqueado si tiene algún requisito no aprobado
        CASE WHEN EXISTS (
            SELECT 1 FROM Requirements r
            WHERE r.CourseId = c.Id AND r.RequirementType = 'Prerequisite'
              AND r.RequiredCourseId NOT IN (
                  SELECT CourseId FROM StudentProgress
                  WHERE StudentId = @StudentId AND Status = 'Aprobado'
              )
        ) THEN 1 ELSE 0 END             AS IsBlocked
    FROM Students s
    INNER JOIN StudyPlans       spl ON spl.StudyPlanId  = s.StudyPlanId
    INNER JOIN StudyPlanCourses spc ON spc.StudyPlanId  = spl.StudyPlanId
    INNER JOIN Courses          c   ON c.Id             = spc.CourseId
    LEFT  JOIN StudentProgress sp  ON sp.StudentId     = s.StudentId AND sp.CourseId = c.Id
    WHERE s.StudentId = @StudentId AND spc.IsStatus = 1
    ORDER BY spc.Levels, spc.Term, spc.IsElective, c.Code;
END;
GO

-- SP: Calcular nota final del curso
CREATE PROCEDURE sp_CalcularNotaFinalCurso
    @StudentProgressId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NotaFinal DECIMAL(5,2);

    SELECT @NotaFinal = SUM((Score * Percentage) / 100)
    FROM Evaluations
    WHERE StudentProgressId = @StudentProgressId AND Score IS NOT NULL;

    UPDATE StudentProgress
    SET FinalGrade  = @NotaFinal,
        Status      = CASE
                          WHEN @NotaFinal >= 70 THEN 'Aprobado'
                          WHEN @NotaFinal IS NOT NULL THEN 'Reprobado'
                          ELSE Status
                      END,
        LastUpdated = GETDATE()
    WHERE StudentProgressId = @StudentProgressId;

    SELECT @NotaFinal AS NotaFinalCalculada;
END;
GO

-- SP: Obtener cursos actuales del estudiante
CREATE PROCEDURE sp_ObtenerCursosActualesEstudiante
    @StudentId  INT,
    @Anio       INT,
    @Ciclo      INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        c.Code              AS Codigo,
        c.Name              AS NombreCurso,
        c.Credits           AS Creditos,
        sp.Status           AS Estado,
        sp.FinalGrade       AS NotaFinal,
        scd.ProfessorName   AS NombreProfesor,
        scd.Classroom       AS Aula,
        scd.Schedule        AS Horario
    FROM StudentProgress        sp
    INNER JOIN Courses          c   ON c.Id                 = sp.CourseId
    LEFT  JOIN StudentCourseDetails scd ON scd.StudentProgressId = sp.StudentProgressId
    WHERE sp.StudentId      = @StudentId
      AND sp.TermYear       = @Anio
      AND sp.AcademicTerm   = @Ciclo
      AND sp.Status        IN ('EnCurso', 'Pendiente')
    ORDER BY c.Code;
END;
GO

-- SP: Obtener próximas actividades del estudiante
CREATE PROCEDURE sp_GetUpcomingActivities
    @UserId     INT,
    @DaysAhead  INT = 7
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ac.Id,
        ac.Title,
        ac.Description,
        ac.ActivityDate,
        ac.ActivityType,
        ac.HasReminder,
        ac.ReminderDate,
        c.Code          AS CourseCode,
        c.Name          AS CourseName,
        DATEDIFF(DAY, GETDATE(), ac.ActivityDate) AS DaysRemaining
    FROM Calendars   ac
    LEFT JOIN Courses        c ON c.Id = ac.CourseId
    WHERE ac.UserId       = @UserId
      AND ac.IsCompleted  = 0
      AND ac.ActivityDate >= GETDATE()
      AND ac.ActivityDate <= DATEADD(DAY, @DaysAhead, GETDATE())
    ORDER BY ac.ActivityDate ASC;
END;
GO

-- SP: Registrar token FCM
CREATE PROCEDURE sp_RegisterNotificationToken
    @UserId     INT,
    @FcmToken   NVARCHAR(255),
    @DeviceName NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    -- DELETE + INSERT en lugar de UPDATE + INSERT
    DELETE FROM NotificationTokens
    WHERE UserId = @UserId AND FcmToken = @FcmToken;

    INSERT INTO NotificationTokens (UserId, FcmToken, DeviceName, IsActive)
    VALUES (@UserId, @FcmToken, @DeviceName, 1);

    SELECT @@ROWCOUNT AS TokenRegistered;
END;
GO

-- SP: Obtener notificaciones no leídas
CREATE PROCEDURE sp_GetUnreadNotifications
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Id, Title, Message, Type, RelatedId, CreatedDate,
        DATEDIFF(MINUTE, CreatedDate, GETDATE()) AS MinutesAgo
    FROM Notifications
    WHERE UserId = @UserId AND IsRead = 0
    ORDER BY CreatedDate DESC;
END;
GO

-- SP: Marcar notificaciones como leídas
CREATE PROCEDURE sp_MarkNotificationsAsRead
    @UserId         INT,
    @NotificationId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @NotificationId IS NULL
        UPDATE Notifications SET IsRead = 1
        WHERE UserId = @UserId AND IsRead = 0;
    ELSE
        UPDATE Notifications SET IsRead = 1
        WHERE Id = @NotificationId AND UserId = @UserId;

    SELECT @@ROWCOUNT AS NotificationsMarked;
END;
GO

-- SP: Generar recordatorios automáticos
CREATE PROCEDURE sp_GenerateActivityReminders
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Notifications (UserId, Title, Message, Type, RelatedId, SentDate)
    SELECT
        ac.UserId,
        ac.Title,
        CASE ac.ActivityType
            WHEN 'Examen'    THEN N'📚 Tienes un examen mañana: '   + ac.Title
            WHEN 'Tarea'     THEN N'✏️ Tarea pendiente: '            + ac.Title
            WHEN 'Proyecto'  THEN N'📊 Entrega de proyecto: '        + ac.Title
            ELSE                  N'📅 Actividad próxima: '           + ac.Title
        END,
        ac.ActivityType,
        ac.Id,
        GETDATE()
    FROM Calendars ac
    WHERE ac.HasReminder  = 1
      AND ac.IsCompleted  = 0
      AND ac.ReminderDate <= GETDATE()
      AND ac.ReminderDate >= DATEADD(HOUR, -1, GETDATE())
      AND NOT EXISTS (
          SELECT 1 FROM Notifications n
          WHERE n.RelatedId = ac.Id AND n.Type = ac.ActivityType
      );

    SELECT @@ROWCOUNT AS RemindersGenerated;
END;
GO

-- ============================================================
-- PART 5: TRIGGERS
-- ============================================================

-- Trigger: Notificación cuando un curso es aprobado
CREATE TRIGGER trg_StudentProgress_Approved
ON StudentProgress
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Notifications (UserId, Title, Message, Type, RelatedId)
    SELECT
        st.UserId,
        N'Curso Aprobado',
        N'¡Felicidades! Aprobaste ' + c.Code + ' - ' + c.Name
            + N' con nota ' + CAST(i.FinalGrade AS NVARCHAR(10)),
        'CourseApproved',
        i.CourseId
    FROM inserted i
    INNER JOIN deleted          d   ON d.StudentProgressId = i.StudentProgressId
    INNER JOIN Courses          c   ON c.Id                = i.CourseId
    INNER JOIN Students         st  ON st.StudentId        = i.StudentId
    WHERE i.Status = 'Aprobado'
      AND d.Status <> 'Aprobado'
      AND i.FinalGrade IS NOT NULL;
END;
GO

-- Trigger: Crear actividad en calendario al insertar evaluación
CREATE TRIGGER trg_Evaluation_CreateCalendarActivity
ON Evaluations
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Calendars (UserId, CourseId, Title, ActivityDate, ActivityType, HasReminder)
    SELECT
        st.UserId,
        sp.CourseId,
        i.Name,
        i.Date,
        i.EvaluationType,
        1
    FROM inserted                   i
    INNER JOIN StudentProgress      sp  ON sp.StudentProgressId = i.StudentProgressId
    INNER JOIN Students             st  ON st.StudentId         = sp.StudentId
    WHERE i.Date IS NOT NULL
      AND NOT EXISTS (
          SELECT 1 FROM Calendars ac
          WHERE ac.UserId   = st.UserId
            AND ac.CourseId = sp.CourseId
            AND ac.Title    = i.Name
      );
END;
GO

-- Trigger: Actualizar timestamp en Note
CREATE TRIGGER trg_Note_UpdateTimestamp
ON Notes
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Notes SET LastUpdated = GETDATE()
    WHERE Id IN (SELECT DISTINCT Id FROM inserted);
END;
GO

-- Trigger: Actualizar timestamp en NotificationToken
CREATE TRIGGER trg_NotificationToken_UpdateTimestamp
ON NotificationTokens
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE NotificationTokens SET LastUpdated = GETDATE()
    WHERE Id IN (SELECT DISTINCT Id FROM inserted);
END;
GO
