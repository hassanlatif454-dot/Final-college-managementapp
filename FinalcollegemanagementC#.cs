using System;
using System.IO;
using System.Threading;

class CollegeManagementApp
{
    // File constants
    const string USERS_FILE = "users.txt";
    const string TEACHERS_FILE = "teachers.txt";
    const string STUDENTS_FILE = "students.txt";

    // Global variables for users
    static string[] usernames = new string[1000];
    static string[] passwords = new string[1000];
    static string[] roles = new string[1000]; // "admin", "teacher", "student"
    static int userCount = 0;

    // Global variables for teachers
    static string[] teacherNames = new string[1000];
    static string[] teacherDepartments = new string[1000];
    static int teacherCount = 0;

    // Global variables for students
    static string[] studentNames = new string[1000];
    static string[] studentRollNumbers = new string[1000];
    static string[] studentDepartments = new string[1000];
    static int studentCount = 0;

    static string inputUsername = "";
    static string inputPassword = "";

    static void SafeClear()
    {
        try { Console.Clear(); }
        catch { }
    }

    // Safe Console.Write()
    static void SafeWrite(string text)
    {
        try { Console.Write(text); }
        catch { }
    }

    // Safe Console.WriteLine() 
    static void SafeWriteLine(string text = "")
    {
        try { Console.WriteLine(text); }
        catch { }
    }

    // Safe Console.ReadLine() 
    static string SafeReadLine()
    {
        try
        {
            string line = Console.ReadLine();
            return (line == null) ? "" : line.Trim();
        }
        catch { return ""; }
    }

    // Safe Console.ReadKey() 
    static void SafeReadKey()
    {
        try { Console.ReadKey(true); }
        catch {  }
    }

    // Safe Thread.Sleep() 
    static void SafeSleep(int ms)
    {
        try { Thread.Sleep(ms); }
        catch (ThreadInterruptedException) { Thread.CurrentThread.Interrupt(); }
        catch { }
    }

    // Safe array-element read 
    static string SafeGet(string[] arr, int index)
    {
        try
        {
            if (arr == null || index < 0 || index >= arr.Length) return "";
            return arr[index] ?? "";
        }
        catch { return ""; }
    }

    // ══════════════════════════════════════════════════════
    //  MAIN
    // ══════════════════════════════════════════════════════
    static void Main(string[] args)
    {
        try
        {
            // Initialise all arrays to empty strings so no slot is ever null
            for (int i = 0; i < 1000; i++)
            {
                usernames[i] = ""; passwords[i] = ""; roles[i] = "";
                teacherNames[i] = ""; teacherDepartments[i] = "";
                studentNames[i] = ""; studentRollNumbers[i] = ""; studentDepartments[i] = "";
            }

            LoadData();

            while (true)
            {
                try
                {
                    SafeClear();
                    SafeWriteLine("=====================================");
                    SafeWriteLine("   College Management System");
                    SafeWriteLine("=====================================");
                    SafeWriteLine("                                     ");
                    SafeWriteLine("1. Sign Up");
                    SafeWriteLine("2. Login");
                    SafeWriteLine("3. Exit");
                    SafeWrite("Enter your choice: ");

                    string choice = SafeReadLine();

                    if (choice == "1")
                    {
                        SignUp();
                    }
                    else if (choice == "2")
                    {
                        Login();
                    }
                    else if (choice == "3")
                    {
                        SafeWriteLine("Exiting..");
                        break;
                    }
                    else
                    {
                        SafeWriteLine("Invalid choice! Please try again.");
                        SafeReadKey();
                    }
                }
                catch (Exception ex)
                {
                    // Inner loop crash guard: show error and continue instead of exiting
                    try { SafeWriteLine("\n[Error] " + ex.Message + " — returning to main menu."); } catch { }
                    SafeSleep(1500);
                }
            }
        }
        catch (Exception ex)
        {
            // Absolute last resort — should never be reached
            try { Console.WriteLine("\n[Fatal] " + ex.Message); } catch { }
        }
    }

    // ══════════════════════════════════════════════════════
    //  SIGN UP
    // ══════════════════════════════════════════════════════
    static void SignUp()
    {
        try
        {
            SafeClear();
            SafeWriteLine("=====================================");
            SafeWriteLine("          SIGN UP MENU");
            SafeWriteLine("=====================================");
            SafeWriteLine();

            if (IsSystemFull())
            {
                SafeWriteLine("System is full! Cannot register more users.");
                SafeWrite("\nPress any key to return to main menu...");
                SafeReadKey();
                return;
            }

            // -- Username --
            SafeWrite("Enter username: ");
            string newUsername = SafeReadLine();

            if (!IsValidUsername(newUsername))
            {
                SafeWriteLine("\nUsername is not Valid! ");
                SafeWriteLine("Username must contain at least one letter.");
                SafeWrite("\nPress any key to return to main menu...");
                SafeReadKey();
                return;
            }
            else if (IsUsernameTaken(newUsername))
            {
                SafeWriteLine("\nUsername exists! Please try again.");
                SafeWrite("\nPress any key to return to main menu...");
                SafeReadKey();
                return;
            }

            // -- Password --
            SafeWrite("Enter Password: ");
            string newPassword = SafeReadLine();

            // Validate password length (minimum 8 characters)
            if (newPassword.Length < 8)
            {
                SafeWriteLine("\nPassword must be at least 8 characters long!");
                SafeWrite("\nPress any key to return to main menu...");
                SafeReadKey();
                return;
            }

            // -- Role --
            SafeWrite("Enter role (admin/teacher/student): ");
            string newRole = SafeReadLine();

            if (newRole != "admin" && newRole != "teacher" && newRole != "student")
            {
                SafeWriteLine("\nInvalid role! Please enter admin, teacher, or student.");
                SafeWrite("\nPress any key to return to main menu...");
                SafeReadKey();
                return;
            }

            AddUser(newUsername, newPassword, newRole);
            SafeWrite("\nWait for a while...");
            SafeSleep(3000);
            SafeWriteLine("\n\nSign up Successful!");
            SafeWriteLine("=====================================");
            SafeWrite("\nPress any key to return to main menu...");
            SafeReadKey();
        }
        catch (Exception ex)
        {
            SafeWriteLine("\n[Error in SignUp] " + ex.Message);
            SafeWrite("\nPress any key to return to main menu...");
            SafeReadKey();
        }
    }

    // ══════════════════════════════════════════════════════
    //  LOGIN
    // ══════════════════════════════════════════════════════
    static void Login()
    {
        try
        {
            SafeClear();
            SafeWriteLine("=====================================");
            SafeWriteLine("           LOGIN MENU");
            SafeWriteLine("=====================================");
            SafeWriteLine();

            SafeWrite("Enter username: ");
            inputUsername = SafeReadLine();

            SafeWrite("Enter password: ");
            inputPassword = SafeReadLine();

            SafeWrite("\nLogging in. Please wait...");
            SafeSleep(2000);

            int userIndex = ValidateLogin(inputUsername, inputPassword);

            if (userIndex != -1)
            {
                string role = SafeGet(roles, userIndex);
                SafeWriteLine("\n\nLogin Successful! Welcome " + role + ".");
                SafeWriteLine("=====================================");

                if (role == "admin")
                {
                    SafeWrite("\nPress any key to enter Admin Menu...");
                    SafeReadKey();
                    AdminMenu();
                }
                else if (role == "teacher")
                {
                    SafeWrite("\nPress any key to enter Teacher Menu...");
                    SafeReadKey();
                    TeacherMenu();
                }
                else if (role == "student")
                {
                    SafeWrite("\nPress any key to enter Student Menu...");
                    SafeReadKey();
                    StudentMenu();
                }
                else
                {
                    SafeWriteLine("Unknown role.");
                    SafeWrite("\nPress any key to return to main menu...");
                    SafeReadKey();
                }
            }
            else
            {
                SafeWriteLine("\n\nInvalid Username or Password!");
                SafeWriteLine("=====================================");
                SafeWrite("\nPress any key to return to main menu...");
                SafeReadKey();
            }
        }
        catch (Exception ex)
        {
            SafeWriteLine("\n[Error in Login] " + ex.Message);
            SafeWrite("\nPress any key to return to main menu...");
            SafeReadKey();
        }
    }

    // ══════════════════════════════════════════════════════
    //  ADMIN MENU
    // ══════════════════════════════════════════════════════
    static void AdminMenu()
    {
        while (true)
        {
            try
            {
                SafeClear();
                SafeWriteLine("====================");
                SafeWriteLine("      Admin Menu    ");
                SafeWriteLine("====================");
                SafeWriteLine("                    ");
                SafeWriteLine("1. Add Teacher");
                SafeWriteLine("2. Add Student");
                SafeWriteLine("3. View All Teachers");
                SafeWriteLine("4. View All Students");
                SafeWriteLine("5. Search Teacher");
                SafeWriteLine("6. Search Student");
                SafeWriteLine("7. View Users");
                SafeWriteLine("8. Logout");
                SafeWriteLine("         ");
                SafeWrite("Enter your choice: ");

                string adminChoice = SafeReadLine();

                if (adminChoice == "1")
                {
                    AddTeacher();
                }
                else if (adminChoice == "2")
                {
                    AddStudent();
                }
                else if (adminChoice == "3")
                {
                    SafeWrite("Wait for a while...");
                    SafeSleep(2000);
                    ViewAllTeachers();
                }
                else if (adminChoice == "4")
                {
                    SafeWrite("Wait for a while...");
                    SafeSleep(2000);
                    ViewAllStudents();
                }
                else if (adminChoice == "5")
                {
                    SafeWrite("Searching...");
                    SafeSleep(2000);
                    SearchTeacher();
                }
                else if (adminChoice == "6")
                {
                    SafeWrite("Searching...");
                    SafeSleep(2000);
                    SearchStudent();
                }
                else if (adminChoice == "7")
                {
                    SafeWrite("Wait for a while...");
                    SafeSleep(3000);
                    ViewUsers();
                }
                else if (adminChoice == "8")
                {
                    break;
                }
                else
                {
                    SafeWriteLine("Invalid choice!");
                }
                SafeWrite("Press any key to continue...");
                SafeReadKey();
            }
            catch (Exception ex)
            {
                SafeWriteLine("\n[Error in AdminMenu] " + ex.Message);
                SafeSleep(1500);
            }
        }
    }

    // ══════════════════════════════════════════════════════
    //  TEACHER MENU
    // ══════════════════════════════════════════════════════
    static void TeacherMenu()
    {
        while (true)
        {
            try
            {
                SafeClear();
                SafeWriteLine("====================");
                SafeWriteLine("     Teacher Menu   ");
                SafeWriteLine("====================");
                SafeWriteLine("                    ");
                SafeWriteLine("1. View All Students");
                SafeWriteLine("2. Search Student");
                SafeWriteLine("3. View My Profile");
                SafeWriteLine("4. Logout");
                SafeWrite("Enter your choice: ");

                string teacherChoice = SafeReadLine();

                if (teacherChoice == "1")
                {
                    SafeWrite("Loading...");
                    SafeSleep(2000);
                    ViewAllStudents();
                }
                else if (teacherChoice == "2")
                {
                    SafeWrite("Searching...");
                    SafeSleep(2000);
                    SearchStudent();
                }
                else if (teacherChoice == "3")
                {
                    SafeClear();
                    SafeWriteLine("\n--- My Profile ---");
                    SafeWriteLine("====================");
                    SafeWriteLine("Username: " + inputUsername);
                    SafeWriteLine("Role: Teacher");
                    SafeWriteLine("====================");
                }
                else if (teacherChoice == "4")
                {
                    break;
                }
                else
                {
                    SafeWriteLine("Invalid choice!");
                }
                SafeWrite("\nPress any key to continue...");
                SafeReadKey();
            }
            catch (Exception ex)
            {
                SafeWriteLine("\n[Error in TeacherMenu] " + ex.Message);
                SafeSleep(1500);
            }
        }
    }

    // ══════════════════════════════════════════════════════
    //  STUDENT MENU
    // ══════════════════════════════════════════════════════
    static void StudentMenu()
    {
        while (true)
        {
            try
            {
                SafeClear();
                SafeWriteLine("====================");
                SafeWriteLine("     Student Menu   ");
                SafeWriteLine("====================");
                SafeWriteLine("                    ");
                SafeWriteLine("1. View All Teachers");
                SafeWriteLine("2. View All Students");
                SafeWriteLine("3. View My Profile");
                SafeWriteLine("4. Logout");
                SafeWrite("Enter your choice: ");

                string studentChoice = SafeReadLine();

                if (studentChoice == "1")
                {
                    SafeWrite("Loading...");
                    SafeSleep(2000);
                    ViewAllTeachers();
                }
                else if (studentChoice == "2")
                {
                    SafeWrite("Loading...");
                    SafeSleep(2000);
                    ViewAllStudents();
                }
                else if (studentChoice == "3")
                {
                    SafeClear();
                    SafeWriteLine("\n--- My Profile ---");
                    SafeWriteLine("====================");
                    SafeWriteLine("Username: " + inputUsername);
                    SafeWriteLine("Role: Student");
                    SafeWriteLine("====================");
                }
                else if (studentChoice == "4")
                {
                    break;
                }
                else
                {
                    SafeWriteLine("Invalid choice!");
                }
                SafeWrite("\nPress any key to continue...");
                SafeReadKey();
            }
            catch (Exception ex)
            {
                SafeWriteLine("\n[Error in StudentMenu] " + ex.Message);
                SafeSleep(1500);
            }
        }
    }

    // ══════════════════════════════════════════════════════
    //  ADD TEACHER
    // ══════════════════════════════════════════════════════
    static void AddTeacher()
    {
        try
        {
            SafeClear();
            SafeWriteLine("\n--- Add Teacher ---");

            if (teacherCount >= 1000)
            {
                SafeWriteLine("Teacher list is full!");
                return;
            }

            // -- Name --
            SafeWrite("Enter Teacher Name: ");
            string teacherName = SafeReadLine();

            // Check if input is empty
            if (teacherName == "")
            {
                SafeWriteLine("Teacher name cannot be empty!");
                return;
            }

            // Validate teacher name contains only alphabets and spaces
            if (!IsAlphabetsOnly(teacherName))
            {
                SafeWriteLine("Teacher name must contain only alphabets and spaces!");
                return;
            }

            // -- Department --
            SafeWrite("Enter Department: ");
            string teacherDept = SafeReadLine();

            // Check if input is empty
            if (teacherDept == "")
            {
                SafeWriteLine("Department cannot be empty!");
                return;
            }

            // Validate department contains only alphabets and spaces
            if (!IsAlphabetsOnly(teacherDept))
            {
                SafeWriteLine("Department must contain only alphabets and spaces!");
                return;
            }

            teacherNames[teacherCount] = teacherName;
            teacherDepartments[teacherCount] = teacherDept;
            teacherCount++;
            SaveTeachers();
            SafeWrite("Adding Teacher...");
            SafeSleep(2000);
            SafeWriteLine("\nTeacher added successfully!");
        }
        catch (Exception ex)
        {
            SafeWriteLine("\n[Error in AddTeacher] " + ex.Message);
        }
    }

    // ══════════════════════════════════════════════════════
    //  ADD STUDENT
    // ══════════════════════════════════════════════════════
    static void AddStudent()
    {
        try
        {
            SafeClear();
            SafeWriteLine("\n--- Add Student ---");

            if (studentCount >= 1000)
            {
                SafeWriteLine("Student list is full!");
                return;
            }

            // -- Name --
            SafeWrite("Enter Student Name: ");
            string studentName = SafeReadLine();

            // Check if input is empty
            if (studentName == "")
            {
                SafeWriteLine("Student name cannot be empty!");
                return;
            }

            // Validate student name contains only alphabets and spaces
            if (!IsAlphabetsOnly(studentName))
            {
                SafeWriteLine("Student name must contain only alphabets and spaces!");
                return;
            }

            // -- Roll Number --
            SafeWrite("Enter Roll Number: ");
            string rollNumber = SafeReadLine();

            // Check if input is empty
            if (rollNumber == "")
            {
                SafeWriteLine("Roll number cannot be empty!");
                return;
            }

            // Validate roll number contains only numbers
            if (!IsNumbersOnly(rollNumber))
            {
                SafeWriteLine("Roll number must contain only numbers!");
                return;
            }

            // -- Department --
            SafeWrite("Enter Department: ");
            string studentDept = SafeReadLine();

            // Check if input is empty
            if (studentDept == "")
            {
                SafeWriteLine("Department cannot be empty!");
                return;
            }

            // Validate department contains only alphabets and spaces
            if (!IsAlphabetsOnly(studentDept))
            {
                SafeWriteLine("Department must contain only alphabets and spaces!");
                return;
            }

            studentNames[studentCount] = studentName;
            studentRollNumbers[studentCount] = rollNumber;
            studentDepartments[studentCount] = studentDept;
            studentCount++;
            SaveStudents();
            SafeWrite("Adding Student...");
            SafeSleep(2000);
            SafeWriteLine("\nStudent added successfully!");
        }
        catch (Exception ex)
        {
            SafeWriteLine("\n[Error in AddStudent] " + ex.Message);
        }
    }

    // ══════════════════════════════════════════════════════
    //  VIEW ALL TEACHERS
    // ══════════════════════════════════════════════════════
    static void ViewAllTeachers()
    {
        try
        {
            SafeClear();
            SafeWriteLine("\n=======================");
            SafeWriteLine("      Teacher List     ");
            SafeWriteLine("=======================");

            if (teacherCount == 0)
            {
                SafeWriteLine("No teachers found!");
                return;
            }

            SafeWriteLine("                       ");
            SafeWriteLine("ID\t\tName\t\t\tDepartment");

            for (int i = 0; i < teacherCount && i < 1000; i++)
            {
                SafeWriteLine((i + 1) + "\t\t" + SafeGet(teacherNames, i) + "\t\t" + SafeGet(teacherDepartments, i));
            }
        }
        catch (Exception ex)
        {
            SafeWriteLine("\n[Error in ViewAllTeachers] " + ex.Message);
        }
    }

    // ══════════════════════════════════════════════════════
    //  VIEW ALL STUDENTS
    // ══════════════════════════════════════════════════════
    static void ViewAllStudents()
    {
        try
        {
            SafeClear();
            SafeWriteLine("\n=======================");
            SafeWriteLine("      Student List     ");
            SafeWriteLine("=======================");

            if (studentCount == 0)
            {
                SafeWriteLine("No students found!");
                return;
            }

            SafeWriteLine("                       ");
            SafeWriteLine("ID\t\tName\t\t\tRoll No\t\tDepartment");

            for (int i = 0; i < studentCount && i < 1000; i++)
            {
                SafeWriteLine((i + 1) + "\t\t" + SafeGet(studentNames, i) + "\t\t"
                    + SafeGet(studentRollNumbers, i) + "\t\t" + SafeGet(studentDepartments, i));
            }
        }
        catch (Exception ex)
        {
            SafeWriteLine("\n[Error in ViewAllStudents] " + ex.Message);
        }
    }

    // ══════════════════════════════════════════════════════
    //  VIEW USERS
    // ══════════════════════════════════════════════════════
    static void ViewUsers()
    {
        try
        {
            SafeWriteLine("\n=======================");
            SafeWriteLine("       User List       ");
            SafeWriteLine("=======================");
            SafeWriteLine("Total Users: " + userCount);
            SafeWriteLine("                       ");
            SafeWriteLine("Username\t\tRole");

            for (int i = 0; i < userCount && i < 1000; i++)
            {
                SafeWriteLine(SafeGet(usernames, i) + "\t\t" + SafeGet(roles, i));
            }
        }
        catch (Exception ex)
        {
            SafeWriteLine("\n[Error in ViewUsers] " + ex.Message);
        }
    }

    // ══════════════════════════════════════════════════════
    //  SEARCH TEACHER
    // ══════════════════════════════════════════════════════
    static void SearchTeacher()
    {
        try
        {
            SafeClear();
            SafeWriteLine("\n--- Search Teacher ---");

            SafeWrite("Enter Teacher Name: ");
            string searchName = SafeReadLine();

            // Check if input is empty
            if (searchName == "")
            {
                SafeWriteLine("Search name cannot be empty!");
                return;
            }

            bool found = false;

            for (int i = 0; i < teacherCount && i < 1000; i++)
            {
                if (SafeGet(teacherNames, i) == searchName)
                {
                    SafeWriteLine("\nTeacher Found!");
                    SafeWriteLine("Name: " + SafeGet(teacherNames, i));
                    SafeWriteLine("Department: " + SafeGet(teacherDepartments, i));
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                SafeWriteLine("\nTeacher not found!");
            }
        }
        catch (Exception ex)
        {
            SafeWriteLine("\n[Error in SearchTeacher] " + ex.Message);
        }
    }

    // ══════════════════════════════════════════════════════
    //  SEARCH STUDENT
    // ══════════════════════════════════════════════════════
    static void SearchStudent()
    {
        try
        {
            SafeClear();
            SafeWriteLine("\n--- Search Student ---");

            SafeWrite("Enter Roll Number: ");
            string searchRoll = SafeReadLine();

            // Check if input is empty
            if (searchRoll == "")
            {
                SafeWriteLine("Roll number cannot be empty!");
                return;
            }

            bool found = false;

            for (int i = 0; i < studentCount && i < 1000; i++)
            {
                if (SafeGet(studentRollNumbers, i) == searchRoll)
                {
                    SafeWriteLine("\nStudent Found!");
                    SafeWriteLine("Name: " + SafeGet(studentNames, i));
                    SafeWriteLine("Roll Number: " + SafeGet(studentRollNumbers, i));
                    SafeWriteLine("Department: " + SafeGet(studentDepartments, i));
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                SafeWriteLine("\nStudent not found!");
            }
        }
        catch (Exception ex)
        {
            SafeWriteLine("\n[Error in SearchStudent] " + ex.Message);
        }
    }

    // ══════════════════════════════════════════════════════
    //  VALIDATION HELPERS
    // ══════════════════════════════════════════════════════

    // Check if system is full
    static bool IsSystemFull()
    {
        try { return userCount >= 1000; }
        catch { return true; } 
    }

    // Validate username (letters, numbers, and special characters allowed)
    static bool IsValidUsername(string username)
    {
        try
        {
            if (username == null || username == "")
                return false;

            bool hasLetter = false;
            for (int i = 0; i < username.Length; i++)
            {
                char ch = username[i];
                if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))
                {
                    hasLetter = true;
                }
                else if (ch >= '0' && ch <= '9')
                {
                    // Numbers from 0-9 are also allowed
                }
                else
                {
                    // Special characters are also allowed
                }
            }
            return hasLetter;
        }
        catch { return false; }
    }

    // Validate string contains only alphabets and spaces
    static bool IsAlphabetsOnly(string str)
    {
        try
        {
            if (str == null || str == "")
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (!((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || ch == ' '))
                {
                    return false;
                }
            }
            return true;
        }
        catch { return false; }
    }

    // Validate string contains only numbers
    static bool IsNumbersOnly(string str)
    {
        try
        {
            if (str == null || str == "")
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (!(ch >= '0' && ch <= '9'))
                {
                    return false;
                }
            }
            return true;
        }
        catch { return false; }
    }

    // Check if username already exists
    static bool IsUsernameTaken(string username)
    {
        try
        {
            for (int i = 0; i < userCount && i < 1000; i++)
            {
                if (SafeGet(usernames, i) == username)
                {
                    return true;
                }
            }
            return false;
        }
        catch { return false; }
    }

    // Add new user
    static void AddUser(string username, string password, string role)
    {
        try
        {
            if (userCount >= 1000) return;
            if (username == null) username = "";
            if (password == null) password = "";
            if (role == null) role = "";

            usernames[userCount] = username;
            passwords[userCount] = password;
            roles[userCount] = role;
            userCount++;
            SaveUsers();
        }
        catch { }
    }

    // Validate login credentials
    static int ValidateLogin(string username, string password)
    {
        try
        {
            if (username == null) username = "";
            if (password == null) password = "";

            for (int i = 0; i < userCount && i < 1000; i++)
            {
                if (SafeGet(usernames, i) == username && SafeGet(passwords, i) == password)
                {
                    return i;
                }
            }
            return -1;
        }
        catch { return -1; }
    }

    // ══════════════════════════════════════════════════════
    //  FILE HANDLING
    // ══════════════════════════════════════════════════════

    static void SaveUsers()
    {
        try
        {
            using (StreamWriter file = new StreamWriter(USERS_FILE))
            {
                file.WriteLine(userCount);
                for (int i = 0; i < userCount && i < 1000; i++)
                {
                    file.WriteLine(SafeGet(usernames, i) + " " + SafeGet(passwords, i) + " " + SafeGet(roles, i));
                }
            }
        }
        catch { /* silently ignore */ }
    }

    static void SaveTeachers()
    {
        try
        {
            using (StreamWriter file = new StreamWriter(TEACHERS_FILE))
            {
                file.WriteLine(teacherCount);
                for (int i = 0; i < teacherCount && i < 1000; i++)
                {
                    file.WriteLine(SafeGet(teacherNames, i));
                    file.WriteLine(SafeGet(teacherDepartments, i));
                }
            }
        }
        catch { }
    }

    static void SaveStudents()
    {
        try
        {
            using (StreamWriter file = new StreamWriter(STUDENTS_FILE))
            {
                file.WriteLine(studentCount);
                for (int i = 0; i < studentCount && i < 1000; i++)
                {
                    file.WriteLine(SafeGet(studentNames, i));
                    file.WriteLine(SafeGet(studentRollNumbers, i));
                    file.WriteLine(SafeGet(studentDepartments, i));
                }
            }
        }
        catch { }
    }

    static void LoadData()
    {
        // -- Load Users --
        try
        {
            if (File.Exists(USERS_FILE))
            {
                using (StreamReader userFile = new StreamReader(USERS_FILE))
                {
                    string countLine = userFile.ReadLine();
                    if (int.TryParse(countLine, out int count))
                    {
                        if (count < 0 || count > 1000)
                        {
                            userCount = 0;
                        }
                        else
                        {
                            userCount = 0;
                            for (int i = 0; i < count; i++)
                            {
                                string line = userFile.ReadLine();
                                if (line == null) break;

                                string[] parts = line.Split(' ');
                                if (parts.Length >= 3)
                                {
                                    usernames[userCount] = parts[0] ?? "";
                                    passwords[userCount] = parts[1] ?? "";
                                    roles[userCount] = parts[2] ?? "";
                                    userCount++;
                                }
                                else
                                {
                                    userCount = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch { userCount = 0; }

        // -- Load Teachers --
        try
        {
            if (File.Exists(TEACHERS_FILE))
            {
                using (StreamReader teacherFile = new StreamReader(TEACHERS_FILE))
                {
                    string countLine = teacherFile.ReadLine();
                    if (int.TryParse(countLine, out int count))
                    {
                        if (count < 0 || count > 1000)
                        {
                            teacherCount = 0;
                        }
                        else
                        {
                            teacherCount = 0;
                            for (int i = 0; i < count; i++)
                            {
                                string name = teacherFile.ReadLine();
                                string dept = teacherFile.ReadLine();
                                if (name == null || dept == null) break;
                                teacherNames[teacherCount] = name;
                                teacherDepartments[teacherCount] = dept;
                                teacherCount++;
                            }
                        }
                    }
                }
            }
        }
        catch { teacherCount = 0; }

        // -- Load Students --
        try
        {
            if (File.Exists(STUDENTS_FILE))
            {
                using (StreamReader studentFile = new StreamReader(STUDENTS_FILE))
                {
                    string countLine = studentFile.ReadLine();
                    if (int.TryParse(countLine, out int count))
                    {
                        if (count < 0 || count > 1000)
                        {
                            studentCount = 0;
                        }
                        else
                        {
                            studentCount = 0;
                            for (int i = 0; i < count; i++)
                            {
                                string name = studentFile.ReadLine();
                                string roll = studentFile.ReadLine();
                                string dept = studentFile.ReadLine();
                                if (name == null || roll == null || dept == null) break;
                                studentNames[studentCount] = name;
                                studentRollNumbers[studentCount] = roll;
                                studentDepartments[studentCount] = dept;
                                studentCount++;
                            }
                        }
                    }
                }
            }
        }
        catch { studentCount = 0; }
    }
}