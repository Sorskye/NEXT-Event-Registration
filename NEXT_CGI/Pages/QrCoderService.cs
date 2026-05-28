using QRCoder;

namespace NEXT.wwwroot
{
    public class QrCoderService
    {
        public byte[] GeneratePng(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(20);
        }
    }
}
