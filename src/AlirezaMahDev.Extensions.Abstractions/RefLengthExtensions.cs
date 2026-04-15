namespace AlirezaMahDev.Extensions.Abstractions;

public static class RefLengthExtensions
{
    extension<TSelf>(TSelf self)
        where TSelf : IRefLength, allows ref struct
    {
        public bool IsEmpty => self.Length == 0;
    }
}