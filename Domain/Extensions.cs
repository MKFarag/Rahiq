namespace Domain;

internal static class Extensions
{
    extension(decimal price)
    {
        internal decimal ApplyDiscount(int percentage)
            => price - (price * percentage / 100);
    }
}
