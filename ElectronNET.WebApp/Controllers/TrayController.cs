using System.IO;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace ElectronNET.WebApp.Controllers
{
    public class TrayController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _viewPath = $"http://localhost:{BridgeSettings.WebPort}/windows/demowindow";

        public TrayController(IWebHostEnvironment env)
        {
            _env = env;
        }


        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                Electron.IpcMain.On("put-in-tray", (args) =>
                {

                    if (Electron.Tray.MenuItems.Count == 0)
                    {
                        var menu = new MenuItem
                        {
                            Label = "Remove",
                            Click = () => Electron.Tray.Destroy()
                        };

						//Electron.Tray.Show(Path.Combine(_env.ContentRootPath, "Assets/electron_32x32.png"), menu);
						Electron.Tray.Show( Path.Combine( _env.ContentRootPath, "Assets/electron_32x32.png" ) );
                        Electron.Tray.OnClick += async ( e, r ) => { await ShowWindow( r ); };
                        Electron.Tray.OnRightClick += ( _, _ ) => Electron.Tray.Destroy();
						Electron.Tray.SetToolTip("Electron Demo in the tray.");
                    }
                    else
                    {
                        Electron.Tray.Destroy();
                    }

                });
            }

            return View();
        }

        private async Task ShowWindow( Rectangle r )
        {
			var display = await Electron.Screen.GetDisplayMatchingAsync( r );
				
			var options = new BrowserWindowOptions
			{
				Frame = false,
				Height = 400,
				Width = 250,
				X = System.Math.Min( r.X + (r.Width/2) - 125, display.WorkArea.Width - 250 ),
				Y = System.Math.Max( 0, r.Y - 10 - 400 )
			};
			await Electron.WindowManager.CreateWindowAsync( options, _viewPath );
		}
    }
}