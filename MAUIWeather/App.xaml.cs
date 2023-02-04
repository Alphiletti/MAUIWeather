using MAUIWeather.MVVM.Views;

namespace MAUIWeather;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new WeatherView();
	}
}
