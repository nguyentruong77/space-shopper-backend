namespace SpaceShopper.Application.Common.Files
{
    /// <summary>
    /// Kiểm tra magic bytes khớp với đuôi file đã whitelist (JPEG, PNG, WebP).
    /// </summary>
    public static class FileContentSignatureValidator
    {
        public static bool MatchesExtension(ReadOnlySpan<byte> header, string extensionNormalized)
        {
            return extensionNormalized switch
            {
                ".jpg" or ".jpeg" => IsJpeg(header),
                ".png" => IsPng(header),
                ".webp" => IsWebP(header),
                _ => false
            };
        }

        private static bool IsJpeg(ReadOnlySpan<byte> h) =>
            h.Length >= 3 && h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF;

        private static bool IsPng(ReadOnlySpan<byte> h)
        {
            ReadOnlySpan<byte> sig = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
            return h.Length >= sig.Length && h[..sig.Length].SequenceEqual(sig);
        }

        private static bool IsWebP(ReadOnlySpan<byte> h)
        {
            if (h.Length < 12)
            {
                return false;
            }

            if (h[0] != (byte)'R' || h[1] != (byte)'I' || h[2] != (byte)'F' || h[3] != (byte)'F')
            {
                return false;
            }

            return h[8] == (byte)'W'
                && h[9] == (byte)'E'
                && h[10] == (byte)'B'
                && h[11] == (byte)'P';
        }
    }
}
