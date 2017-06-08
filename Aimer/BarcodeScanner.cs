using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
using Windows.Storage.Streams;
using PosBarcodeScanner = Windows.Devices.PointOfService.BarcodeScanner;

namespace Aimer
{
    public class BarcodeScannedEventArgs : EventArgs
    {
        public string Text { get; set; }
        public BarcodeScannedEventArgs(string text)
        {
            Text = text;
        }
    }
    public class BarcodeScanner : IDisposable
    {
        private ClaimedBarcodeScanner scanner;

        public event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;

        ~BarcodeScanner()
        {
            this.Dispose(false);
        }

        public bool Exists
        {
            get
            {
                return this.scanner != null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task StartAsync()
        {
            if (this.scanner == null)
            {

                var scanner = await PosBarcodeScanner.GetDefaultAsync();
                var s = PosBarcodeScanner.GetDeviceSelector();
                var collection = await DeviceInformation.FindAllAsync(s);
                if (collection != null && collection.Count > 0)
                {
                    var identity = collection[1].Id;// First().Id;
                    var device = await PosBarcodeScanner.FromIdAsync(identity);
                    if (device != null)
                    {
                        this.scanner = await device.ClaimScannerAsync();
                        if (this.scanner != null)
                        {
                            this.scanner.IsDecodeDataEnabled = true;
                            this.scanner.ReleaseDeviceRequested += WhenScannerReleaseDeviceRequested;
                            this.scanner.DataReceived += WhenScannerDataReceived;

                            await this.scanner.EnableAsync();
                        }
                    }
                }
            }
        }

        private void WhenScannerDataReceived(object sender, BarcodeScannerDataReceivedEventArgs args)
        {
            var data = args.Report.ScanDataLabel;

            using (var reader = DataReader.FromBuffer(data))
            {
                var text = reader.ReadString(data.Length);
                var bsea = new BarcodeScannedEventArgs(text);
                this.BarcodeScanned?.Invoke(this, bsea);
            }
        }

        private void WhenScannerReleaseDeviceRequested(object sender, ClaimedBarcodeScanner args)
        {
            args.RetainDevice();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.scanner = null;
            }
        }
    }
}
