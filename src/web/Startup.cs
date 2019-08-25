using Akka.Actor;
using core;
using core.Portfolio;
using core.Stocks;
using financialmodelingclient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using storage;

namespace web
{
	public class Startup
	{
		private ActorSystem _system;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			// In production, the Angular files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});

			services.AddSingleton<IStocksService, StocksService>();
			services.AddSingleton<IAnalysisStorage>(s => {
				var cnn = this.Configuration.GetConnectionString("db");
				return new AnalysisStorage(cnn);
			});

			services.AddSingleton<IPortfolioStorage>(s => {
				var cnn = this.Configuration.GetConnectionString("db");
				return new PortfolioStorage(cnn);
			});

			services.AddSingleton<IActorRef>(s => {
				var stocks = s.GetService<StocksService>();
				var storage = s.GetService<IAnalysisStorage>();
				var props = Props.Create(() => new AnalysisCoordinator(stocks, storage));
				return _system.ActorOf(props, "coordinator");
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseSpaStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller}/{action=Index}/{id?}");
			});

			app.UseSpa(spa =>
			{
				// To learn more about options for serving an Angular SPA from ASP.NET Core,
				// see https://go.microsoft.com/fwlink/?linkid=864501

				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseAngularCliServer(npmScript: "start");
				}
			});

			this._system = ActorSystem.Create("analysis");
		}
	}
}