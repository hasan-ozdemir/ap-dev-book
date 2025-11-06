using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSnippets;

internal static class ObjectModelShowcase
{
    private static readonly Predicate<decimal> HighValuePredicate = balance => balance >= 1_000m;

    public static async Task RunAsync()
    {
        Console.WriteLine("== Object model showcase ==");

        var account = new CustomerAccount("C-1024")
        {
            PreferredCurrency = Currency.FromCode("USD"),
            PreferredTier = SubscriptionTier.Premium | SubscriptionTier.Analytics
        };

        var auditor = new AccountAuditor();
        auditor.BalanceChanged += (_, e) =>
        {
            if (HighValuePredicate(e.NewBalance))
            {
                Console.WriteLine($"High-value change detected: {e.NewBalance:C}");
            }
        };

        auditor.Subscribe(account);

        account.Credit(1_250m);

        IAccountNotifier notifier = new ConsoleAccountNotifier();
        await notifier.NotifyAsync(account, CancellationToken.None);

        var accounts = new List<CustomerAccount>
        {
            account,
            new CustomerAccount("C-2048")
            {
                PreferredCurrency = Currency.FromCode("EUR"),
                PreferredTier = SubscriptionTier.Standard
            }
        };

        var premiumAccounts = accounts.FindAll(acc => acc.PreferredTier.HasFlag(SubscriptionTier.Premium));
        Console.WriteLine($"Premium accounts count: {premiumAccounts.Count}");
        Console.WriteLine($"Namespace: {typeof(CustomerAccount).Namespace}");
    }
}

public sealed class CustomerAccount
{
    public CustomerAccount(string id)
    {
        Id = id;
    }

    public string Id { get; }

    public Currency PreferredCurrency { get; init; } = Currency.FromCode("USD");

    public SubscriptionTier PreferredTier { get; init; } = SubscriptionTier.Standard;

    public decimal Balance { get; private set; }

    public event EventHandler<BalanceChangedEventArgs>? BalanceChanged;

    public void Credit(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        var previous = Balance;
        Balance += amount;
        OnBalanceChanged(previous, Balance);
    }

    private void OnBalanceChanged(decimal previous, decimal current)
        => BalanceChanged?.Invoke(this, new BalanceChangedEventArgs(this, previous, current));
}

public readonly record struct Currency
{
    private Currency(string code)
    {
        Code = code;
    }

    public string Code { get; }

    public static Currency FromCode(string code)
    {
        ArgumentException.ThrowIfNullOrEmpty(code);
        return new Currency(code.ToUpperInvariant());
    }

    public decimal ConvertTo(decimal amount, Currency target, decimal rate)
        => target.Code.Equals(Code, StringComparison.OrdinalIgnoreCase)
            ? amount
            : Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);

    public override string ToString() => Code;
}

[Flags]
public enum SubscriptionTier
{
    Standard = 0,
    Premium = 1 << 0,
    Analytics = 1 << 1,
    Support = 1 << 2
}

public interface IAccountNotifier
{
    Task NotifyAsync(CustomerAccount account, CancellationToken token);
}

public sealed class ConsoleAccountNotifier : IAccountNotifier
{
    public Task NotifyAsync(CustomerAccount account, CancellationToken token)
    {
        Console.WriteLine($"Notify {account.Id}: balance {account.Balance:C} ({account.PreferredTier})");
        return Task.CompletedTask;
    }
}

public sealed class AccountAuditor
{
    public event EventHandler<BalanceChangedEventArgs>? BalanceChanged;

    public void Subscribe(CustomerAccount account)
    {
        account.BalanceChanged += OnAccountBalanceChanged;
    }

    private void OnAccountBalanceChanged(object? sender, BalanceChangedEventArgs e)
        => BalanceChanged?.Invoke(this, e);
}

public sealed class BalanceChangedEventArgs : EventArgs
{
    public BalanceChangedEventArgs(CustomerAccount account, decimal previousBalance, decimal newBalance)
    {
        Account = account;
        PreviousBalance = previousBalance;
        NewBalance = newBalance;
    }

    public CustomerAccount Account { get; }

    public decimal PreviousBalance { get; }

    public decimal NewBalance { get; }
}
