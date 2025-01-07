using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BudgetTracker.Models;

public class UserService
{
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static readonly string FolderPath = Path.Combine(DesktopPath, "LocalDB");
    private static readonly string FilePath = Path.Combine(FolderPath, "db.json");
    public string? activeUsername { get; set; }

    public List<BudgetTracker.Models.Transaction> fetchUserTransaction(string username)
    {
        var users = fetchUsers();
        var user = users.FirstOrDefault(a => a.username == username);
        return user?.Transactions ?? new List<BudgetTracker.Models.Transaction>();
    }

    public void clearTransactions(string username)
    {
        var users = fetchUsers();
        var user = users.FirstOrDefault(a => a.username == username);
        if (user != null)
        {
            user.Transactions.Clear();
            saveUsers(users);
        }
    }

    public void updateCashInflows(string username, int newCashInflow)
    {
        var users = fetchUsers();
        var user = users.FirstOrDefault(a => a.username == username);
        if (user != null)
        {
            user.cashInflow += newCashInflow;

            if (user.Transactions == null)
            {
                user.Transactions = new List<Transaction>();
            }

            user.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Type = "Inflow",
                Amount = newCashInflow,
                Description = $"{newCashInflow} successfully added to cash inflow by {activeUsername}."
            });

            saveUsers(users);
        }
    }

    public void updateCashOutflow(string username, int newCashOutflow)
    {
        var users = fetchUsers();
        var user = users.FirstOrDefault(a => a.username == username);
        if (user != null)
        {
            if (user.cashInflow >= newCashOutflow)
            {
                user.cashOutflow += newCashOutflow;
                user.cashInflow -= newCashOutflow;
            }

            if (user.Transactions == null)
            {
                user.Transactions = new List<Transaction>();
            }

            user.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Type = "Outflow",
                Amount = newCashOutflow,
                Description = $"{newCashOutflow} successfully added to cash outflow by {activeUsername}."
            });

            saveUsers(users);
        }
    }

    public void updateDebt(string username, int newDebt)
    {
        var users = fetchUsers();
        var user = users.FirstOrDefault(a => a.username == username);
        if (user != null)
        {
            user.cashInflow += newDebt;
            user.debt += newDebt;
        }

        if (user.Transactions == null)
        {
            user.Transactions = new List<Transaction>();
        }

        user.Transactions.Add(new Transaction
        {
            Date = DateTime.Now,
            Type = "Debt",
            Amount = newDebt,
            Description = $"{activeUsername} has successfully taken {newDebt} as Debt"
        });
        saveUsers(users);
    }

    public void clearDebt(string username)
    {
        var users = fetchUsers();
        var user = users.FirstOrDefault(a => a.username == username);
        if (user != null && user.cashInflow >= user.debt && user.debt != 0)
        {
            if (user.Transactions == null)
            {
                user.Transactions = new List<Transaction>();
            }


            user.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Type = "Debt",
                Amount = user.debt,
                Description = $"{activeUsername} has cleared {user.debt} from Debt."
            });

            user.cashInflow -= user.debt;
            user.debt = 0;

            saveUsers(users);
        }
        else if (user.cashInflow < user.debt)
        {
            throw new Exception("Ops, Your debt amount is greater than your credit balance.");
        }
    }

    public void resetDebit(string username)
    {
        var users = fetchUsers();
        var user = users.FirstOrDefault(u => u.username == username);
        user.cashOutflow = 0;
        saveUsers(users);
    }

    public User? getLoggedInUser()
    {
        var users = fetchUsers();
        return users.FirstOrDefault(u => u.username == activeUsername);

    }


    public List<User> fetchUsers()
    {
        if (!File.Exists(FilePath))
            return new List<User>();

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }
    public void saveUsers(List<User> users)
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }

        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, "[]");
        }

        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }
    public string hashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    public bool ValidatePassword(string inputPassword, string storedPassword)
    {
        var hashedInputPassword = hashPassword(inputPassword);
        return hashedInputPassword == storedPassword;
    }
}

