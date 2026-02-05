------------------------------------------------------------------------------------
Guideline to use Syncfusion ASP.NET MVC libraries
------------------------------------------------------------------------------------

(1) Copy Syncfusion DLLs:
- Syncfusion.EJ.dll
- Syncfusion.EJ.xml
- Syncfusion.EJ.MVC.dll
- Syncfusion.EJ.MVC.xml

(2) Copy Syncfusion CSS and Javascripts:
Content\ej\default-theme
Content\ej\ej.widgets.core.bootstrap.less
Content\ej\ej.widgets.core.bootstrap.min.css
Content\ej\ej.widgets.core.less
Content\ej\ej.widgets.core.min.css
Content\ej\default-theme\ej.theme.less
Content\ej\default-theme\ej.theme.min.css
Content\ej\default-theme\ej.web.all.min.css
Content\ej\default-theme\images
Content\ej\default-theme\images\ajax-loader.gif
Content\ej\default-theme\images\checkedtick.png
Content\ej\default-theme\images\dot.png
Content\ej\default-theme\images\drop-sibling.png
Content\ej\default-theme\images\gradient.png
Content\ej\default-theme\images\palette-arrow.png
Content\ej\default-theme\images\rating-star.png
Content\ej\default-theme\images\rotator-icon.png
Content\ej\default-theme\images\toggle-text.png
Content\ej\default-theme\images\toolbar_icons.png
Content\ej\default-theme\images\ui-icon.png
Content\ej\default-theme\images\waitingpopup.gif
Scripts\ej.web.all.min.js

(3) Add the following keys to application Web.config:
  <appSettings>
    <add key="ClientValidationEnabled" value="true"/>
    <add key="UnobtrusiveJavaScriptEnabled" value="false"/>
  </appSettings>
  <system.web>
    <compilation debug="true" targetFramework="4.5.2">
      <assemblies>
        <add assembly="Syncfusion.EJ, Version=14.4450.0.15, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89" />
        <add assembly="Syncfusion.EJ.Mvc, Version=14.4500.0.15, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89" />
      </assemblies>
    </compilation>
  </system.web>
  <system.webServer>
    <validation validateIntegratedModeConfiguration="false" />
  </system.webServer>

(4) Add the following keys to Views' Web.config:
  <system.web.webPages.razor>
    <pages pageBaseType="System.Web.Mvc.WebViewPage">
      <namespaces>
        <add namespace="Syncfusion.JavaScript"/>
        <add namespace="Syncfusion.JavaScript.Shared"/>
        <add namespace="Syncfusion.MVC.EJ"/>
        <add namespace="YourProjectNameSpace.Models" />
      </namespaces>
    </pages>
  </system.web.webPages.razor>

(5) Put CSS & Javascript references in the cshtml head:
    <link rel="stylesheet" href="~/assets/vendor/ej/ej.widgets.core.min.css" />
    <link rel="stylesheet" href="~/assets/vendor/ej/default-theme/ej.theme.min.css" />
    <script src="~/assets/vendor/ej/ej.web.all.min.js"></script>


(6) Put EJ ScriptManager in the cshtml body:
  @(Html.EJ().ScriptManager())



Example for AutoComplete:
cshtml:
            @using (Html.BeginForm())
            {
                Html.EJ()
                .Autocomplete("selectCar")
                .Width("100%")
                .Datasource((IEnumerable<CarsList>)ViewBag.datasource)
                .WatermarkText("Select a car")
                .FilterType(FilterOperatorType.Contains)
                .Value(Model.selectCar)
                .Render();
              }

Controller:
        public ActionResult Index()
        {
            ViewBag.datasource = CarsList.GetCarList();
            IndexModel mdl = new IndexModel();
            mdl.selectCar = string.Empty;
            return View(mdl);
        }

        [HttpPost]
        public ActionResult Index(IndexModel mdl)
        {
            ViewBag.datasource = CarsList.GetCarList();
            IndexModel mdl2 = new IndexModel();
            mdl2.selectCar = mdl.selectCar;
            return View(mdl2);
        }
